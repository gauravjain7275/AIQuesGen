AIQuesGen – AI Powered Question Generator & Online Test Platform

AIQuesGen is a full-stack SaaS application that leverages AI (OpenAI GPT) to generate question papers from PDF study material and conduct online tests with automated evaluation.
The platform is designed for teachers and coaching institutes to reduce manual effort in question creation and enable seamless digital assessments.

Core Features-
- Generate questions from uploaded PDF using AI
- Supports MCQ, descriptive, and true/false formats
- Export generated question papers as text files
- Create and manage online tests
- Share test links with students (no login required)
- Timer-based test execution with auto submission
- Automatic scoring and result calculation
- Dashboard for managing tests and viewing analytics

Core Modules-
Question Paper Generator (Offline Workflow)
- Upload PDF study material
- Generate structured questions using AI
- Control difficulty level and question type
- Export generated questions as .txt file

Ideal for teachers who want ready-to-use question papers

Online Test System (Interactive Workflow)
- Create tests using generated or custom questions
- Configure duration, passing score, and rules
- Students can attempt tests via shareable link
- Built-in timer and auto submission
- Automatic evaluation and result calculation

Ideal for conducting online exams and assessments

AI Integration-
- Uses OpenAI GPT API for question generation
- Designed structured prompts to control output format and difficulty
- Parses and validates AI responses before storing
- Handles inconsistent AI outputs with validation logic

Tech Stack-

- Frontend: Angular
- Backend: .NET Web API
- Database: SQL Server
- AI Integration: OpenAI GPT API
- Authentication: JWT-based authentication

System Workflow-

- PDF Upload → AI Processing → Question Generation →

(1) Export as Question Paper

(2) Create Online Test → Student Attempt → Evaluation

Security & Best Practices-
- JWT-based authentication for secure APIs
- Password hashing implemented
- API usage tracking to control AI cost per user
- Environment-based configuration for API keys and secrets

Key Highlights-
- Built complete end-to-end SaaS workflow independently
- Integrated AI into a real-world educational use case
- Designed dual workflow: offline paper generation + online testing
- Implemented full test lifecycle (creation → attempt → evaluation → analytics)
- Developed scalable backend architecture with clean separation of concerns

Setup Instructions
- Backend (.NET API)
1. Open solution in Visual Studio
2. Configure appsettings.Development.json with:
   - OpenAI API Key
   - Database Connection String
   - JWT Secret
3. Run the API

- Frontend (Angular)

1. Navigate to frontend folder:
   cd frontend

2. Install dependencies:
   npm install

3. Run application:
   ng serve

4. Open browser:
   http://localhost:4200



Author

Gaurav Jain
Senior Full Stack Developer | AI Application Developer
