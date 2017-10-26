import { Action, Store } from '@ngrx/store';
import { ActionHandler, EffectHandler, IActionHandler, IEffectsHandler } from '@ck/rx';
import { IDateRangePickerState } from '../state/dateRangePicker.state';
import { IAppState } from 'app/app.state';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { IDateRangePicker } from '../models/dateRangePicker.model';

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
            dateRange: action.payload
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
            dateRange: {
                ...state.dateRange,
                from: action.payload
            }
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
            dateRange: {
                ...state.dateRange,
                to: action.payload
            }
        };
    }
}