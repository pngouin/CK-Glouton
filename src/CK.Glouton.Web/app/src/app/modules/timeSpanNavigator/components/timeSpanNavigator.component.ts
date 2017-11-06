import { Component, Input, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { IAppState } from 'app/app.state';
import { ITimeSpanNavigator, ITimeSpanNavigatorSettings, Scale } from '../models';
import { SubmitTimeSpanEffect } from '../actions';

@Component({
    selector: 'timeSpanNavigator',
    templateUrl: './timeSpanNavigator.component.html'
})
export class TimeSpanNavigatorComponent implements OnInit {

    private _from$: Observable<Date>;
    private _to$: Observable<Date>;
    private _subscriptions: Subscription[];

    private _currentScale: Scale;
    private _range: number[];
    private _dateRange: Date[];
    private _scaleDescription: string;

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
        return (argument as ITimeSpanNavigatorSettings).from !== undefined
            && (argument as ITimeSpanNavigatorSettings).to !== undefined
            && (argument as ITimeSpanNavigatorSettings).scale !== undefined;
    }

    private setRange(): number[] {
        return [20, 80];
    }

    /**
     * onChange function
     * @param event The event:
     * e.originalEvent: Slide event
     * e.value: New value
     * e.values: Values in range mode
     */
    private handleChange(event: any): void {
        console.log(`Change: ${event}`);
    }

    /**
     * onSlideEnd function
     * @param event The event:
     * event.originalEvent: Mouseup event
     * event.value: New value
     */
    private handleSlideEnd(event: any): void {
        const width: number = this._range[1] - this._range[0];
        const offset: number = (100 - width) / 2;
        this._range = [offset, offset + width];
    }

    ngOnInit(): void {
        if(!this.validateArgument(this.configuration)) {throw new Error('Configuration is invalid!');}
        this._timeSpan.next({from: new Date(), to: new Date()});
        this._dateRange = [this.configuration.from, this.configuration.to];
        this._currentScale = this.configuration.scale;
        this._scaleDescription = `Current scale: ${Scale[this._currentScale]}`;

        // TODO: Calculate intial scale
        this._range = [13, 87];
    }
}