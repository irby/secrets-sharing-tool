import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { environment } from '../../environments/environment';
import { ErrorCodes } from '../enums/error-codes.enum';
import axios, { AxiosError } from 'axios';
import { SecretSubmissionRetrieveResponse } from '../models/SecretSubmissionRetrieveResponse';
import { HttpErrorResponse } from '@angular/common/http';


@Component({
  selector: 'app-vault',
  templateUrl: './vault.component.html',
  styleUrls: ['./vault.component.css']
})
export class VaultComponent implements OnInit {

  secretMessage: string | null = null;
  isLoading: boolean = true;
  systemError: string = "An unexpected error has occured. Please try your request again later.";
  isSystemError: boolean = false;
  parameterWarningMesssage: string[] = [];
  isCopied: boolean = false;
  isHidden: boolean = true;

  constructor(private route: ActivatedRoute) { }

  async ngOnInit() {
    let secretId: string | null = null;
    let privateKey: string | null = null;

    this.isLoading = true;
    this.isSystemError = false;
    this.parameterWarningMesssage = [];
    this.isHidden = true;
    
    this.route.paramMap.subscribe((paramMap: ParamMap) => {
      if(paramMap.has('secretId')){
        secretId = paramMap.get('secretId');
      }
    })
    this.route.queryParams.subscribe(params => {
      privateKey = params['key'];
    });

    await this.getSecret(secretId, privateKey);
  }

  async getSecret(secretId: string|null, privateKey: string|null) {
    if(!secretId || !privateKey) {
      this.isLoading = false;

      if(!secretId) {
        this.parameterWarningMesssage.push("Secret ID is missing");
      }

      if(!privateKey) {
        this.parameterWarningMesssage.push("Private key is missing");
      }

      return;
    }

    try {
      const resp = await axios.get<SecretSubmissionRetrieveResponse>(`${environment.apiUrl}/api/secrets/${secretId}?key=${privateKey}`);
      this.secretMessage = resp.data.message;
    } catch (error: any) {
      const err = error as AxiosError<HttpErrorResponse>;
      const errorResponse = err.response!;

      if (errorResponse.status !== ErrorCodes.NotFound) {
        this.isSystemError = true;
      }
    }
    
    this.isLoading = false;
  }

  copyText() {
    this.selectText();
    document.execCommand('copy');
    this.isCopied = true;
  }

  selectText(){
    const textElement = document.getElementById("secretMessage") as HTMLInputElement;
    textElement!.focus();
    textElement!.select();
  }

  toggleSecretVisibility() {
    this.isHidden = !this.isHidden;
  }
}
