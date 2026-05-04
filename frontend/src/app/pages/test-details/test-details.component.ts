import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from 'src/app/api.service';

@Component({
  selector: 'app-test-details',
  templateUrl: './test-details.component.html',
  styleUrls: ['./test-details.component.scss']
})
export class TestDetailsComponent {
  testId!: number;
  test: any;
  loading = true;
  testTitle = '';

  constructor(private route: ActivatedRoute, private api: ApiService, private router: Router) {}

  ngOnInit() {
    this.testId = Number(this.route.snapshot.paramMap.get('id'));
    this.route.queryParams.subscribe(params => {
      this.testTitle = params['testTitle'];
    });
    this.loadDetails();
  }

  loadDetails() {
    this.api.getTestDetails(this.testId).subscribe({
      next: res => {
        this.test = res;
        this.loading = false;
      },
      error: () => {
        alert("Failed to load details");
        this.loading = false;
      }
    });
  }

  openPreview() {
    this.router.navigate(['/test/preview', this.testId]);
  }

  openStudentView() {
    this.router.navigate(['/test/start', this.testId], { state: { preview: true } });
  }

  copyLink() {
    navigator.clipboard.writeText(`${window.location.origin}/test/start/${this.testId}`);
    alert("Link copied!");
  }

  backClick() {
    this.router.navigate(['/my-tests']);
  }

}
