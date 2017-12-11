import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { IAppState } from 'app/app.state';
import { ECriticityLevel } from '../models';
import { SubmitCriticityEffect } from '../actions';

@Component({
    selector: 'criticitySelector',
    templateUrl: 'criticitySelector.component.html'
})


export class CriticitySelectorComponent {

    private _criticityLevel$: Observable<ECriticityLevel>;
    private _criticityLevel: ECriticityLevel;
    private _subscription: Subscription;

    private criticityLevel: string[] = [];

    constructor(
        private store: Store<IAppState>,
        private effectDispatcher: EffectDispatcher
    ) {
        this._criticityLevel$ = this.store.select(s => s.luceneParameters.level);
        this._subscription = this._criticityLevel$.subscribe(l => this._criticityLevel = l);
    }

    onChange( _: boolean ): void {
        this._criticityLevel = 0;
        // tslint:disable:one-line
        this.criticityLevel.forEach(level => {
            if( level === 'debug' ) { this._criticityLevel |= ECriticityLevel.debug;}
            else if( level === 'trace' ) { this._criticityLevel |= ECriticityLevel.trace; }
            else if( level === 'info' ) { this._criticityLevel |= ECriticityLevel.info; }
            else if( level === 'warn' ) { this._criticityLevel |= ECriticityLevel.warn; }
            else if( level === 'error' ) { this._criticityLevel |= ECriticityLevel.error; }
            else if( level === 'fatal' ) { this._criticityLevel |= ECriticityLevel.fatal; }
        });
        this.effectDispatcher.dispatch(new SubmitCriticityEffect(this._criticityLevel));
    }
}

// getArray() :string[]{
//     let array = new Array();
//     if (this.debug) array.push("debug");
//     if (this.trace) array.push("trace");
//     if (this.info) array.push("info");
//     if (this.warn) array.push("warn");
//     if (this.error) array.push("error");
//     if (this.fatal) array.push("fatal");
//     return array;
// }