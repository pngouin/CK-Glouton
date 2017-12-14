import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { IAppState } from 'app/app.state';
import { MenuItem } from 'primeng/primeng';
import { ILogViewModel } from 'app/common/logs/models';
import { LogService } from 'app/_services';
import { EffectDispatcher } from '@ck/rx';
import { ITimeSpanNavigatorState } from 'app/modules/timeSpanNavigator/state/timeSpanNavigator.state';

@Component({
    selector: 'logViewer',
    templateUrl: 'logViewer.component.html'
})
export class LogViewerComponent {

    private _appNames$: Observable<string[]>;
    private _appNames: string[];

    private _level$: Observable<string[]>;
    private _level: string[];

    private _dateRange$: Observable<ITimeSpanNavigatorState>;
    private _dateRange: ITimeSpanNavigatorState;

    private _subscriptions: Subscription[];

    private _logs: ILogViewModel[];
    private _loading: boolean;

    constructor(
        private logService: LogService,
        private store: Store<IAppState>,
    ) {
        this._subscriptions = [];
        this._loading = false;
        this._appNames$ = this.store.select(s => s.luceneParameters.appNames);
        this._level$ = this.store.select(s => s.luceneParameters.level);
        this._dateRange$ = this.store.select( s => s.timeSpanNavigator);
        this._subscriptions.push(this._appNames$.subscribe(a => this._appNames = a));
        this._subscriptions.push(this._level$.subscribe(l => this._level = l));
        this._subscriptions.push(this._dateRange$.subscribe(d => this._dateRange = d));
    }

    public getLogs(query: string): void {
        this._loading = true;
        this._logs = null;
        this.logService
            .filter(
                {
                    appName: this._appNames,
                    keyword: query,
                    logLevel: this._level
                }
            ).subscribe(l => {
                this._logs = l;
                this._loading = false;
            });
    }
}