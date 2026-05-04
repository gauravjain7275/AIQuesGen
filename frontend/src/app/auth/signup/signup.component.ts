import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.scss']
})
export class SignupComponent {
 name = '';
  email = '';
  password = '';
  userType = 'Individual';

  constructor(private auth: AuthService, private router: Router) { }

  register() {
    this.auth.signup({
      username: this.name,
      email: this.email,
      password: this.password,
      userType: this.userType
    }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: err => alert(err.error?.message || 'Signup error')
    });
  }
}
