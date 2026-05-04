export interface TestCreateResponse {
  testId: number;
  link: string;
}

export interface QuestionModel {
  questionId: number;
  questionText: string;
  optionA?: string;
  optionB?: string;
  optionC?: string;
  optionD?: string;
  correctOption?: string;
}

export interface TestDetails {
  testId: number;
  testTitle: string;
  numberOfQuestions: number;
  createdAt?: string;
  durationMinutes?: number | null;
  passingScore?: number | null;
}

export interface TestAccessResponse {
  allowed: boolean;
  reason?: string;
  needIdentity?: boolean;
}