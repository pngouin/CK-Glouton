import { Component, OnInit } from '@angular/core';
import { LogService } from 'app/_services';
import { ILogView } from 'app/common/logs/models';

@Component({
    selector: 'applicationNameSelector',
    templateUrl: 'applicationNameSelector.component.html'
})

export class ApplicationNameSelectorComponent implements OnInit {

    private _applicationNames: string[];
    private _selectedApplicationNameIndex: number;

    private _logs: ILogView[];

    constructor(
        private logService: LogService
    ) {
    }

    onSubmit(): void {
        console.log(this._applicationNames[this._selectedApplicationNameIndex]);
        this.logService.getAll(this._applicationNames[this._selectedApplicationNameIndex])
            .subscribe(l => this._logs = l);
    }

    ngOnInit(): void {
        this.logService.getAllApplicationName()
            .subscribe(n => this._applicationNames = n);
     }
}