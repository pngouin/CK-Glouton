import * as actions from '../actions';
import { reducerFactory } from '@ck/rx';
import { ILogsPreferenceState } from './logsPreference.state';

const initial: ILogsPreferenceState = {
    level: [],
    appNames: []
};

export default reducerFactory(Object.values(actions), initial);