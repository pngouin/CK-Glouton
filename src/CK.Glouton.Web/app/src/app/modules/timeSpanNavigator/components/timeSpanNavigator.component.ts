import { Component, Input, OnInit } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { IAppState } from 'app/app.state';
import { ITimeSpanNavigator, ITimeSpanNavigatorSettings, Scale, IScaleEdge, IEdge, SliderSide } from '../models';
import { SubmitTimeSpanEffect } from '../actions';
import { Initializer } from "./initializer";

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
        if(event.values[0] < 5 || event.values[1] > 95) {state |= TimeSpanNavigatorState.Dezoom;}
        if(event.values[1] - event.values[0] < 10) {state |= TimeSpanNavigatorState.Zoom;}
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
        const width: number = event.values[1] - event.values[0];
        if(width < 10) {
            if(this._currentScale !== Scale.Seconds) {this.updateScale(this._currentScale + 1);}
        } else if(event.values[0] < 5 || event.values[1] > 95) {
            if(this._currentScale !== Scale.Year) {this.updateScale(this._currentScale - 1);}
        } else {
            const offset: number = (100 - width) / 2;
            this._range = [offset, offset + width];
        }
        this._dateRange[0] = this.updateDate(this._dateRange[0], event.values[0], this._currentScale, SliderSide.Left);
        this._dateRange[1] = this.updateDate(this._dateRange[1], event.values[1], this._currentScale, SliderSide.Right);
        console.log(this._dateRange);
    }

    private updateDate(date: Date, percent: number, scale: Scale, sliderSide : SliderSide) : Date {
        let value : number = this.getScaleDateValue(date, scale) + (this.getEdges(scale).max * percent);
        if(value > this.getScaleDateValue(date, scale) && sliderSide == SliderSide.Right)
            value *= -1;

        return this.setDateScaleValue(date, value, scale);
    }

    private getScaleDateValue (date: Date, scale: Scale) : number {
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

    private setDateScaleValue(date: Date, value: number, scale: Scale) : Date {
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