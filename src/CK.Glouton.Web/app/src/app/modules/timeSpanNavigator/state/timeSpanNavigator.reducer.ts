import * as actions from '../actions';
import { reducerFactory } from '@ck/rx';
import { ITimeSpanNavigatorState } from './timeSpanNavigator.state';

const initial: ITimeSpanNavigatorState = {
    from: new Date('1900-00-00'),
    to: new Date()
};

export default reducerFactory(Object.values(actions), initial);
