import { Component, Input, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { IDateRangePicker } from '../models/dateRangePicker.model';
import { Store } from '@ngrx/store';
import { IAppState } from 'app/app.state';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { SubmitDateRangeEffect } from '../actions';
import { EffectDispatcher } from '@ck/rx';

@Component({
    selector: 'dateRangePicker',
    templateUrl: './dateRangePicker.component.html'
})
export class DateRangePickerComponent implements OnInit {

    get dateRange(): IDateRangePicker { return this._dateRange.getValue(); }
    private _dateRange = new BehaviorSubject<IDateRangePicker>({
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
        this._from$ = store.select(s => s.dateRangePicker.from);
        this._to$ = store.select(s => s.dateRangePicker.to);
        this._subscriptions = [];
        this._subscriptions.push(this._from$.subscribe(d => this._from = d));
        this._subscriptions.push(this._to$.subscribe(d => this._to = d));
    }

    private onClick(): void {
        this.effectDispatcher.dispatch( new SubmitDateRangeEffect( { from: new Date( '1900-01-01' ), to: new Date() } ) );
    }

    ngOnInit(): void {
        // if(this.configurationFilePath === null) {
        //     throw new Error('Configuration file not set');
        // }
    }
}