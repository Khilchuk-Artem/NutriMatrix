import {Component, OnInit} from '@angular/core';
import {UserDto} from '../Models/UserDTO';
import {FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {UserSummaryService} from '../Services/user-summary.service';
import {AuthService} from '../Services/auth.service';
import {UpdateUserDTO} from '../Models/UpdateUserDTO';
import {NgForOf, NgIf} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {Router} from '@angular/router';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-edit-user',
  imports: [
    ReactiveFormsModule,
    NgForOf,
    NgIf
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

  ngOnInit(): void {
    this.user = this.authService.getUser();

    this.editForm = this.fb.group({

      name: [this.user?.name || '', Validators.required],
      nutrientsToTrack: this.fb.array(this.createNutrientsControls())
    });
    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe(data => {
        this.nutrientMetadata = data;
      });
  }

  get nutrients(): FormArray {
    return this.editForm.get('nutrientsToTrack') as FormArray;
  }

  private createNutrientsControls() {
    if (!this.user?.nutrientsToTrack) {
      return [];
    }
    return this.user.nutrientsToTrack.map(nutrient =>
      this.fb.group({
        id:[nutrient.id],
        userId:[nutrient.userId],
        nutrientId: [nutrient.nutrientId], // readonly
        targetAmount: [nutrient.targetAmount, [Validators.required, Validators.min(0)]],
        isActive: [nutrient.isActive]
      })
    );
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
  getNutrientLabel(id: number): string {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? `${match.name} (${match.unit})` : `Unknown (#${id})`;
  }
}
