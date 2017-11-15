import { Component, Input, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { IAppState } from 'app/app.state';
import { ITimeSpanNavigator, ITimeSpanNavigatorSettings, Scale, IScaleEdge, IEdge, SliderSide } from '../models';
import { SubmitTimeSpanEffect } from '../actions';
import { Initializer } from './initializer';

@Component({
    selector: 'timeSpanNavigator',
    templateUrl: './timeSpanNavigator.component.html'
})
export class TimeSpanNavigatorComponent implements OnInit {

    private _from$: Observable<Date>;
    private _to$: Observable<Date>;
    private _subscriptions: Subscription[];

    private _currentScale: Scale;
    private _currentScaleWidth: number;

    private _range: number[];
    private _rangeSnapshot: number[];
    private _dateRange: Date[];

    private _scaleDescription: string;
    private _nextScaleDescription: string;

    get timeSpan(): ITimeSpanNavigator { return this._timeSpan.getValue(); }
    private _timeSpan = new BehaviorSubject<ITimeSpanNavigator>({from: null, to: null});

    @Input()
    configuration: ITimeSpanNavigatorSettings;

    @Input()
    edges : IScaleEdge;

    constructor(
        private store: Store<IAppState>,
        private effectDispatcher: EffectDispatcher
    ) {
        this._from$ = store.select(s => s.timeSpanNavigator.from);
        this._to$ = store.select(s => s.timeSpanNavigator.to);
        this._subscriptions = [];
        this._subscriptions.push(this._from$.subscribe(d => this._timeSpan.next({from: d, to: this.timeSpan.to})));
        this._subscriptions.push(this._to$.subscribe(d => this._timeSpan.next({from: this.timeSpan.from, to: d})));
        this._scaleDescription = '';
        this._nextScaleDescription = '';
    }

    /**
     * Returns the falling and rising edge for the given scale.
     * Throws an error if the given state is invalid.
     * @param scale The scale we want to get falling and rising edges.
     */
    private getEdges(scale: Scale): IEdge {
        switch(scale) {
            case Scale.Year: return this.edges.Years;
            case Scale.Months: return this.edges.Months;
            case Scale.Days: return this.edges.Days;
            case Scale.Hours: return this.edges.Hours;
            case Scale.Minutes: return this.edges.Minutes;
            case Scale.Seconds: return this.edges.Seconds;
            default: throw new Error('Invalid parameter( scale )');
        }
    }

    private getScaleItemPercent(scale: Scale): number {
        return 100 / this.getEdges(scale).max * this.getEdges(scale).min;
    }

    private updateScale(scale: Scale): void {
        if(!(scale in Scale)) {throw new Error('Argument error( scale )');}
        this._currentScale = scale;
        this._currentScaleWidth = this.getScaleItemPercent(this._currentScale);
        this._scaleDescription = `Current scale: ${Scale[this._currentScale]}`;
        this._nextScaleDescription = '';
        this._range = [25, 75]; // Todo: Change me
        this._rangeSnapshot = this._range.slice();
    }

    /**
     * onChange function
     * @param event The event:
     * e.originalEvent: Slide event
     * e.value: New value
     * e.values: Values in range mode
     */
    private handleChange(event: any): void {
        let state: TimeSpanNavigatorState = TimeSpanNavigatorState.None;
        if(event.values[SliderSide.Left] <= 0 || event.values[SliderSide.Right] >= 100) {state |= TimeSpanNavigatorState.Dezoom;}
        if(event.values[SliderSide.Right] - event.values[SliderSide.Left] < 10) {state |= TimeSpanNavigatorState.Zoom;}
        if(!(this._nextScaleDescription.length === 0)) {state |= TimeSpanNavigatorState.Flagged;}

        switch(state) {
            case TimeSpanNavigatorState.Dezoom:
                if(this._currentScale !== Scale.Year) {
                    this._nextScaleDescription = ` -> ${Scale[this._currentScale - 1]}`;
                    this._scaleDescription += this._nextScaleDescription;
                }
                break;

            case TimeSpanNavigatorState.Zoom:
                if(this._currentScale !== Scale.Seconds) {
                    this._nextScaleDescription = ` -> ${Scale[this._currentScale + 1]}`;
                    this._scaleDescription += this._nextScaleDescription;
                }
                break;

            case TimeSpanNavigatorState.Flagged:
                this._scaleDescription = this._scaleDescription.substring(0,
                    (<string>this._scaleDescription).length - (<string>this._nextScaleDescription).length
                );
                this._nextScaleDescription = '';
                break;

            case TimeSpanNavigatorState.None:
            case TimeSpanNavigatorState.Flagged | TimeSpanNavigatorState.Dezoom:
            case TimeSpanNavigatorState.Flagged | TimeSpanNavigatorState.Zoom:
                break;

            default:
                throw new Error(`State invalid.`);
        }
    }

    /**
     * onSlideEnd function
     * @param event The event:
     * event.originalEvent: Mouseup event
     * event.value: New value
     */
    private handleSlideEnd(event: any): void {
        const width: number = event.values[SliderSide.Right] - event.values[SliderSide.Left];
        if(width < 10) {
            if(this._currentScale !== Scale.Seconds) {this.updateScale(this._currentScale + 1);}
        } else if(event.values[0] <= 0 || event.values[1] >= 100) {
            if(this._currentScale !== Scale.Year) {this.updateScale(this._currentScale - 1);}
        } else {
            let difference: number = this._rangeSnapshot[SliderSide.Left] - event.values[SliderSide.Left];
            const updatedSlider : SliderSide = difference !== 0 ? SliderSide.Left : SliderSide.Right;
            if(updatedSlider === SliderSide.Right) {difference = this._rangeSnapshot[SliderSide.Right] - event.values[SliderSide.Right];}
            // Todo: Cache me <3 ! (Cache only max, not all the expression)
            let actualDifference: number =
                (this.getEdges(this._currentScale).max - this.getEdges(this._currentScale).min) / Math.abs(difference);
            actualDifference *= Math.pow(-1, difference < 0 ? 0 : 1);
            console.log(this._dateRange[SliderSide.Left]);
            console.log(this._dateRange[SliderSide.Right]);
            if(updatedSlider === SliderSide.Left) {
                this._dateRange[SliderSide.Left] =
                this.setDateScaleValue(this._dateRange[SliderSide.Left], actualDifference, this._currentScale);
            } else {
                this._dateRange[SliderSide.Right] =
                this.setDateScaleValue(this._dateRange[SliderSide.Right], actualDifference, this._currentScale);
            }
            console.log(this._dateRange[SliderSide.Left]);
            console.log(this._dateRange[SliderSide.Right]);

            this._rangeSnapshot = event.values.slice();
        }
    }

    private getScaleDateValue (date: Date, scale: Scale): number {
        switch(scale) {
            case Scale.Year: return date.getFullYear();
            case Scale.Months: return date.getMonth();
            case Scale.Days: return date.getDay();
            case Scale.Hours: return date.getHours();
            case Scale.Minutes: return date.getMinutes();
            case Scale.Seconds: return date.getSeconds();
            default: throw new Error('Invalid parameter( scale )');
        }
    }

    private setDateScaleValue(date: Date, value: number, scale: Scale): Date {
        switch(scale) {
            case Scale.Year: date.setFullYear(value); break;
            case Scale.Months: date.setMonth(value); break;
            case Scale.Days: date.setDate(value); break;
            case Scale.Hours: date.setHours(value); break;
            case Scale.Minutes: date.setMinutes(value); break;
            case Scale.Seconds: date.setSeconds(value); break;
            default: throw new Error('Invalid parameter( scale )');
        }
        return date;
    }

    /**
     * Initilization method.
     */
    ngOnInit(): void {
        if(!Initializer.validateArgument(this.configuration)) {throw new Error('Configuration is invalid!');}
        this._timeSpan.next({from: new Date(), to: new Date()});
        this._dateRange = [this.configuration.from, this.configuration.to];
        this.updateScale(this.configuration.scale);
    }
}

enum TimeSpanNavigatorState {
    None = 0,
    Dezoom = 1 << 0,
    Zoom = 1 << 1,
    Flagged = 1 << 2
}