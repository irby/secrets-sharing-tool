import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { SecretSubmissionRequest } from '../models/SecretSubmissionRequest';
import { SecretSubmissionResponse } from '../models/SecretSubmissionResponse';
import { TimeOption } from '../models/TimeOption';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

  itle = 'spa';
  secretText = new FormControl('');
  timeExpiryOptions: TimeOption[] = [];
  errorMessage = '';
  secretCreationResponse: SecretSubmissionResponse | null = null;
  appUrl: string;
  expireDateTime: string = '';
  isLoading: boolean = false;
  isSystemError: boolean = false;
  isCopied: boolean = false;

  constructor(private http: HttpClient){
    this.appUrl = environment.appUrl;
  }

  ngOnInit() {
    this.timeExpiryOptions = [
      new TimeOption('5 minutes', 60 * 5),
      new TimeOption('10 minutes', 60 * 10),
      new TimeOption('30 minutes', 60 * 30),
      new TimeOption('1 hour', 60 * 60),
      new TimeOption('24 hours', 60 * 60 * 24)
    ];
  }

  copyText() {
    this.selectText();
    document.execCommand('copy');
    this.isCopied = true;
  }

  selectText(){
    const textElement = document.getElementById("secretUrl") as HTMLInputElement;
    textElement?.focus();
    textElement?.select();
  }

  async submit() {
    const option = (document.getElementById("timeOptions") as HTMLInputElement);
    this.errorMessage = '';
    this.secretCreationResponse = null;
    this.isLoading = true;
    this.isSystemError = false;
    this.isCopied = false;

    await this.http.post<SecretSubmissionRequest>(environment.apiUrl + '/api/secrets', 
      new SecretSubmissionRequest(this.secretText.value, parseInt(option.value))
      ).subscribe(data => {
        this.secretCreationResponse = data as unknown as SecretSubmissionResponse;
        const expiry = new Date(this.secretCreationResponse.expireDateTime);
        this.expireDateTime = `${expiry.getUTCMonth()+1}/${expiry.getUTCDate()}/${expiry.getUTCFullYear()} ${expiry.getUTCHours()}:${expiry.getUTCMinutes() < 10 ? '0' : ''}${expiry.getUTCMinutes()}:${expiry.getUTCSeconds() < 10 ? '0' : ''}${expiry.getUTCSeconds()}`;
      }, err => {
          if(err.status === 400) {
            this.errorMessage = err.error.message;
          } else {
            this.isSystemError = true;
          }
        this.errorMessage = err.error.message;
      });

    this.isLoading = false;


  }

  reset() {
    this.errorMessage = '';
    this.secretCreationResponse = null;
    this.isSystemError = false;
    this.isCopied = false;
    this.secretText.setValue('');
    this.isLoading = false;

    const secretText = (document.getElementById("secretText") as HTMLInputElement);
    secretText.disabled = false;
  }

}
