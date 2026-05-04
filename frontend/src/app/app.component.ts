import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'question-gen';
  showNavbar = true;

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {

        // Routes where navbar should be hidden
        const hideNavbarRoutes = [
          '/login',
          '/register'
        ];

        // Student test route pattern: /test/start/:id
        const isStudentTest = event.urlAfterRedirects.startsWith('/test/start');

        if (isStudentTest) {
          this.showNavbar = false;
        } else {
          this.showNavbar = true;
        }
      }
    });
  }
}
