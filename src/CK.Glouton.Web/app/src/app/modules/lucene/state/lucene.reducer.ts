import * as actions from '../actions';
import { reducerFactory } from '@ck/rx';
import { ILuceneState } from './lucene.state';

const initial: ILuceneState = {
    level: 0
};

export default reducerFactory(Object.values(actions), initial);