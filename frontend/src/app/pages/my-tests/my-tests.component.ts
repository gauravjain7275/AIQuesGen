import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from 'src/app/api.service';
import { AuthService } from 'src/app/auth/auth.service';

@Component({
  selector: 'app-my-tests',
  templateUrl: './my-tests.component.html',
  styleUrls: ['./my-tests.component.scss']
})
export class MyTestsComponent {
  tests: any[] = [];
  userId = 0;

  // filters
  difficulty = "";
  fromDate = "";
  toDate = "";
  subject = "";

  constructor(private api: ApiService, private auth: AuthService, private router: Router) {}

  ngOnInit(): void {
    this.userId = this.auth.getCurrentUserId();
    this.loadTests();
  }

  loadTests() {
    this.api.getMyTests(this.userId).subscribe(res => {
      this.tests = res;
    });
  }

  applyFilters() {
    let filtered = [...this.tests];

    if (this.difficulty) {
      filtered = filtered.filter(x => x.difficulty === this.difficulty);
    }
    if (this.fromDate) {
      filtered = filtered.filter(x => new Date(x.createdOn) >= new Date(this.fromDate));
    }
    if (this.toDate) {
      filtered = filtered.filter(x => new Date(x.createdOn) <= new Date(this.toDate));
    }
    this.tests = filtered;
  }

  viewDetails(id: number, testTitle: string) {
    this.router.navigate(
    ['/test-details', id],
    { queryParams: { testTitle: testTitle } }
  );
  }

  viewAttempts(id: number, testTitle: string) {
    this.router.navigate(
    ['/test-attempts', id],
    { queryParams: { testTitle: testTitle } }
  );
  }

  editTest(id: number) {
    this.router.navigate(['/create-test'], { queryParams: { editId: id } });
  }

}
