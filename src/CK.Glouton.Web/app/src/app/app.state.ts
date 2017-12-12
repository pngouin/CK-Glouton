import { ITimeSpanNavigatorState } from './modules/timeSpanNavigator/state/timeSpanNavigator.state';
import { ILucenePreferenceState } from './modules/lucene/state/lucenePreference.state';

export interface IAppState {
    timeSpanNavigator: ITimeSpanNavigatorState;
    luceneParameters: ILucenePreferenceState;
}