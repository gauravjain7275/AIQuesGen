import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestAttemptsComponent } from './test-attempts.component';

describe('TestAttemptsComponent', () => {
  let component: TestAttemptsComponent;
  let fixture: ComponentFixture<TestAttemptsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [TestAttemptsComponent]
    });
    fixture = TestBed.createComponent(TestAttemptsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
