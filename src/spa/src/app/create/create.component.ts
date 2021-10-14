import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { SecretSubmissionRequest } from '../models/SecretSubmissionRequest';
import { SecretSubmissionResponse } from '../models/SecretSubmissionResponse';
import { TimeOption } from '../models/TimeOption';
import { HostListener } from '@angular/core';

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
  maxCharacterCount: number = 5000;
  charactersRemaining: number = this.maxCharacterCount;
  tabPadding: string = '    ';
  expiryTimeInSeconds: number = 0;

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

    this.expiryTimeInSeconds = this.timeExpiryOptions[0].timeInSeconds;
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

  valueChange() {
    const text = document.getElementById("secretText") as HTMLInputElement;
    const charactersRemaining = document.getElementById("charactersRemaining") as HTMLInputElement;
    this.charactersRemaining = this.maxCharacterCount - text.value.length;

    if(this.charactersRemaining < 0) {
      charactersRemaining.classList.add('warn-text');
    } else if (charactersRemaining.classList.contains('warn-text')) {
      charactersRemaining.classList.remove('warn-text');
    }

    this.isSystemError = false;
    this.errorMessage = '';
  }

  async submit() {
    this.errorMessage = '';
    this.secretCreationResponse = null;
    this.isLoading = true;
    this.isSystemError = false;
    this.isCopied = false;

    await this.http.post<SecretSubmissionRequest>(environment.apiUrl + '/api/secrets', 
      new SecretSubmissionRequest(this.secretText.value, this.expiryTimeInSeconds)
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

    this.charactersRemaining = this.maxCharacterCount;

    this.expiryTimeInSeconds = this.timeExpiryOptions[0].timeInSeconds;
  }

  // Override the tab default behavior and instead treat tab like you would in a word editor
  @HostListener('document:keydown.tab', ['$event'])
  onKeydownHandler(event: KeyboardEvent) {
    event.preventDefault();
    const secretText = (document.getElementById("secretText") as HTMLInputElement);
    
    const cursorPosition = secretText.selectionStart;

    const secretValue = secretText.value;

    const firstHalf = secretValue.substring(0, cursorPosition!);
    const secondHalf = secretValue.substring(cursorPosition!);

    // Insert tab padding between the first half and second half
    secretText.value = firstHalf + this.tabPadding + secondHalf;

    // Place the cursor to the point at which it inserted the tab
    secretText.selectionStart = (firstHalf + this.tabPadding).length;
    secretText.selectionEnd = secretText.selectionStart;

    this.valueChange();
  }

  changeExpiryTime(value: number){
    this.expiryTimeInSeconds = value;
  }

}
