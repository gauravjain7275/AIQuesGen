import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from 'src/app/api.service';
import { QuestionModel } from 'src/app/shared/models/test.model';

@Component({
  selector: 'app-test-preview',
  templateUrl: './test-preview.component.html',
  styleUrls: ['./test-preview.component.scss']
})
export class TestPreviewComponent {
  testId!: number;
  questions: QuestionModel[] = [];
  loading = true;
  testLink?: string;

  constructor(private route: ActivatedRoute, private api: ApiService, private router: Router) {}

  ngOnInit(): void {
    this.testId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadQuestions();
  }

  loadQuestions() {
    this.loading = true;
    this.api.getTestQuestions(this.testId).subscribe({
      next: qs => { this.questions = qs; this.loading = false; },
      error: err => { console.error(err); alert('Error loading questions'); this.loading = false; }
    });
  }

  // Publish / show link (assuming backend returned link earlier; we can build it)
  getShareableLink() {
    // If backend returns link, you can fetch it; else construct
    this.testLink = `${window.location.origin}/test/start/${this.testId}`;
    // show it (UI will display)
  }

  goToStudentView() {
    this.router.navigate(
    ['/test/start', this.testId],
    { queryParams: { preview: true } }
  );
  }

}
