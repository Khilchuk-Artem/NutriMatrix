import {Component, OnInit} from '@angular/core';
import {NutrientTracking, UserDto} from '../Models/UserDTO';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {UserSummaryService} from '../Services/user-summary.service';
import {AuthService} from '../Services/auth.service';
import {UpdateUserDTO} from '../Models/UpdateUserDTO';
import {NgForOf, NgIf} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {Router} from '@angular/router';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';
import {NUTRIENT_CATEGORIES} from '../../../Core/services/nutrient-categories';

@Component({
  selector: 'app-edit-user',
  imports: [
    ReactiveFormsModule,
    NgForOf,
    NgIf,
    FormsModule
  ],
  templateUrl: './edit-user.component.html',
  standalone: true,
  styleUrl: './edit-user.component.css'
})
export class EditUserComponent implements OnInit {
  user?: UserDto;
  editForm!: FormGroup;
  saving = false;
  errorMsg = '';
  public nutrientMetadata: NutrientInfo[] = [];

  constructor(
    private fb: FormBuilder,
    private userSummaryService: UserSummaryService,
    private authService: AuthService,
    private toastr:ToastrService,
    private router:Router,
    private http:HttpClient
  ) {}
  groupedControls: { category: string, controls: { control: AbstractControl, index: number }[] }[] = [];
  searchTerm: string = '';

  ngOnInit(): void {
    this.user = this.authService.getUser();
    this.editForm = this.fb.group({
      name: [this.user?.name || '', Validators.required],
      searchTerm: [''],
      nutrientsToTrack: this.fb.array(this.createNutrientsControls())
    });
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe(data => {
        this.nutrientMetadata = data;
      });
    this.editForm.get('searchTerm')?.valueChanges.subscribe(term => {
      this.searchTerm = (term || '') as string;
      this.groupControlsByCategory();
    });


    this.groupControlsByCategory();
  }

  get nutrients(): FormArray {
    return this.editForm.get('nutrientsToTrack') as FormArray;
  }

  private createNutrientsControls() {
    if (!this.user?.nutrientsToTrack) return [];

    return this.user.nutrientsToTrack.map(nutrient => {
      const nutrientMeta = this.nutrientMetadata.find(m => m.attr_id === nutrient.nutrientId);
      const category = Object.entries(NUTRIENT_CATEGORIES).find(([_, ids]) => ids.includes(nutrient.nutrientId))?.[0] || 'Other';

      return this.fb.group({
        id: [nutrient.id],
        userId: [nutrient.userId],
        nutrientId: [nutrient.nutrientId],
        targetAmount: [nutrient.targetAmount, [Validators.required, Validators.min(0)]],
        isActive: [nutrient.isActive],
        category: [category]
      });
    });
  }


  onSubmit() {
    if (this.editForm.invalid || !this.user) {
      return;
    }

    this.saving = true;
    this.errorMsg = '';

    const updateDto: UpdateUserDTO = {
      name: this.editForm.value.name,
      updateNutrients: this.editForm.value.nutrientsToTrack.map((n: any) => ({
        id: n.id,
        userId: this.user?.userId || '',
        nutrientId: n.nutrientId,
        targetAmount: n.targetAmount,
        isActive: n.isActive
      }))
    };


    this.userSummaryService.updateUserSummary(updateDto, this.user.userId).subscribe({
      next: (updatedUser) => {
        this.authService.updateUser(updatedUser);
        this.user = updatedUser;
        this.saving = false;

        this.toastr.success("Profile successfully updated")
        this.router.navigate(['/app/me'])
      },
      error: (err) => {
        this.errorMsg = 'Failed to save changes. Please try again.';
        this.saving = false;
      }
    });
  }

  resetForm() {
    if (!this.user) return;
    this.editForm.reset({
      name: this.user.name,
      nutrientsToTrack: this.user.nutrientsToTrack
    });

    // Reset nutrients FormArray controls manually:
    this.nutrients.clear();
    this.user.nutrientsToTrack.forEach(nutrient => {
      this.nutrients.push(this.fb.group({
        nutrientId: [nutrient.nutrientId],
        targetAmount: [nutrient.targetAmount, [Validators.required, Validators.min(0)]],
        isActive: [nutrient.isActive]
      }));
    });
  }
  getNutrientLabel(id: number|undefined): string {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? `${match.name} (${match.unit})` : `Unknown (#${id})`;
  }
  groupControlsByCategory() {
    const map = new Map<string, { control: AbstractControl, index: number }[]>();

    this.nutrients.controls.forEach((control, index) => {
      const nutrientId = control.get('nutrientId')?.value;
      const nutrientLabel = this.getNutrientLabel(nutrientId).toLowerCase();
      const category = control.get('category')?.value || 'Other';

      // Filter by search term
      if (!this.searchTerm || nutrientLabel.includes(this.searchTerm.toLowerCase())) {
        if (!map.has(category)) map.set(category, []);
        map.get(category)!.push({ control, index });
      }
    });
    console.log(this.searchTerm)

    const orderedCategories = Object.keys(NUTRIENT_CATEGORIES);

    this.groupedControls = orderedCategories
      .filter(category => map.has(category))
      .map(category => ({
        category,
        controls: map.get(category)!
      }));

    if (map.has('Other') && !orderedCategories.includes('Other')) {
      this.groupedControls.push({
        category: 'Other',
        controls: map.get('Other')!
      });
    }
  }

  searchTermChanged() {
    console.log(this.searchTerm)
    this.groupControlsByCategory();
  }
}
