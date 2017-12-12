import { Component, OnInit } from '@angular/core';
import { LogService } from 'app/_services';
import { ILogView } from 'app/common/logs/models';

@Component({
    selector: 'applicationNameSelector',
    templateUrl: 'applicationNameSelector.component.html'
})

export class ApplicationNameSelectorComponent implements OnInit {

    private _applicationNames: string[];
    private _selected: string[];

    constructor(
        private logService: LogService
    ) {
    }

    onChange(_: boolean): void {
        console.log(this._selected);
    }

    ngOnInit(): void {
        this.logService.getAllApplicationName()
            .subscribe(n => this._applicationNames = n);
     }
}