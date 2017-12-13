import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { IAppState } from 'app/app.state';
import { LogService } from 'app/_services';
import { ILogView } from 'app/common/logs/models';
import { SubmitAppNamesEffect } from '../actions';

@Component({
    selector: 'applicationNameSelector',
    templateUrl: 'applicationNameSelector.component.html'
})

export class ApplicationNameSelectorComponent implements OnInit {

    private _applicationNames$: Observable<string[]>;
    private _applicationNames: string[];

    private _selected: string[];
    private _subscription: Subscription;

    constructor(
        private logService: LogService,
        private effectDispatcher: EffectDispatcher,
        private store: Store<IAppState>
    ) {
        this._applicationNames$ = this.store.select(s => s.luceneParameters.appNames);
        this._subscription = this._applicationNames$.subscribe(a => this._selected = a);
    }

    onChange(_: boolean): void {
        this.effectDispatcher.dispatch(new SubmitAppNamesEffect(this._selected));
    }

    ngOnInit(): void {
        this.logService.getAllApplicationName()
            .subscribe(n => this._applicationNames = n);
     }
}