import { Component, OnInit } from '@angular/core';
import { AppNameService } from 'app/_services';

@Component({
    selector: 'appName',
    templateUrl: 'appName.component.html'
})

export class AppNameComponent implements OnInit {

    private _appNames: string[];
    private _chooseAppName: string;

    constructor(private appNameService: AppNameService) { }

    onClick() {
        // TODO: Event emitter to parent component or use service to update selected result ?
    }

    ngOnInit() {
        this.appNameService.GetAll().subscribe(appN => this._appNames = appN);
     }
}