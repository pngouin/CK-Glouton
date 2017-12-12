import { Action, Store } from '@ngrx/store';
import { ActionHandler, EffectHandler, IActionHandler, IEffectsHandler } from '@ck/rx';
import { ECriticityLevel } from 'app/modules/lucene/models';
import { ILucenePreferenceState } from '../state/lucenePreference.state';
import { IAppState } from 'app/app.state';