import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="profile-page">
      <div class="profile-container">
        <div class="profile-header">
          <div class="avatar">{{ initials }}</div>
          <div>
            <h1>{{ user?.firstName }} {{ user?.lastName }}</h1>
            <p class="email">{{ user?.email }}</p>
            <span class="role-badge">{{ user?.role }}</span>
          </div>
        </div>

        <div class="profile-card">
          <h2>Account Details</h2>
          <div class="detail-row">
            <span class="label">First Name</span>
            <span class="value">{{ user?.firstName }}</span>
          </div>
          <div class="detail-row">
            <span class="label">Last Name</span>
            <span class="value">{{ user?.lastName }}</span>
          </div>
          <div class="detail-row">
            <span class="label">Email</span>
            <span class="value">{{ user?.email }}</span>
          </div>
          <div class="detail-row">
            <span class="label">Role</span>
            <span class="value">{{ user?.role }}</span>
          </div>
        </div>

        <div class="profile-actions">
          <a routerLink="/orders" class="btn-secondary">View My Orders</a>
        </div>
      </div>
    </div>
  `,
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  constructor(public authService: AuthService) {}

  get user() {
    return this.authService.currentUser;
  }

  get initials(): string {
    const u = this.user;
    if (!u) return '?';
    return `${u.firstName?.[0] ?? ''}${u.lastName?.[0] ?? ''}`.toUpperCase();
  }
}
