import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { environment } from 'src/environments/environment';

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

  constructor(private route: ActivatedRoute, private http: HttpClient) { }

  async ngOnInit() {
    let secretId: string | null = null;
    let privateKey: string | null = null;

    this.isLoading = true;
    this.isSystemError = false;
    this.parameterWarningMesssage = [];
    
    this.route.paramMap.subscribe((paramMap: ParamMap) => {
      if(paramMap.has('secretId')){
        secretId = paramMap.get('secretId');
      }
    })
    this.route.queryParams.subscribe(params => {
      privateKey = params['key'];
    });

    await this.getSecret(secretId!, privateKey!);
  }

  async getSecret(secretId: string, privateKey: string) {
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
    
    await this.http.get(`${environment.apiUrl}/api/secrets/${secretId}?key=${privateKey}`).subscribe(data => {
      this.secretMessage = (data as any).message;
      this.isLoading = false;
    }, err => {
      if(err.status !== 400) {
        this.isSystemError = true;
      }
      this.isLoading = false;
    });
    
  }

  copyText() {
    this.selectText();
    document.execCommand('copy');
    this.isCopied = true;
  }

  selectText(){
    const textElement = document.getElementById("secretMessage") as HTMLInputElement;
    textElement?.focus();
    textElement?.select();
  }

}
