import { Action, Store } from '@ngrx/store';
import { ActionHandler, EffectHandler, IActionHandler, IEffectsHandler } from '@ck/rx';
import { IDateRangePickerState } from '../state/dateRangePicker.state';
import { IAppState } from 'app/app.state';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { IDateRangePicker } from '../models/dateRangePicker.model';

export class SubmitDateRangeEffect implements Action {
    public type = '[EFFECT](DateRange) => Submit date';
    constructor(
        public payload: IDateRangePicker
    ) {
    }
}
@EffectHandler(SubmitDateRangeEffect)
export class SumbitDateRangeEffectHandler implements IEffectsHandler {
    constructor(
        private store: Store<IAppState>
    ) {
    }

    public execute(action: SubmitDateRangeEffect): SetDateRangeMutation | SetDateFromMutation | SetDateToMutation | void {
        let from: Date = action.payload.from;
        let to: Date = action.payload.to;

        if( from === undefined || from === null || to === undefined || to === null ) {
            throw new Error('Params cannot be null!');
        } else if( from.getTime() > to.getTime() ) {
            throw new Error('From needs to be lower than to!');
        }

        let sFrom: Date;
        let sTo: Date;
        this.store.select(s => s.dateRangePicker.from).subscribe(d => sFrom = d);
        this.store.select(s => s.dateRangePicker.to).subscribe(d => sTo = d);
        let index: number = 0;
        if(sFrom === null || from.getTime() !== sFrom.getTime()) {index += 1;}
        if(sTo === null || to.getTime() !== sTo.getTime()) {index+=2;}

        switch(index) {
            case 0: return;
            case 1: return new SetDateFromMutation(from);
            case 2: return new SetDateToMutation(to);
            case 3: return new SetDateRangeMutation({from,to});
            default: throw new Error('Index is invalid!');
        }
    }
}

export class SetDateRangeMutation implements Action {
    public type = '[MUTATION](DateRange) => Set date range.';
    constructor(
        public payload: IDateRangePicker
    ) {
    }
}
@ActionHandler(SetDateRangeMutation)
export class SetDateRangeMutationHandler implements IActionHandler {
    public apply(state: IDateRangePickerState, action: SetDateRangeMutation): IDateRangePickerState {
        return {
            ...state,
            from: action.payload.from,
            to: action.payload.to
        };
    }
}

export class SetDateFromMutation implements Action {
    public type = '[MUTATION](DateRange) => Set date from.';
    constructor(
        public payload: Date
    ) {
    }
}
@ActionHandler(SetDateFromMutation)
export class SetDateFromMutationHandler implements IActionHandler {
    public apply(state: IDateRangePickerState, action: SetDateFromMutation): IDateRangePickerState {
        return {
            ...state,
            from: action.payload
        };
    }
}

export class SetDateToMutation implements Action {
    public type = '[MUTATION](DateRange) => Set date to.';
    constructor(
        public payload: Date
    ) {
    }
}
@ActionHandler(SetDateToMutation)
export class SetDateToMutationHandler implements IActionHandler {
    public apply(state: IDateRangePickerState, action: SetDateToMutation): IDateRangePickerState {
        return {
            ...state,
            to: action.payload
        };
    }
}