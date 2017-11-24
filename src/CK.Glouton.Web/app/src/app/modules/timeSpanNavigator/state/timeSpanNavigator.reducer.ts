import * as actions from '../actions';
import { reducerFactory } from '@ck/rx';
import { ITimeSpanNavigatorState } from './timeSpanNavigator.state';

const initial: ITimeSpanNavigatorState = {
    from: null,
    to: null
};

export default reducerFactory(Object.values(actions), initial);
