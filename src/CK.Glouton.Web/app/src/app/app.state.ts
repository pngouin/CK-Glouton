import { ITimeSpanNavigatorState } from './modules/timeSpanNavigator/state/timeSpanNavigator.state';
import { ILogsPreferenceState } from './modules/logs/state/logsPreference.state';

export interface IAppState {
    timeSpanNavigator: ITimeSpanNavigatorState;
    logsParameters: ILogsPreferenceState;
}