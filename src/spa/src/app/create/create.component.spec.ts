/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, getTestBed, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { CreateComponent } from './create.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AngularMaterialModule } from '../angular-material';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SecretSubmissionResponse } from '../models/SecretSubmissionResponse';
import axios from 'axios';

describe('CreateComponent', () => {
  let component: CreateComponent;
  let fixture: ComponentFixture<CreateComponent>;
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateComponent ]
      , imports: [ RouterTestingModule, HttpClientTestingModule, AngularMaterialModule, NoopAnimationsModule, FormsModule, ReactiveFormsModule ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should set timeExpiryOptions and expiryTimeInMinutes', () => {
      component.timeExpiryOptions = [];
      component.expiryTimeInMinutes = 0;

      component.ngOnInit();

      expect(component.timeExpiryOptions.length).toBe(5);
      
      expect(component.timeExpiryOptions[0].displayText).toBe("30 minutes");
      expect(component.timeExpiryOptions[0].timeInMinutes).toBe(30);

      expect(component.timeExpiryOptions[1].displayText).toBe("1 hour");
      expect(component.timeExpiryOptions[1].timeInMinutes).toBe(60);

      expect(component.timeExpiryOptions[2].displayText).toBe("8 hours");
      expect(component.timeExpiryOptions[2].timeInMinutes).toBe(480);

      expect(component.timeExpiryOptions[3].displayText).toBe("24 hours");
      expect(component.timeExpiryOptions[3].timeInMinutes).toBe(1440);

      expect(component.timeExpiryOptions[4].displayText).toBe("7 days");
      expect(component.timeExpiryOptions[4].timeInMinutes).toBe(10080);

      expect(component.expiryTimeInMinutes).toBe(30);
    });
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

  describe('submit', () => {
    beforeEach(() => {
      jest.mock('axios');
      axios.post = jest.fn();
    });

    it('should clear properties', async() => {
      const mockResponse: SecretSubmissionResponse = new SecretSubmissionResponse('TestId', 'TestKey', '2023-06-26T01:29:24.6827179+00:00', 1687742964);
      axios.post = jest.fn().mockImplementationOnce(() => ({
        type: 'data',
        data: mockResponse
      }));
      
      component.errorMessage = 'Test error message';
      component.isSystemError = true;
      component.isCopied = true;

      await component.submit();

      expect(component.errorMessage).toBe('');
      expect(component.isSystemError).toBe(false);
      expect(component.isCopied).toBe(false);
    });

    it('when request success, populates with results from API call', async() => {

      expect(component.secretCreationResponse).toBeFalsy();

      const mockResponse: SecretSubmissionResponse = new SecretSubmissionResponse('TestId', 'TestKey', '2023-06-26T01:29:24.6827179+00:00', 1687742964);
      axios.post = jest.fn().mockImplementationOnce(() => ({
        type: 'data',
        data: mockResponse
      }));

      await component.submit();
      
      expect(component.secretCreationResponse).toBeTruthy();
      expect(component.secretCreationResponse!.secretId).toBe(mockResponse.secretId);
      expect(component.secretCreationResponse!.key).toBe(mockResponse.key);
      expect(component.secretCreationResponse!.expireDateTime).toBe(mockResponse.expireDateTime);
      expect(component.secretCreationResponse!.expireDateTimeEpoch).toBe(mockResponse.expireDateTimeEpoch);
    });

    it('when request fails with 400, shows error message defined in API call', async() => {
      expect(component.secretCreationResponse).toBeFalsy();

      axios.post = jest.fn().mockRejectedValueOnce({
        response: {
          data: {
            message: 'A bad request!!'
          },
          status: 400,
          statusText: 'Bad Request'
        }
      });

      await component.submit();
      
      expect(component.secretCreationResponse).toBeFalsy();
      expect(component.errorMessage).toBe('A bad request!!');
    });

    it('when request fails with other status type, shows unexpected error occurred', async() => {
      expect(component.secretCreationResponse).toBeFalsy();

      axios.post = jest.fn().mockRejectedValueOnce({
        response: {
          data: {
            message: 'Not found!!!'
          },
          status: 500
        }
      });

      await component.submit();
      
      expect(component.secretCreationResponse).toBeFalsy();
      expect(component.errorMessage).toBe('An unexpected error has occured. Please try again.');
      expect(component.isSystemError).toBe(true);
    });
  });

  describe('reset', () => {
    it('should reset properties to expected values', () => {
      component.errorMessage = 'Error!';
      component.secretCreationResponse = new SecretSubmissionResponse('', '', '', 0);
      component.isSystemError = true;
      component.isCopied = true;
      component.secretText.setValue('');
      component.isLoading = true;
      component.charactersRemaining = 0;
      component.expiryTimeInMinutes = 0;

      component.reset();

      expect(component.errorMessage).toBe('');
      expect(component.secretCreationResponse).toBeNull();
      expect(component.isSystemError).toBe(false);
      expect(component.isCopied).toBe(false);
      expect(component.secretText.value).toBe('');
      expect(component.charactersRemaining).toBe(5000);
      expect(component.expiryTimeInMinutes).toBe(30);
    });
  });

  describe('onKeydownHandler', () => {
    const tab: string = "    ";

    it('should insert tab when called from start', () => {
      let secretInput: HTMLTextAreaElement = fixture.debugElement.query(By.css('#secretText')).nativeElement;
      secretInput.value = "";
      secretInput.selectionStart = 0;
      fixture.detectChanges();
      fixture.whenStable();

      const event = new KeyboardEvent('keydown');
      component.onKeydownHandler(event);
      fixture.detectChanges();
      fixture.whenStable();

      secretInput = fixture.debugElement.query(By.css('#secretText')).nativeElement;
      expect(secretInput.value).toBe(`${tab}`);
    });

    it('should insert tab when called between two elements', () => {
      let secretInput: HTMLTextAreaElement = fixture.debugElement.query(By.css('#secretText')).nativeElement;
      secretInput.value = "ab";
      secretInput.selectionStart = 1;
      fixture.detectChanges();
      fixture.whenStable();

      const event = new KeyboardEvent('keydown');
      component.onKeydownHandler(event);
      fixture.detectChanges();
      fixture.whenStable();

      secretInput = fixture.debugElement.query(By.css('#secretText')).nativeElement;
      expect(secretInput.value).toBe(`a${tab}b`);
    });
  });

  describe('changeExpiryTime', () => {
    it('calling with value sets expiry time to value', () => {
      component.expiryTimeInMinutes = 30;
      fixture.detectChanges();

      expect(component.expiryTimeInMinutes).toBe(30);

      component.changeExpiryTime(100);
      
      expect(component.expiryTimeInMinutes).toBe(100);
    });
  });

  describe('convertDateToString', () => {
    it('calling with date returns formatted string', () => {
      const dateTime = new Date("2023-07-05T05:06:03.4532984+00:00");
      const expected = "7/5/2023 05:06:03";
      const actual = component.convertDateToString(dateTime);

      expect(actual).toBe(expected);
    });
  });

  describe('padTimeUnit', () => {
    it('pads if value is less than 10 (low)', () => {
      const expected = "00";
      const actual = component.padTimeUnit(0);

      expect(actual).toBe(expected);
    });

    it('pads if value is less than 10 (high)', () => {
      const expected = "09";
      const actual = component.padTimeUnit(9);

      expect(actual).toBe(expected);
    });

    it('does not pad if value is equal to 10', () => {
      const expected = "10";
      const actual = component.padTimeUnit(10);

      expect(actual).toBe(expected);
    });

    it('does not pad if value is greater than 10', () => {
      const expected = "11";
      const actual = component.padTimeUnit(11);

      expect(actual).toBe(expected);
    });
  });
});
