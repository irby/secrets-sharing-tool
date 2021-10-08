import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AngularMaterialModule } from './angular-material';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { VaultComponent } from './vault/vault.component';
import { CreateComponent } from './create/create.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { HeaderComponent } from './header/header.component';
import { AboutComponent } from './about/about.component';
import { PrivacyComponent } from './privacy/privacy.component';

@NgModule({
  declarations: [							
    AppComponent,
      VaultComponent,
      CreateComponent,
      NotFoundComponent,
      HeaderComponent,
      AboutComponent,
      PrivacyComponent
   ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AngularMaterialModule,
    ReactiveFormsModule,
    AppRoutingModule,
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
