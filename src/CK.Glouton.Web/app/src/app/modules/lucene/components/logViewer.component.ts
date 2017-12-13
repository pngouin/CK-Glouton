import { Component, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import { IAppState } from 'app/app.state';
import { MenuItem } from 'primeng/primeng';
import { ILogView } from 'app/common/logs/models';

@Component({
    selector: 'logViewer',
    templateUrl: 'logViewer.component.html'
})
export class LogViewerComponent {

    private _logs: ILogView[];
}