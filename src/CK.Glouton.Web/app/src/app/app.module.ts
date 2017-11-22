import 'reflect-metadata';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { rootRouterConfig } from './app.routes';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { restoreStore, SignatureEffectsModule, EffectDispatcher } from '@ck/rx';
import { EffectsModule } from '@ngrx/effects';
import { StoreModule } from '@ngrx/store';
import { reducer } from './app.reducer';
import { StoreDevtoolsModule } from '@ngrx/store-devtools';

import * as components from './_components';
import * as modules from './_modules';
import * as services from './_services';
// import * as guards from './_guards';

import * as timeSpanNavigatorRx from './modules/timeSpanNavigator/actions';

const stateStorageKey: string = 'hln_glouton/state';

@NgModule({
  declarations: [
    AppComponent,
    ...Object.values(components)
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    ...Object.values(modules),
    StoreModule.provideStore(reducer),
    RouterModule.forRoot(rootRouterConfig),
    SignatureEffectsModule.runAfterBootstrap({
      handlers: [
        ...Object.values(timeSpanNavigatorRx)
      ],
      storage: {key: stateStorageKey}
    }),
    StoreDevtoolsModule.instrumentOnlyWithExtension({maxAge: 5})
  ],
  providers: [
    EffectDispatcher, // ,
    // ...Object.values(services),
    // ...Object.values(guards),
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule {

}
