import { Action, Store } from '@ngrx/store';
import { ActionHandler, EffectHandler, IActionHandler, IEffectsHandler } from '@ck/rx';
import { ITimeSpanNavigatorState } from '../state/timeSpanNavigator.state';
import { IAppState } from 'app/app.state';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { ITimeSpanNavigator } from '../models/timeSpanNavigator.model';

export class SubmitTimeSpanEffect implements Action {
    public type = '[EFFECT](TimeSpan) => Submit date';
    constructor(
        public payload: ITimeSpanNavigator
    ) {
    }
}
@EffectHandler(SubmitTimeSpanEffect)
export class SubmitTimeSpanEffectHandler implements IEffectsHandler {
    constructor(
        private store: Store<IAppState>
    ) {
    }

    public execute(action: SubmitTimeSpanEffect): SetTimeSpanMutation | SetTimeFromMutation | SetTimeToMutation | void {
        let from: Date = action.payload.from;
        let to: Date = action.payload.to;

        if( from === undefined || from === null || to === undefined || to === null ) {
            throw new Error('Params cannot be null!');
        } else if( from.getTime() > to.getTime() ) {
            throw new Error('From date needs to be lower than To date!');
        }

        let sFrom: Date;
        let sTo: Date;
        this.store.select(s => s.timeSpanNavigator.from).subscribe(d => sFrom = d);
        this.store.select(s => s.timeSpanNavigator.to).subscribe(d => sTo = d);
        let index: number = 0;
        if(sFrom === null || from.getTime() !== sFrom.getTime()) {index += 1;}
        if(sTo === null || to.getTime() !== sTo.getTime()) {index+=2;}

        switch(index) {
            case 0: return;
            case 1: return new SetTimeFromMutation(from);
            case 2: return new SetTimeToMutation(to);
            case 3: return new SetTimeSpanMutation({from,to});
            default: throw new Error('Index is invalid!');
        }
    }
}

export class SetTimeSpanMutation implements Action {
    public type = '[MUTATION](TimeSpan) => Set date range.';
    constructor(
        public payload: ITimeSpanNavigator
    ) {
    }
}
@ActionHandler(SetTimeSpanMutation)
export class SetTimeSpanMutationHandler implements IActionHandler {
    public apply(state: ITimeSpanNavigatorState, action: SetTimeSpanMutation): ITimeSpanNavigatorState {
        return {
            ...state,
            from: action.payload.from,
            to: action.payload.to
        };
    }
}

export class SetTimeFromMutation implements Action {
    public type = '[MUTATION](TimeSpan) => Set date from.';
    constructor(
        public payload: Date
    ) {
    }
}
@ActionHandler(SetTimeFromMutation)
export class SetTimeFromMutationHandler implements IActionHandler {
    public apply(state: ITimeSpanNavigatorState, action: SetTimeFromMutation): ITimeSpanNavigatorState {
        return {
            ...state,
            from: action.payload
        };
    }
}

export class SetTimeToMutation implements Action {
    public type = '[MUTATION](TimeSpan) => Set date to.';
    constructor(
        public payload: Date
    ) {
    }
}
@ActionHandler(SetTimeToMutation)
export class SetTimeToMutationHandler implements IActionHandler {
    public apply(state: ITimeSpanNavigatorState, action: SetTimeToMutation): ITimeSpanNavigatorState {
        return {
            ...state,
            to: action.payload
        };
    }
}