import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { environment } from '../../environments/environment';
import { SecretSubmissionRequest } from '../models/SecretSubmissionRequest';
import { SecretSubmissionResponse } from '../models/SecretSubmissionResponse';
import { TimeOption } from '../models/TimeOption';
import { HostListener } from '@angular/core';
import axios, { AxiosError } from 'axios';
import { ErrorCodes } from '../enums/error-codes.enum';

@Component({
  selector: 'app-create',
  templateUrl: './create.component.html',
  styleUrls: ['./create.component.css']
})
export class CreateComponent implements OnInit {

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
  expiryTimeInMinutes: number = 0;

  constructor(private http: HttpClient){
    this.appUrl = environment.appUrl;
  }

  ngOnInit() {
    this.timeExpiryOptions = [
      new TimeOption('30 minutes', 30),
      new TimeOption('1 hour', 60),
      new TimeOption('8 hours', 60 * 8),
      new TimeOption('24 hours', 60 * 24),
      new TimeOption('7 days', 60 * 24 * 7)
    ];

    this.expiryTimeInMinutes = this.timeExpiryOptions[0].timeInMinutes;
  }

  copyText() {
    this.selectText();
    document.execCommand('copy');
    this.isCopied = true;
  }

  selectText() {
    const textElement = document.getElementById("secretUrl") as HTMLInputElement;
    textElement!.focus();
    textElement!.select();
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

    try {
      const response = await axios.post<SecretSubmissionResponse>(environment.apiUrl + '/api/secrets', 
      new SecretSubmissionRequest(this.secretText.value, this.expiryTimeInMinutes));
      this.secretCreationResponse = response.data;
      const expiry = new Date(this.secretCreationResponse.expireDateTime);
      this.expireDateTime = this.convertDateToString(expiry);
    }
    catch (error: any) {
      const err = error as AxiosError<HttpErrorResponse>;
      const errorResponse = err.response!;

      if (errorResponse.status === ErrorCodes.BadRequest) {
        this.errorMessage = errorResponse.data.message as string;
      }
      else {
        this.errorMessage = "An unexpected error has occured. Please try again.";
        this.isSystemError = true;
      }
    }

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

    this.expiryTimeInMinutes = this.timeExpiryOptions[0].timeInMinutes;
  }

  // Override the tab default behavior and instead treat tab like you would in a word editor
  @HostListener('document:keydown.tab', ['$event'])
  onKeydownHandler(event: KeyboardEvent) {
    event.preventDefault();
    const secretText = document.getElementById("secretText") as HTMLInputElement;
    
    const cursorPosition = secretText.selectionStart!;

    const secretValue = secretText.value;

    const firstHalf = secretValue.substring(0, cursorPosition);
    const secondHalf = secretValue.substring(cursorPosition);

    // Insert tab padding between the first half and second half
    secretText.value = firstHalf + this.tabPadding + secondHalf;

    // Place the cursor to the point at which it inserted the tab
    secretText.selectionStart = (firstHalf + this.tabPadding).length;
    secretText.selectionEnd = secretText.selectionStart;

    this.valueChange();
  }

  changeExpiryTime(value: number){
    this.expiryTimeInMinutes = value;
  }

  convertDateToString(date: Date): string {
    const month = date.getUTCMonth()+1;
    const day = date.getUTCDate();
    const year = date.getUTCFullYear();
    const hours = this.padTimeUnit(date.getUTCHours());
    const minutes = this.padTimeUnit(date.getUTCMinutes());
    const seconds = this.padTimeUnit(date.getUTCSeconds());
    return `${month}/${day}/${year} ${hours}:${minutes}:${seconds}`;
  }

  padTimeUnit(value: number): string {
    let result = '';
    if (value < 10) {
      result += '0';
    }
    result += value;
    return result;
  }

}
