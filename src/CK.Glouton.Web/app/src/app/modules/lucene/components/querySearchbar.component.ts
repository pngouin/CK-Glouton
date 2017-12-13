import { Component, OnInit } from '@angular/core';
import { LogService } from 'app/_services';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { EffectDispatcher } from '@ck/rx';
import { IAppState } from 'app/app.state';
import { ECriticityLevel } from '../models';
import { ITimeSpanNavigatorState } from 'app/modules/timeSpanNavigator/state/timeSpanNavigator.state';
import { ILogView } from 'app/common/logs/models';

@Component({
    selector: 'querySearchbar',
    templateUrl: 'querySearchbar.component.html'
})
export class QuerySearchbarComponent {

    private _query: string;

    private _appNames$: Observable<string[]>;
    private _appNames: string[];

    private _level$: Observable<ECriticityLevel>;
    private _level: ECriticityLevel;

    private _dateRange$: Observable<ITimeSpanNavigatorState>;
    private _dateRange: ITimeSpanNavigatorState;

    private _subscriptions: Subscription[];

    private _logs: ILogView[];

    constructor(
        private logService: LogService,
        private store: Store<IAppState>,
    ) {
        this._subscriptions = [];
        this._query = '';
        this._appNames$ = this.store.select(s => s.luceneParameters.appNames);
        this._level$ = this.store.select(s => s.luceneParameters.level);
        this._dateRange$ = this.store.select( s => s.timeSpanNavigator);
        this._subscriptions.push(this._appNames$.subscribe(a => this._appNames = a));
        this._subscriptions.push(this._level$.subscribe(l => this._level = l));
        this._subscriptions.push(this._dateRange$.subscribe(d => this._dateRange = d));
    }

    onClick(): void {
        this.logService
            .filter({appName: this._appNames})
            .subscribe(l => this._logs = l);
    }
}