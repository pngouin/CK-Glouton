import { ITimeSpanNavigatorState } from './modules/timeSpanNavigator/state/timeSpanNavigator.state';
import { ILuceneState } from './modules/lucene/state/lucene.state';

export interface IAppState {
    timeSpanNavigator: ITimeSpanNavigatorState;
    luceneParameters: ILuceneState;
}