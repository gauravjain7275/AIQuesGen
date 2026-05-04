import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../environments/environment';
import { Observable } from 'rxjs';
import { QuestionModel, TestAccessResponse, TestCreateResponse, TestDetails } from './shared/models/test.model';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  base = environment.apiUrl;

  constructor(private http: HttpClient) { }

  generateQuestions(form: FormData) {
    return this.http.post<any>(`${this.base}/question/generate-questions`, form);
  }

  // Online test create
  createTest(form: FormData): Observable<TestCreateResponse> {
    return this.http.post<TestCreateResponse>(`${this.base}/test/create`, form);
  }

  // Get questions for a test (for preview or for student)
  getTestQuestions(testId: number): Observable<QuestionModel[]> {
    return this.http.get<QuestionModel[]>(`${this.base}/test/questions/${testId}`);
  }

  getTestWithQuestions(id: number) {
    return this.http.get<any>(`${this.base}/test/test-with-questions/${id}`);
  }

  // Get metadata
  getTestDetails(testId: number): Observable<TestDetails> {
    return this.http.get<TestDetails>(`${this.base}/test/${testId}`);
  }

  // Submit test answers
  submitTest(body: any): Observable<any> {
    return this.http.post<any>(`${this.base}/test/submit`, body);
  }

  getMyTests(userId: number) {
    return this.http.get<any[]>(`${this.base}/test/my-tests/${userId}`);
  }

  getTestAttempts(testId: number) {
    return this.http.get<any[]>(`${this.base}/test/attempts/${testId}`);
  }

  validateTestAccess(testId: number, participantEmail?: string) {
    const params = participantEmail ? new HttpParams().set('participantEmail', participantEmail) : undefined;

    return this.http.get<TestAccessResponse>(`${this.base}/test/validate/${testId}`, { params });
  }
}
