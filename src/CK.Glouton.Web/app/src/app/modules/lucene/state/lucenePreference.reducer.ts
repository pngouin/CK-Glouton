import * as actions from '../actions';
import { reducerFactory } from '@ck/rx';
import { ILucenePreferenceState } from './lucenePreference.state';

const initial: ILucenePreferenceState = {
    level: 0,
    appNames: []
};

export default reducerFactory(Object.values(actions), initial);