import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { CreateComponent } from './create/create.component';
import { VaultComponent } from './vault/vault.component';

const routes: Routes = [
  {
    path: '',
    component: CreateComponent
  },
  {
    path: 'vault/:secretId',
    component: VaultComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
