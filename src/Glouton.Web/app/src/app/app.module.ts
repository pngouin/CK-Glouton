import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { rootRouterConfig } from './app.routes';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';

import * as pages from './pages';

@NgModule({
  declarations: [
    AppComponent,
    ...Object.values(pages)
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot(rootRouterConfig, { useHash: true })
  ],
  providers: [
  ],
  bootstrap: [ AppComponent ]
})
export class AppModule {

}
