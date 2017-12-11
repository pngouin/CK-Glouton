import { Action, Store } from '@ngrx/store';
import { ActionHandler, EffectHandler, IActionHandler, IEffectsHandler } from '@ck/rx';
import { ECriticityLevel } from 'app/modules/lucene/models';
import { ILuceneState } from '../state/lucene.state';
import { IAppState } from 'app/app.state';

export class SubmitCriticityEffect implements Action {
    public type = '[EFFECT](LuceneParameters) => Submit criticity';
    constructor(
        public payload: ECriticityLevel
    ) {
    }
}
@EffectHandler(SubmitCriticityEffect)
export class SubmitCriticityEffectHandler implements IEffectsHandler {
    constructor(
        private store: Store<IAppState>
    ) {
        console.log('hi');
    }

    public execute(action: SubmitCriticityEffect): SetCriticityMutation | void {
        let criticityLevel: ECriticityLevel = action.payload;

        if(criticityLevel === null || criticityLevel === undefined) { throw new Error('Param cannot be null!'); }

        let sCriticityLevel: ECriticityLevel;
        this.store.select(s => s.luceneParameters.level).subscribe(l => criticityLevel = l);

        if(sCriticityLevel !== criticityLevel) { return new SetCriticityMutation(criticityLevel); }
    }
}

export class SetCriticityMutation implements Action {
    public type = '[MUTATION](LuceneParameters) => Set criticity';
    constructor (
        public payload: ECriticityLevel
    ) {
    }
}
@ActionHandler(SetCriticityMutation)
export class SetCriticityMutationHandler implements IActionHandler {
    public apply(state: ILuceneState, action: SetCriticityMutation): ILuceneState {
        return {
            ...state,
            level: action.payload
        };
    }
}