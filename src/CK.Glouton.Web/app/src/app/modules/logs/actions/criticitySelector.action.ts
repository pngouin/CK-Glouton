import { Action, Store } from '@ngrx/store';
import { ActionHandler, EffectHandler, IActionHandler, IEffectsHandler } from '@ck/rx';
import { ILogsPreferenceState } from '../state/logsPreference.state';
import { IAppState } from 'app/app.state';

export class SubmitCriticityEffect implements Action {
    public type = '[EFFECT](LuceneParameters) => Submit criticity';
    constructor(
        public payload: string[]
    ) {
    }
}
@EffectHandler(SubmitCriticityEffect)
export class SubmitCriticityEffectHandler implements IEffectsHandler {
    constructor(
        private store: Store<IAppState>
    ) {
    }

    public execute(action: SubmitCriticityEffect): SetCriticityMutation | void {
        let criticityLevel: string[] = action.payload;

        if(criticityLevel === null || criticityLevel === undefined) { throw new Error('Param cannot be null!'); }

        let sCriticityLevel: string[];
        this.store.select(s => s.logsParameters.level).subscribe(l => sCriticityLevel = l);

        if(sCriticityLevel !== criticityLevel) { return new SetCriticityMutation(criticityLevel); }
    }
}

export class SetCriticityMutation implements Action {
    public type = '[MUTATION](LuceneParameters) => Set criticity';
    constructor (
        public payload: string[]
    ) {
    }
}
@ActionHandler(SetCriticityMutation)
export class SetCriticityMutationHandler implements IActionHandler {
    public apply(state: ILogsPreferenceState, action: SetCriticityMutation): ILogsPreferenceState {
        return {
            ...state,
            level: action.payload
        };
    }
}