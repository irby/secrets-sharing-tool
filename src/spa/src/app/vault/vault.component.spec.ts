/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { VaultComponent } from './vault.component';
import { RouterTestingModule } from '@angular/router/testing';
import { HttpClientModule } from '@angular/common/http';
import { AngularMaterialModule } from '../angular-material';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { from, of } from 'rxjs';
import axios, { Axios } from 'axios';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { SecretSubmissionRetrieveResponse } from '../models/SecretSubmissionRetrieveResponse';

describe('VaultComponent', () => {
  let component: VaultComponent;
  let fixture: ComponentFixture<VaultComponent>;
  let activatedRoute: ActivatedRoute;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VaultComponent ],
      imports: [ RouterTestingModule, HttpClientModule, AngularMaterialModule, NoopAnimationsModule, FormsModule, ReactiveFormsModule, HttpClientTestingModule ],
      providers: [
        { provide: ActivatedRoute, useValue: { paramMap: of({has:()=>true, get:()=>'10'}), queryParams: of({}) }}
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VaultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    activatedRoute = TestBed.inject(ActivatedRoute);
    jest.mock('axios');
    axios.get = jest.fn();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('calls getSecret with route and query parameters', async() => {
      component.getSecret = jest.fn();
      activatedRoute.queryParams = of({key: '123'});
      
      await component.ngOnInit();

      expect(component.getSecret).toHaveBeenCalledWith('10', '123');
    });
  });

  describe('getSecret', () => {
    it('errors when secretId is missing', async() => {
      component.parameterWarningMesssage = [];

      await component.getSecret(null, '123');

      expect(component.parameterWarningMesssage.length).toBe(1);
      expect(component.parameterWarningMesssage[0]).toBe("Secret ID is missing");
    });
    it('errors when key is missing', async() => {
      component.parameterWarningMesssage = [];

      await component.getSecret('Hello', null);

      expect(component.parameterWarningMesssage.length).toBe(1);
      expect(component.parameterWarningMesssage[0]).toBe("Private key is missing");
    });
    it('gets message from API call when successful', async() => {
      const mockResponse: SecretSubmissionRetrieveResponse = {message: 'Hello, World!'};
      axios.get = jest.fn().mockImplementationOnce(() => ({
        type: 'data',
        data: mockResponse
      }));
      
      await component.getSecret('hello', 'world');

      expect(component.secretMessage).toBe('Hello, World!');
    });

    it('shows not found message when API returns 404', async() => {
      component.parameterWarningMesssage = [];
      axios.get = jest.fn().mockRejectedValueOnce({
        response: {
          data: {
            message: 'A bad request!!'
          },
          status: 404,
          statusText: 'Not Found'
        }
      });
      
      await component.getSecret('hello', 'world');
      fixture.detectChanges();
      fixture.whenStable();

      expect(component.secretMessage).toBe(null);
      expect(component.isSystemError).toBe(false);
      const error: HTMLDivElement = fixture.debugElement.query(By.css('#not-found-error')).nativeElement;
      expect(error.textContent).toContain('Hmmm, we could not retrieve your secret...');
      expect(error.textContent).toContain('Either the secret not found, has expired, or has already been recovered – or your key was invalid.');
    });

    it('shows system message when API returns status other than 404', async() => {
      component.parameterWarningMesssage = [];
      axios.get = jest.fn().mockRejectedValueOnce({
        response: {
          data: {
            message: 'A bad request!!'
          },
          status: 500,
          statusText: 'Internal Server Error'
        }
      });
      
      await component.getSecret('hello', 'world');
      fixture.detectChanges();
      fixture.whenStable();

      expect(component.secretMessage).toBe(null);
      expect(component.isSystemError).toBe(true);
      const error: HTMLDivElement = fixture.debugElement.query(By.css('#system-error')).nativeElement;
      expect(error.textContent).toContain('An unexpected error has occured. Please try your request again later.');
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
      component.secretMessage = "Hello, World!";
      fixture.detectChanges();
      fixture.whenStable();
    });

    it('selects text', () => {
      expect(fixture.debugElement.query(By.css('#secretMessage:focus'))).toBeFalsy();

      component.selectText();

      expect(fixture.debugElement.query(By.css('#secretMessage:focus'))).toBeTruthy();
    })
  });

  describe('toggleSecretVisibility', () => {
    it('toggles isHidden to true when false', () => {
      component.isHidden = false;
      component.toggleSecretVisibility();
      component.isHidden = true;
    });
    it('toggles isHidden to false when true', () => {
      component.isHidden = true;
      component.toggleSecretVisibility();
      component.isHidden = false;
    })
  });


});
