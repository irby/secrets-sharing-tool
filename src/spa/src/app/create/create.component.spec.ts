/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { CreateComponent } from './create.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientModule } from '@angular/common/http';
import { AngularMaterialModule } from '../angular-material';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SecretSubmissionResponse } from '../models/SecretSubmissionResponse';

describe('CreateComponent', () => {
  let component: CreateComponent;
  let fixture: ComponentFixture<CreateComponent>;
  let compiled: any;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateComponent ]
      , imports: [ RouterTestingModule, HttpClientModule, AngularMaterialModule, NoopAnimationsModule, FormsModule, ReactiveFormsModule ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    compiled = fixture.debugElement.nativeElement;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('copyText', () => {
    it('should call copy command and set isCopy to true', () => {
      document.execCommand = jest.fn();
      component.selectText = jest.fn();

      expect(component.isCopied).toBe(false);

      component.copyText();
      fixture.detectChanges();

      expect(component.isCopied).toBe(true);
      expect(document.execCommand).toHaveBeenCalledWith('copy');
      expect(component.selectText).toHaveBeenCalled();
    });
  });

  describe('selectText', () => {
    beforeEach(() => {
      component.secretCreationResponse = new SecretSubmissionResponse('TestId', 'TestKey', '2023-05-01', 1000);
      fixture.detectChanges();
      fixture.whenStable();
    });

    it('selects text', () => {
      expect(fixture.debugElement.query(By.css('#secretUrl:focus'))).toBeFalsy();

      component.selectText();

      expect(fixture.debugElement.query(By.css('#secretUrl:focus'))).toBeTruthy();
    })
  });

  describe('valueChange', () => {
    it('should decrease charactersRemaining when characters are added', () => {
      expect(component.charactersRemaining).toBe(5000);

      setSecretText('A');

      component.valueChange();
      fixture.detectChanges();

      expect(component.charactersRemaining).toBe(4999);
    });

    it('should increase charactersRemaining when characters are removed', () => {
      setSecretText('ABC');
      component.valueChange();

      expect(component.charactersRemaining).toBe(4997);

      setSecretText('AB');
      component.valueChange();

      expect(component.charactersRemaining).toBe(4998);
    });

    it('should apply warn-text to charactersRemaining when threshold is met', () => {
      expect(fixture.debugElement.query(By.css('.warn-text'))).toBeFalsy();

      setSecretText(createStringWithLength(5001));
      component.valueChange();

      expect(component.charactersRemaining).toBe(-1);
      expect(fixture.debugElement.query(By.css('.warn-text'))).toBeTruthy();
    });

    it('should remove warn-text from charactersRemaining when threshold no longer met', () => {
      setSecretText(createStringWithLength(5001));
      component.valueChange();

      expect(fixture.debugElement.query(By.css('.warn-text'))).toBeTruthy();

      setSecretText(createStringWithLength(5000));
      component.valueChange();

      expect(fixture.debugElement.query(By.css('.warn-text'))).toBeFalsy();
    });

    it('should set isSystemError and errorMessage to defaults when called', () => {
      component.isSystemError = true;
      component.errorMessage = 'Test error';
      fixture.detectChanges();

      expect(component.isSystemError).toBeTruthy();
      expect(component.errorMessage).toBeTruthy();

      component.valueChange();

      expect(component.isSystemError).toBeFalsy();
      expect(component.errorMessage).toBeFalsy();
    });

    function setSecretText(input: string): void {
      const inputField: HTMLInputElement = fixture.debugElement.query(By.css("#secretText")).nativeElement;
      inputField.value = input;
      fixture.detectChanges();
      fixture.whenStable();
    }

    function createStringWithLength(length: number): string {
      let result: string = "";
      const pad: string = "A";
      for (let i = 0; i < length; i++) {
        result += pad;
      }
      return result;
    }
  });

  describe('changeExpiryTime', () => {
    it('calling with value sets expiry time to value', () => {
      component.expiryTimeInMinutes = 30;
      fixture.detectChanges();

      expect(component.expiryTimeInMinutes).toBe(30);

      component.changeExpiryTime(100);
      
      expect(component.expiryTimeInMinutes).toBe(100);
    });
  })
});
