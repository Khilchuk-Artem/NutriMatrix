import {AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {BarcodeFormat, BrowserMultiFormatReader, IScannerControls} from '@zxing/browser';
import {NgClass, NgForOf, NgIf} from '@angular/common';
import {ZXingScannerModule} from '@zxing/ngx-scanner';
import {DecodeHintType, Result} from '@zxing/library';
import {Calendar} from 'primeng/calendar';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {animate, style, transition, trigger} from '@angular/animations';
import {FoodDTO, FoodShortcutDTO, MeasureDto} from '../FoodCatalog/models/food.models';
import {FoodService} from '../FoodCatalog/food.service';
import {AuthService} from '../Auth/Services/auth.service';
import {NutrientInfo} from '../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';
import {AddFoodRecordDto, FoodRecordDto, FoodRecordsService} from '../FoodRecords/services/food-records.service';
import {MeasureService, MeasureWithFoodDto} from '../FoodRecords/services/measure.service';
import {MenuItem} from 'primeng/api';
import {Menu} from 'primeng/menu';

interface FoodRecordViewModel {
  record: FoodRecordDto;
  measure?: MeasureWithFoodDto;
}


@Component({
  selector: 'app-dashboard',
  imports: [
    NgIf,
    ZXingScannerModule,
    NgForOf,
    Calendar,
    FormsModule,
    ReactiveFormsModule,
    NgClass,
    Menu
  ],
  templateUrl: './dashboard.component.html',
  standalone: true,
  styleUrl: './dashboard.component.css',
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0 }),
        animate('150ms ease-out', style({ opacity: 1 })),
      ]),
      transition(':leave', [
        animate('150ms ease-in', style({ opacity: 0 })),
      ]),
    ]),
    trigger('scaleFade', [
      transition(':enter', [
        style({ opacity: 0, transform: 'scale(0.9)' }),
        animate('300ms cubic-bezier(0.4, 0, 0.2, 1)', style({ opacity: 1, transform: 'scale(1)' })),
      ]),
      transition(':leave', [
        animate('300ms cubic-bezier(0.4, 0, 0.2, 1)', style({ opacity: 0, transform: 'scale(0.9)' })),
      ]),
    ]),
  ],
})
export class DashboardComponent implements OnInit, OnDestroy {
  @ViewChild('video') video!: ElementRef<HTMLVideoElement>;
  private codeReader = new BrowserMultiFormatReader();
  result: string = '';
  videoInputDevices: MediaDeviceInfo[] = [];
  selectedDeviceId: string | null = null;
  private controls?: IScannerControls;
  date: Date = new Date(new Date().setHours(0, 0, 0, 0));
  allShortcuts: FoodShortcutDTO[] = [];
  public searchQuery = '';
  addFoodForm: FormGroup;
  isAddFoodModalOpen = false;
  foodEntries: { foodName: string; amount: string }[] = [];
  selectedFood: FoodDTO | null = null;
  includeIds:number[] = []
  public nutrientMetadata: NutrientInfo[] = [];
  public selectedMeasureDto: MeasureDto | undefined;
  public records!:FoodRecordDto[];
  public foodRecordViewModels!:FoodRecordViewModel[];
  constructor(private fb: FormBuilder,
              private foodService:FoodService,
              private authService:AuthService,
              private http:HttpClient,
              private foodRecordService:FoodRecordsService,
              private measureService:MeasureService) {
    this.addFoodForm = this.fb.group({
      foodName: ['', Validators.required],
      amount: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadDevices();
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe(data => {
        this.nutrientMetadata = data;
      });
    var user = this.authService.getUser();
    if (user && 'nutrientsToTrack' in user) {
      this.includeIds = user.nutrientsToTrack
        .filter(rec => rec.isActive)
        .map(r => r.nutrientId);

      console.log(this.includeIds)
    }


    this.foodRecordService.getAllRecords(user?.userId||'', true, this.date).subscribe(records => {
      const viewModels: FoodRecordViewModel[] = records.map(record => ({ record }));

      this.foodRecordViewModels = viewModels;

      for (const vm of viewModels) {
        this.measureService.getMeasureWithFood(vm.record.foodMeasureId).subscribe(measure => {
          vm.measure = measure;
        });
      }
    });
  }
  getNutrientLabel(id: number): string[] {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? [match.name, match.unit] : ['',''];
  }
  ngOnDestroy(): void {
    this.stopScanner();
  }

  async loadDevices() {
    try {
      const devices = await navigator.mediaDevices.enumerateDevices();
      this.videoInputDevices = devices.filter((d) => d.kind === 'videoinput');
      if (this.videoInputDevices.length > 0) {
        this.selectedDeviceId = this.videoInputDevices[0].deviceId;
        this.startScanner(this.selectedDeviceId);
      }
    } catch (err) {
      console.error('Error accessing camera:', err);
    }
  }

  /*async startScanner(deviceId: string) {
    try {
      const videoElement = this.video.nativeElement;

      this.stopScanner();

      this.controls = await this.codeReader.decodeFromVideoDevice(
        deviceId,
        videoElement,
        (result: Result | undefined, error) => {
          if (result) {
            this.result = result.getText();
            console.log('Scanned:', this.result);
          }
        }
      );
    } catch (err) {
      console.error('Error starting scanner:', err);
    }
  }*/

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

  // === Add Food modal methods ===
  searchControl: any;
  selectedMeasure!: string;
  enteredAmount: number = 1;
  openAddFoodModal() {
    this.isAddFoodModalOpen = true;
    this.foodService.getFoodShortcuts(1,30).subscribe(res=>{
      this.allShortcuts = res;
    })
  }

  closeAddFoodModal() {
    this.isAddFoodModalOpen = false;
    this.selectedFood = null;
  }

  submitAddFood() {
    if (this.addFoodForm.valid) {
      const newEntry = this.addFoodForm.value;
      this.foodEntries.push(newEntry);
      this.closeAddFoodModal();
    }
  }

  onBarcodeClick() {
    this.isBarcodeActive = true;
    this.result = 'null';
    if (this.selectedDeviceId != null) {
      this.selectedFood = null;
      this.startScanner(this.selectedDeviceId);
    }
  }

  selectFood(id: number) {
    this.foodService.getFoodById(id,this.includeIds).subscribe({
      next: (food) => {
        this.selectedFood = food;
        this.selectedMeasure = food.measures[0].name
        this.selectedMeasureDto = this.selectedFood.measures.find(m => m.name === this.selectedMeasure);

      },
      error: (err) => {
        console.error('Failed to fetch food by id', err);
      }
    });
  }
  updateShortcuts() {
    if(this.searchQuery.length>1||this.searchQuery.length==0){
      this.foodService.getFoodShortcuts(1,30,this.includeIds,this.searchQuery).subscribe(res=>{
        this.allShortcuts = res;
      })
    }
  }

  onSelected() {
    if (this.selectedFood ) {
      this.selectedMeasureDto = this.selectedFood.measures.find(m => m.name === this.selectedMeasure);
    }
  }

  protected readonly Math = Math;
  isBarcodeActive = false;


  closeBarcode() {
    this.isBarcodeActive = false;
    this.result = '';
    // Stop video stream / scanner if needed
  }
  // In the DashboardComponent class, add the following method:

  private handleBarcodeResult(barcode: string): void {
    if (!barcode) return;

    this.foodService.getFoodByBarcode(barcode, this.includeIds).subscribe({
      next: (food: FoodDTO) => {
        // Create a shortcut from the scanned food
        const shortcut: FoodShortcutDTO = {
          id: food.id,
          name: food.name,
          nutrients: food.foodNutrients
        };

        // Add to shortcuts if not exists
        if (!this.allShortcuts.some(s => s.id === food.id)) {
          this.allShortcuts = [shortcut, ...this.allShortcuts];
        }

        // Clear search and select the food
        this.searchQuery = '';
        this.selectedFood = food;
        this.selectedMeasure = food.measures[0]?.name || '';
        this.selectedMeasureDto = food.measures[0];

        // Switch back to main view
        this.isBarcodeActive = false;
        this.result = '';
        this.stopScanner();
      },
      error: (error) => {
        console.error('Error fetching food by barcode:', error);
        // Handle error (e.g., show error message)
      }
    });
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
            this.result = result.getText();
            this.handleBarcodeResult(this.result); // Add this line
          }
        }
      );
    } catch (err) {
      console.error('Error starting scanner:', err);
    }
  }

  onEdit(vm: FoodRecordViewModel) {
    console.log('Edit clicked for record id:', vm.record.id);
    // your edit logic here
  }

  onDelete(vm: FoodRecordViewModel) {
    if (!vm || !vm.record?.id) {
      console.error('Invalid record to delete');
      return;
    }

    // Optional: Confirm deletion
    if (!confirm(`Delete record for ${vm.measure?.food?.name || 'this item'}?`)) {
      return;
    }

    this.foodRecordService.deleteRecord(vm.record.id).subscribe({
      next: () => {
        // Remove from list after successful delete
        this.foodRecordViewModels = this.foodRecordViewModels.filter(x => x.record.id !== vm.record.id);
        console.log('Record deleted:', vm.record.id);
      },
      error: (err) => {
        console.error('Failed to delete record:', err);
      }
    });
  }

  submitFoodEntry() {
    if (!this.selectedFood || !this.enteredAmount) return;

    const dto: AddFoodRecordDto = {
      userId: this.authService.getUser()?.userId||"",
      foodMeasureId: this.selectedMeasureDto?.id||-1,
      amount: this.enteredAmount,
      dateEaten: new Date().toISOString(),
    };
    console.log(dto)
    this.foodRecordService.addRecord(dto).subscribe({
      next: (record) => {
        console.log('Record added:', record);
        // Optional: push to dashboard records collection or emit event
        this.closeAddFoodModal(); // Close modal
      },
      error: (err) => {
        console.error('Failed to add food record', err);
        // Optionally show an error message
      }
    });
    this.isAddFoodModalOpen = false;
    this.selectedFood = null;
    this.enteredAmount = 0;
    this.closeAddFoodModal()
  }


  onDateChange(event: Date): void {
    this.date = new Date(
      event.getFullYear(),
      event.getMonth(),
      event.getDate()
    );
    this.foodRecordService.getAllRecords(this.authService.getUser()?.userId||'', true, this.date).subscribe(records => {
      const viewModels: FoodRecordViewModel[] = records.map(record => ({ record }));

      this.foodRecordViewModels = viewModels;

      for (const vm of viewModels) {
        this.measureService.getMeasureWithFood(vm.record.foodMeasureId).subscribe(measure => {
          vm.measure = measure;
        });
      }
    });
  }
  @ViewChild('menu') menu!: Menu;

  menuItems: any[] = [];
  currentVm: any;

  openMenu(event: Event, vm: any) {
    this.currentVm = vm; // store current record for commands

    this.menuItems = [
      {
        label: 'Edit',
        icon: 'pi pi-pencil',
        command: () => this.onEdit(this.currentVm),
      },
      {
        label: 'Delete',
        icon: 'pi pi-trash',
        command: () => this.onDelete(this.currentVm),
      },
    ];

    this.menu.toggle(event);
  }

}
