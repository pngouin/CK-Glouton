import * as actions from '../actions';
import { reducerFactory } from '@ck/rx';
import { IDateRangePickerState } from './dateRangePicker.state';

const initial: IDateRangePickerState = {
    dateRange: {
        from: null,
        to: null
    }
};

export default reducerFactory(Object.values(actions), initial);
