import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { IAppState } from 'app/app.state';
import { SubmitCriticityEffect } from '../actions';

@Component({
    selector: 'criticitySelector',
    templateUrl: 'criticitySelector.component.html'
})


export class CriticitySelectorComponent {

    private _criticityLevel$: Observable<string[]>;
    private _criticityLevel: string[];
    private _subscription: Subscription;

    constructor(
        private store: Store<IAppState>,
        private effectDispatcher: EffectDispatcher
    ) {
        this._criticityLevel$ = this.store.select(s => s.luceneParameters.level);
        this._subscription = this._criticityLevel$.subscribe(l => this._criticityLevel = l);
    }

    onChange( _: boolean ): void {
        this.effectDispatcher.dispatch(new SubmitCriticityEffect(this._criticityLevel));
    }
}