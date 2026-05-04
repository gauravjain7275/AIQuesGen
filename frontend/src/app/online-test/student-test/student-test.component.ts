import { Component } from '@angular/core';
import { ActivatedRoute, NavigationStart, Router } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { ApiService } from 'src/app/api.service';
import { QuestionModel } from 'src/app/shared/models/test.model';

@Component({
  selector: 'app-student-test',
  templateUrl: './student-test.component.html',
  styleUrls: ['./student-test.component.scss']
})
export class StudentTestComponent {
  isPreviewMode = false;
  testId!: number;
  questions: QuestionModel[] = [];
  answers: { [key: number]: string } = {}; // questionId -> "A"/"B"/"C"/"D"
  participantName = '';
  participantEmail = '';
  loading = true;
  testTitle = '';

  // Timer
  durationMinutes: number = 0; // if provided by test metadata
  remainingSeconds: number | null = null;
  timerSub?: Subscription;
  result: any;
  showIdentityForm = false;

  startTestVisible = true;
  submitTestVisible = false;


  constructor(private api: ApiService, private route: ActivatedRoute, private router: Router) { }

  ngOnInit(): void {
    this.testId = Number(this.route.snapshot.paramMap.get('id'));
    this.route.queryParams.subscribe(params => {
      this.isPreviewMode = params['preview'] === 'true';
    });
    if (!this.isPreviewMode && localStorage.getItem("test_in_progress") === "true") {
      const storedTestId = Number(localStorage.getItem("test_id"));
      if (storedTestId === this.testId) {
        alert("You refreshed the page — the test is now submitted automatically.");
        this.forceSubmitDueToReload();
        return;
      }
    }

    history.pushState(null, '', location.href);
    window.onpopstate = () => {
      history.pushState(null, '', location.href);
      alert("You cannot go back during the test.");
    };

    window.addEventListener("beforeunload", this.blockRefresh);




    if (this.isPreviewMode) {
      this.loadTest();
      this.startTestVisible = false;
    }
  }

  forceSubmitDueToReload() {
    const payload = {
      TestId: this.testId,
      ParticipantName: this.participantName || "Unknown",
      ParticipantEmail: this.participantEmail || "Unknown",
      Answers: this.answers || {}
    };

    this.api.submitTest(payload).subscribe({
      next: res => {
        localStorage.removeItem("test_in_progress");
        localStorage.removeItem("test_id");

        // show result
        this.result = {
          percentage: res.score,
          correct: res.correct,
          wrong: res.total - res.correct,
          total: res.total
        };
      },
      error: err => {
        console.error(err);
        alert("Error submitting test after reload");
      }
    });
  }

  blockRefresh = (event: BeforeUnloadEvent) => {
    event.preventDefault();
    event.returnValue = '';
  }

  startTest() {
    if (this.isPreviewMode) {
      this.loadTest();
    }
    else {
      this.startTestVisible = false;
      // prevent navigating to any other angular route
      this.router.events.subscribe(event => {
        if (event instanceof NavigationStart) {
          if (!this.result) {
            alert("You cannot leave the test until you submit!");
            this.router.navigateByUrl(`/test/start/${this.testId}`);
          }
        }
      });

      this.api.validateTestAccess(this.testId, this.participantEmail).subscribe({
        next: (res) => {
          if (!res.allowed) {
            if (res.needIdentity) {
              // show UI to ask for participant email/name before proceeding
              this.showIdentityForm = true;
            } else {
              this.submitTestVisible = false;
              this.startTestVisible = true;
              alert(res.reason || 'Access denied');
            }
            this.loading = false;
            return;
          }

          // allowed: fetch questions and proceed
          localStorage.setItem("test_in_progress", "true");
          localStorage.setItem("test_id", this.testId.toString());
          this.loadTest();
        },
        error: err => {
          console.error(err);
          alert('Error validating test access');
          this.loading = false;
        }
      });
    }
  }

  goBack() {
    this.router.navigate(['/test/preview', this.testId]);
  }

  ngOnDestroy(): void {
    window.removeEventListener("beforeunload", this.blockRefresh);
    this.timerSub?.unsubscribe();
  }

  loadTest() {
    this.submitTestVisible = true;
    this.result = null;
    // fetch questions
    this.api.getTestWithQuestions(this.testId).subscribe({
      next: data => {
        this.testTitle = data.title;
        this.durationMinutes = data.duration;
        this.questions = data.questions;
        this.loading = false;

        if (data.duration) {
          this.durationMinutes = data.duration;
          this.startTimer(this.durationMinutes);
        }

      },
      error: err => {
        console.error(err);
        alert('Error loading test');
        this.loading = false;
      }
    });
  }

  startTimer(minutes: number) {
    this.remainingSeconds = minutes * 60;
    this.timerSub = interval(1000).subscribe(() => {
      if (this.remainingSeconds !== null) {
        this.remainingSeconds!--;
        if (this.remainingSeconds <= 0) {
          this.timerSub?.unsubscribe();
          this.submit();
        }
      }
    });
  }

  selectAnswer(qId: number, opt: string) {
    this.answers[qId] = opt;
  }

  submit() {
    localStorage.removeItem("test_in_progress");
    localStorage.removeItem("test_id");
    this.submitTestVisible = false;
    this.startTestVisible = false;
    if (!this.participantName) { alert('Enter name'); return; }
    // Build payload according to your backend model
    const payload = {
      TestId: this.testId,
      ParticipantName: this.participantName,
      ParticipantEmail: this.participantEmail || null,
      Answers: this.answers
    };

    this.api.submitTest(payload).subscribe({
      next: res => {
        this.result = {
          percentage: res.score,
          correct: res.correct,
          wrong: res.total - res.correct,
          total: res.total
        };
        // optionally navigate or display result
      },
      error: err => {
        console.error(err);
        alert('Submission failed');
      }
    });
  }

}
