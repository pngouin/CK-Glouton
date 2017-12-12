import { Action, Store } from '@ngrx/store';
import { ActionHandler, EffectHandler, IActionHandler, IEffectsHandler } from '@ck/rx';
import { ECriticityLevel } from 'app/modules/lucene/models';
import { ILucenePreferenceState } from '../state/lucenePreference.state';
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
        this.store.select(s => s.luceneParameters.appNames).subscribe(a => sAppNames = a);

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
@EffectHandler(SetAppNamesMutation)
export class SetAppNamesMutationHandler implements IActionHandler {
    public apply(state: ILucenePreferenceState, action: SetAppNamesMutation): ILucenePreferenceState {
        return {
            ...state,
            appNames: action.payload
        };
    }
}