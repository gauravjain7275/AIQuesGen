import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { SignupComponent } from './auth/signup/signup.component';
import { AuthGuard } from './auth/auth.guard';
import { DashboardComponent } from './dashboard/dashboard.component';
import { UploadComponent } from './upload/upload.component';
import { CreateTestComponent } from './online-test/create-test/create-test.component';
import { TestPreviewComponent } from './online-test/test-preview/test-preview.component';
import { StudentTestComponent } from './online-test/student-test/student-test.component';
import { MyTestsComponent } from './pages/my-tests/my-tests.component';
import { TestDetailsComponent } from './pages/test-details/test-details.component';
import { TestAttemptsComponent } from './pages/test-attempts/test-attempts.component';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },

  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: "generate-questions", component: UploadComponent , canActivate: [AuthGuard]},
  { path: 'create-test', component: CreateTestComponent, canActivate: [AuthGuard] },
  { path: 'test/preview/:id', component: TestPreviewComponent, canActivate: [AuthGuard] }, // teacher preview
  { path: 'test/start/:id', component: StudentTestComponent },
  { path: "my-tests", component: MyTestsComponent, canActivate: [AuthGuard] },
{ path: "test-details/:id", component: TestDetailsComponent, canActivate: [AuthGuard] },
{ path: "test-attempts/:id", component: TestAttemptsComponent, canActivate: [AuthGuard] },

  { path: '**', redirectTo: 'login' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}