import { Component } from '@angular/core';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class UploadComponent {

  files: File[] = [];
  questionType: string = 'mixed';
  questionCount: number = 10;
  difficulty: string = 'Medium';
  language: string = 'Hindi';

  loading = false;
  result: string[] = [];

  constructor(private api: ApiService) { }

  onFileSelect(event: any) {
    this.files = Array.from(event.target.files || []);
  }

  async upload() {
  if (!this.files.length) return;
  this.loading = true;
  this.result = [];

  try {
    const form = new FormData();
    form.append('file', this.files[0], this.files[0].name);
    form.append('QuestionType', this.questionType);
    form.append('QuestionCount', String(this.questionCount));
    form.append('Difficulty', this.difficulty);
    form.append('Language', this.language);

    const res = await this.api.generateQuestions(form).toPromise();
    if (!res?.questions) {
      alert('No questions generated.');
      return;
    }

    let text: string = res.questions as string;

    // Remove any leading numbers like "1. " before the first question
    text = text.replace(/^\s*\d+\.\s*/, '');

    // Normalize newlines and remove Markdown bold markers
    text = text.replace(/\*\*/g, '');
    text = text.replace(/\r\n/g, '\n');

    // Split text at newlines before Q1., Q2., etc.
    this.result = text
      .split(/\n(?=Q\d+\.)/)
      .map(q => q.trim())
      .filter(q => q.length > 0);

  } catch (err) {
    console.error(err);
    alert('Error, see console');
  } finally {
    this.loading = false;
  }
}

  downloadTXT() {
    if (!this.result.length) return;
    const text = this.result.map((q, i) => `${i + 1}. ${q}`).join('\n\n');
    const blob = new Blob([text], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'questions.txt';
    a.click();
    URL.revokeObjectURL(url);
  }

}
