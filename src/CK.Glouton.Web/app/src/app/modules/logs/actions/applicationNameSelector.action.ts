import { Action, Store } from '@ngrx/store';
import { ActionHandler, EffectHandler, IActionHandler, IEffectsHandler } from '@ck/rx';
import { ILogsPreferenceState } from '../state/logsPreference.state';
import { IAppState } from 'app/app.state';

export class SubmitAppNamesEffect implements Action {
    public type = '[EFFECT](LucenePreferences) => Submit appNames';
    constructor(
        public payload: string[]
    ) {
    }
}
@EffectHandler(SubmitAppNamesEffect)
export class SubmitAppNamesEffectHandler implements IEffectsHandler {
    constructor(
        private store: Store<IAppState>
    ) {
    }

    public execute(action: SubmitAppNamesEffect): SetAppNamesMutation | void {
        let appNames: string[] = action.payload;

        if(appNames === null || appNames === undefined) { throw new Error('Params cannot be null!'); }

        let sAppNames: string[];
        this.store.select(s => s.logsParameters.appNames).subscribe(a => sAppNames = a);

        if(appNames !== sAppNames) { return new SetAppNamesMutation(appNames); }
    }
}

export class SetAppNamesMutation implements Action {
    public type = '[MUTATION](LuceneParameters) => Set appNames';
    constructor(
        public payload: string[]
    ) {
    }
}
@ActionHandler(SetAppNamesMutation)
export class SetAppNamesMutationHandler implements IActionHandler {
    public apply(state: ILogsPreferenceState, action: SetAppNamesMutation): ILogsPreferenceState {
        return {
            ...state,
            appNames: action.payload
        };
    }
}