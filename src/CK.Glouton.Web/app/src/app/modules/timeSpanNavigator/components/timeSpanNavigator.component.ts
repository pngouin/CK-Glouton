import { Component, Input, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { IAppState } from 'app/app.state';
import { ITimeSpanNavigator, ITimeSpanNavigatorSettings } from '../models';
import { SubmitTimeSpanEffect } from '../actions';

@Component({
    selector: 'timeSpanNavigator',
    templateUrl: './timeSpanNavigator.component.html'
})
export class TimeSpanNavigatorComponent implements OnInit {

    private _from$: Observable<Date>;
    private _to$: Observable<Date>;
    private _subscriptions: Subscription[];

    private _range: number[];
    private _currentScale: string;

    get timeSpan(): ITimeSpanNavigator { return this._timeSpan.getValue(); }
    private _timeSpan = new BehaviorSubject<ITimeSpanNavigator>({from: null, to: null});

    @Input()
    configuration: ITimeSpanNavigatorSettings;

    constructor(
        private store: Store<IAppState>,
        private effectDispatcher: EffectDispatcher
    ) {
        this._from$ = store.select(s => s.timeSpanNavigator.from);
        this._to$ = store.select(s => s.timeSpanNavigator.to);
        this._subscriptions = [];
        this._subscriptions.push(this._from$.subscribe(d => this._timeSpan.next({from: d, to: this.timeSpan.to})));
        this._subscriptions.push(this._to$.subscribe(d => this._timeSpan.next({from: this.timeSpan.from, to: d})));
    }

    private validateArgument(argument: any): argument is ITimeSpanNavigatorSettings {
        if(argument === undefined || argument === null) {return false;}
        return (argument as ITimeSpanNavigatorSettings).scales !== undefined;
    }

    ngOnInit(): void {
        if(!this.validateArgument(this.configuration)) {throw new Error('Configuration is invalid!');}
        this._currentScale = this.configuration.default.scale;
        this._timeSpan.next({from: new Date(), to: new Date()});
        this._range = [10, 20];
    }
}