import { Component } from '@angular/core';
import { FormControl } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { TimeOption } from './models/TimeOption';
import { environment } from 'src/environments/environment';
import { SecretSubmissionRequest } from './models/SecretSubmissionRequest';
import { SecretSubmissionResponse } from './models/SecretSubmissionResponse';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  constructor(){
    
  }
  ngOnInit(){

  }
}
