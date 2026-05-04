import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { User } from '../shared/models/user.model';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
  user?: User;
  constructor(public auth: AuthService) { }

  ngOnInit() {
    this.user = this.auth.getUser().user; // Load from localStorage
  }

}
