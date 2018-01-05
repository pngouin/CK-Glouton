import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { ITimeSpanNavigatorState } from 'app/modules/timeSpanNavigator/state/timeSpanNavigator.state';

@Injectable()
export class LuceneParametersSnapshotService {

    private _appNames: string[];
    get appNames(): string[] { return this._appNames; }
    set appNames(appNames: string[]) { this._appNames = appNames; }

    private _level: string[];
    get level(): string[] { return this._level; }
    set level(level: string[]) { this._level = level; }

    private _dateRange: ITimeSpanNavigatorState;
    get dateRange(): ITimeSpanNavigatorState { return this._dateRange; }
    set dateRange(dateRange: ITimeSpanNavigatorState) { this._dateRange = dateRange; }

    private _keyword: string;
    get keyword(): string { return this._keyword; }
    set keyword(keyword: string) { this._keyword = keyword; }
}