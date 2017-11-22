import { Component, OnInit } from '@angular/core';
import { LogService } from 'app/_services';

@Component({
    selector: 'applicationNameSelector',
    templateUrl: 'applicationNameSelector.component.html'
})

export class ApplicationNameSelectorComponent implements OnInit {

    private _applicationNames: string[];

    private _choosedAppName: string;

    constructor(
        private logService: LogService
    ) {
    }

    onClick(): void {
        // TODO: Event emitter to parent component or use service to update selected result ?
    }

    ngOnInit(): void {
        this.logService.getAllApplicationName()
            .subscribe(n => this._applicationNames = n);
     }
}