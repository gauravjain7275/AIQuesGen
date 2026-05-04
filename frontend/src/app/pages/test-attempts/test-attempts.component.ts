import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from 'src/app/api.service';

@Component({
  selector: 'app-test-attempts',
  templateUrl: './test-attempts.component.html',
  styleUrls: ['./test-attempts.component.scss']
})
export class TestAttemptsComponent {
  testId!: number;
  attempts: any[] = [];
  loading = true;
  testTitle = '';

  constructor(private route: ActivatedRoute, private api: ApiService, private router: Router) {}

  ngOnInit() {
    this.testId = Number(this.route.snapshot.paramMap.get('id'));
    this.route.queryParams.subscribe(params => {
      this.testTitle = params['testTitle'];
    });
    this.load();
  }

  load() {
    this.api.getTestAttempts(this.testId).subscribe({
      next: res => {
        this.attempts = res;
        this.loading = false;
      }
    });
  }

  backClick() {
    this.router.navigate(['/my-tests']);
  }

}
