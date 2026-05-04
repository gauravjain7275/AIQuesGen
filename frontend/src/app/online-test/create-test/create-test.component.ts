import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiService } from 'src/app/api.service';

@Component({
  selector: 'app-create-test',
  templateUrl: './create-test.component.html',
  styleUrls: ['./create-test.component.scss']
})
export class CreateTestComponent {
  file?: File;
  testTitle = '';
  questionCount = 20;
  language = 'English';
  difficulty = 'Medium';
  durationMinutes?: number | null = null;
  passingScore?: number | null = null;
  linkExpiryDate?: Date | null = null;
  maxAttemptsTotal?: number | null = null;
  maxAttemptsPerStudent?: number | null = null;
  loading = false;
  previewTestId?: number;
  previewLink?: string;

  constructor(private api: ApiService, private router: Router) {}

  onFileSelected(event: any) {
    this.file = event.target.files?.[0];
  }

  async createTest() {
    if (!this.file) { alert('Please upload the PDF'); return; }
    if (!this.testTitle) { alert('Enter test title'); return; }
    this.loading = true;

    try {
      const form = new FormData();
      form.append('File', this.file, this.file.name);
      form.append('TestTitle', this.testTitle);
      form.append('CreatedByUserId', String(this.getCurrentUserId())); // implement from auth
      form.append('QuestionCount', String(this.questionCount));
      form.append('Language', this.language);
      form.append('Difficulty', this.difficulty);
      if (this.durationMinutes) form.append('TestDuration', String(this.durationMinutes));
      if (this.passingScore) form.append('PassingScore', String(this.passingScore));
      if (this.linkExpiryDate) form.append('LinkExpiryDate', String(this.linkExpiryDate));
      if (this.maxAttemptsTotal) form.append('MaxAttemptsTotal', String(this.maxAttemptsTotal));
      if (this.maxAttemptsPerStudent) form.append('MaxAttemptsPerStudent', String(this.maxAttemptsPerStudent));

      const res = await this.api.createTest(form).toPromise();
      // res => { testId, link }
      this.previewTestId = res?.testId;
      this.previewLink = res?.link;
      // navigate to preview page (teacher)
      this.router.navigate(['/test/preview', res?.testId]);
    } catch (err) {
      console.error(err);
      alert('Error creating test. See console.');
    } finally {
      this.loading = false;
    }
  }

  // Replace with actual auth service call
  getCurrentUserId(): number {
    const u = localStorage.getItem('user');
    if (!u) return 0;
    try { 
      const userObj = JSON.parse(u);
      const userId = userObj.user?.userId || 0;
      return userId; 
    } 
    catch 
    { 
      return 0; 
    }
  }

}
