import { Component, Input, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { ITimeSpanNavigator } from '../models/timeSpanNavigator.model';
import { Store } from '@ngrx/store';
import { IAppState } from 'app/app.state';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { SubmitTimeSpanEffect } from '../actions';
import { EffectDispatcher } from '@ck/rx';

@Component({
    selector: 'timeSpanNavigator',
    templateUrl: './timeSpanNavigator.component.html'
})
export class TimeSpanNavigatorComponent implements OnInit {

    get timeSpan(): ITimeSpanNavigator { return this._timeSpan.getValue(); }
    private _timeSpan = new BehaviorSubject<ITimeSpanNavigator>({
            from: null,
            to: null
    });

    private _from$: Observable<Date>;
    private _to$: Observable<Date>;
    private _subscriptions: Subscription[];

    private _from: Date;
    private _to: Date;

    @Input()
    configurationFilePath: string;

    constructor(
        private store: Store<IAppState>,
        private effectDispatcher: EffectDispatcher
    ) {
        this._from$ = store.select(s => s.timeSpanNavigator.from);
        this._to$ = store.select(s => s.timeSpanNavigator.to);
        this._subscriptions = [];
        this._subscriptions.push(this._from$.subscribe(d => this._from = d));
        this._subscriptions.push(this._to$.subscribe(d => this._to = d));
    }

    private onClick(): void {
        this.effectDispatcher.dispatch( new SubmitTimeSpanEffect( { from: new Date( '1900-01-01' ), to: new Date() } ) );
    }

    ngOnInit(): void {
        // if(this.configurationFilePath === null) {
        //     throw new Error('Configuration file not set');
        // }
    }
}