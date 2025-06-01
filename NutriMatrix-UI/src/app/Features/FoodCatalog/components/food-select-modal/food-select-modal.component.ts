import {Component, ElementRef, EventEmitter, OnDestroy, OnInit, Output, ViewChild} from '@angular/core';
import {FoodDTO, FoodShortcutDTO, MeasureDto} from '../../models/food.models';
import {BrowserMultiFormatReader, IScannerControls} from '@zxing/browser';
import {NutrientInfo} from '../../../../Core/services/nutrient.service';
import {FormBuilder, FormsModule} from '@angular/forms';
import {FoodService} from '../../Services/food.service';
import {HttpClient} from '@angular/common/http';
import {Result} from '@zxing/library';
import {NgForOf, NgIf} from '@angular/common';
import { Input } from '@angular/core';
import {MeasureService} from '../../../FoodRecords/services/measure.service';
import {AuthService} from '../../../Auth/Services/auth.service';
import {UserDto} from '../../../Auth/Models/UserDTO';

@Component({
  selector: 'app-food-select-modal',
  imports: [
    NgIf,
    FormsModule,
    NgForOf
  ],
  templateUrl: './food-select-modal.component.html',
  standalone: true,
  styleUrl: './food-select-modal.component.css'
})
export class FoodSelectModalComponent implements OnInit, OnDestroy {
  @ViewChild('video') video!: ElementRef<HTMLVideoElement>;
  @Output() foodSelected = new EventEmitter<{
    editedFoodRecord:number|null;
    food: FoodDTO;
    amount: number;
    measure: MeasureDto;
  }>();
  @Output() modalClosed = new EventEmitter<void>();

  @Input() preselectedFood: FoodDTO | null = null;
  @Input() preselectedRecord: { amount: number; measure: MeasureDto } | null = null;
  editedRecordId:number|null = null;
  isOpen = false;
  isBarcodeActive = false;
  searchQuery = '';
  allShortcuts: FoodShortcutDTO[] = [];
  selectedFood: FoodDTO | null = null;
  selectedMeasure: string = '';
  selectedMeasureDto: MeasureDto | undefined;
  enteredAmount: number = 1;
  videoInputDevices: MediaDeviceInfo[] = [];
  selectedDeviceId: string | undefined;
  private codeReader = new BrowserMultiFormatReader();
  private controls?: IScannerControls;
  nutrientMetadata: NutrientInfo[] = [];
  includeNutrientIds!:number[];
  constructor(
    private fb: FormBuilder,
    private foodService: FoodService,
    private http: HttpClient,
    private measureService:MeasureService,
    private authService:AuthService
  ) {}

  ngOnInit(): void {
    this.includeNutrientIds = this.authService
      .getUser()?.nutrientsToTrack?.filter(n=>n.isActive).map(n => n.nutrientId) || [];

    this.loadDevices();
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json').subscribe((data) => {
      this.nutrientMetadata = data;
    });
  }

  ngOnDestroy(): void {
    this.stopScanner();
  }

  open(preselectedFood:{ amount: number; measure: MeasureDto|undefined } | null = null, editedRecordId:number|null = null): void {
    this.isOpen = true;
    this.editedRecordId = editedRecordId;
    this.foodService.getFoodShortcuts(1, 30,this.includeNutrientIds).subscribe((res) => {
      this.allShortcuts = res;
      if (preselectedFood) {
        this.measureService.getMeasureWithFood(preselectedFood.measure?.id||1).subscribe(res=>{
          this.selectedMeasure = res.name
          this.selectedMeasureDto = preselectedFood.measure
          this.enteredAmount = preselectedFood.amount
          this.allShortcuts = [res.food, ...this.allShortcuts]
          this.foodService.getFoodById(res.food.id,this.includeNutrientIds).subscribe(res=>{
            this.selectedFood = res;
          })
        })
        this.enteredAmount = this.preselectedRecord?.amount || 1;
      } else {
        this.selectedMeasure = '';
        this.selectedMeasureDto = undefined;
        this.enteredAmount = 1;
        this.foodService.getFoodShortcuts(1, 30, this.includeNutrientIds).subscribe((res) => {
          this.allShortcuts = res;
        });
      }
    });

  }

  close(): void {
    this.isOpen = false;
    this.selectedFood = null;
    this.searchQuery = '';
    this.isBarcodeActive = false;
    this.enteredAmount = 1;
    this.stopScanner();
    this.modalClosed.emit();
  }

  async loadDevices() {
    try {
      const devices = await navigator.mediaDevices.enumerateDevices();
      this.videoInputDevices = devices.filter((d) => d.kind === 'videoinput');
      if (this.videoInputDevices.length > 0) {
        //this.selectedDeviceId = this.videoInputDevices[0].deviceId;
      }
    } catch (err) {
      console.error('Error accessing camera:', err);
    }
  }

  async startScanner(deviceId: string) {
    try {
      const videoElement = this.video.nativeElement;
      this.stopScanner();
      this.controls = await this.codeReader.decodeFromVideoDevice(
        deviceId,
        videoElement,
        (result: Result | undefined, error) => {
          if (result) {
            this.handleBarcodeResult(result.getText());
          }
        }
      );
    } catch (err) {
      console.error('Error starting scanner:', err);
    }
  }

  stopScanner() {
    if (this.controls) {
      this.controls.stop();
    }
  }

  onDeviceSelect(event: Event) {
    const selectedId = (event.target as HTMLSelectElement).value;
    this.selectedDeviceId = selectedId;
    this.startScanner(selectedId);
  }

  onBarcodeClick() {
    this.isBarcodeActive = true;
    if (this.selectedDeviceId) {
      this.selectedFood = null;
      this.startScanner(this.selectedDeviceId);
    }
  }

  closeBarcode() {
    this.isBarcodeActive = false;
    this.stopScanner();
  }

  updateShortcuts() {
    if (this.searchQuery.length > 1 || this.searchQuery.length === 0) {
      this.foodService.getFoodShortcuts(1, 30, [], this.searchQuery).subscribe((res) => {
        this.allShortcuts = res;
      });
    }
  }

  selectFood(id: number) {
    this.foodService.getFoodById(id, this.includeNutrientIds).subscribe({
      next: (food) => {
        this.selectedFood = food;
        this.selectedMeasure = food.measures[0]?.name || '';
        this.selectedMeasureDto = food.measures[0];
      },
      error: (err) => {
        console.error('Failed to fetch food by id', err);
      },
    });
  }

  onSelected() {
    if (this.selectedFood) {
      this.selectedMeasureDto = this.selectedFood.measures.find(
        (m) => m.name === this.selectedMeasure
      );
    }
  }

  private handleBarcodeResult(barcode: string): void {
    if (!barcode) return;

    this.foodService.getFoodByBarcode(barcode, this.includeNutrientIds).subscribe({
      next: (food: FoodDTO) => {
        const shortcut: FoodShortcutDTO = {
          id: food.id,
          name: food.name,
          nutrients: food.foodNutrients,
        };

        if (!this.allShortcuts.some((s) => s.id === food.id)) {
          this.allShortcuts = [shortcut, ...this.allShortcuts];
        }

        this.searchQuery = '';
        this.selectedFood = food;
        this.selectedMeasure = food.measures[0]?.name || '';
        this.selectedMeasureDto = food.measures[0];
        this.isBarcodeActive = false;
        this.stopScanner();
      },
      error: (error) => {
        console.error('Error fetching food by barcode:', error);
      },
    });
  }

  submitFoodEntry() {
    if (!this.selectedFood || !this.enteredAmount || !this.selectedMeasureDto) return;
    console.log({
      editedFoodRecord:this.editedRecordId,
      food: this.selectedFood,
      amount: this.enteredAmount,
      measure: this.selectedMeasureDto,
    })
    this.foodSelected.emit({
      editedFoodRecord:this.editedRecordId,
      food: this.selectedFood,
      amount: this.enteredAmount,
      measure: this.selectedMeasureDto,
    });
    this.close();
  }

  getNutrientLabel(id: number): string[] {
    const match = this.nutrientMetadata.find((n) => n.attr_id === id);
    return match ? [match.name, match.unit] : ['', ''];
  }

  protected readonly Math = Math;
}
