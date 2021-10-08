import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AboutComponent } from './about/about.component';
import { AppComponent } from './app.component';
import { CreateComponent } from './create/create.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { PrivacyComponent } from './privacy/privacy.component';
import { VaultComponent } from './vault/vault.component';

const routes: Routes = [
  {
    path: '',
    component: CreateComponent
  },
  {
    path: 'vault/:secretId',
    component: VaultComponent
  },
  {
    path:'vault',
    component: VaultComponent
  },
  {
    path:'about',
    component: AboutComponent
  },
  {
    path:'privacy',
    component: PrivacyComponent
  },
  {
    path: '**',
    component: NotFoundComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
