import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { UploadComponent } from './upload/upload.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { SignupComponent } from './auth/signup/signup.component';
import { LoginComponent } from './auth/login/login.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { RouterModule, Routes } from '@angular/router';
import { CreateTestComponent } from './online-test/create-test/create-test.component';
import { TestPreviewComponent } from './online-test/test-preview/test-preview.component';
import { StudentTestComponent } from './online-test/student-test/student-test.component';
import { MyTestsComponent } from './pages/my-tests/my-tests.component';
import { TestDetailsComponent } from './pages/test-details/test-details.component';
import { TestAttemptsComponent } from './pages/test-attempts/test-attempts.component';
import { TimeFormatPipe } from './shared/pipes/time-format.pipe';

const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', component: SignupComponent },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'generate-questions', component: UploadComponent },
  { path: 'create-test', component: CreateTestComponent },
    { path: 'test/preview/:id', component: TestPreviewComponent }, // teacher preview
    { path: 'test/start/:id', component: StudentTestComponent },
    { path: "my-tests", component: MyTestsComponent },
{ path: "test-details/:id", component: TestDetailsComponent },
{ path: "test-attempts/:id", component: TestAttemptsComponent},
];

@NgModule({
  declarations: [
    AppComponent,
    UploadComponent,
    SignupComponent,
    LoginComponent,
    DashboardComponent,
    NavbarComponent,
    CreateTestComponent,
    TestPreviewComponent,
    StudentTestComponent,
    MyTestsComponent,
    TestDetailsComponent,
    TestAttemptsComponent,
    TimeFormatPipe
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(routes)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
