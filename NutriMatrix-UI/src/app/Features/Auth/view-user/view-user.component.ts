import {Component, OnInit} from '@angular/core';
import {AuthService} from '../Services/auth.service';
import {UserDto} from '../Models/UserDTO';
import {NgForOf, NgIf} from '@angular/common';
import {NutrientInfo} from '../../../Core/services/nutrient.service';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-view-user',
  imports: [
    NgForOf,
    NgIf
  ],
  templateUrl: './view-user.component.html',
  standalone: true,
  styleUrl: './view-user.component.css'
})
export class ViewUserComponent implements OnInit {
  public user!: UserDto | undefined;
  public nutrientMetadata: NutrientInfo[] = [];

  constructor(
    private authService: AuthService,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    this.user = this.authService.getUser();

    this.http.get<NutrientInfo[]>('assets/nutrient_attributes.json')
      .subscribe(data => {
        this.nutrientMetadata = data;
      });
  }

  getNutrientLabel(id: number): string {
    const match = this.nutrientMetadata.find(n => n.attr_id === id);
    return match ? `${match.name} (${match.unit})` : `Unknown (#${id})`;
  }
}
