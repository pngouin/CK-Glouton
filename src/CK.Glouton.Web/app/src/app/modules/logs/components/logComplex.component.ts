import { Component, OnInit } from '@angular/core';
import { Input } from '@angular/core';
import { ILogViewModel, LogType } from 'app/common/logs/models';

@Component({
    selector: 'logComplex',
    templateUrl: 'logComplex.component.html',
    styleUrls: ['logComplex.component.css']
})

export class LogComplexComponent implements OnInit {
    constructor() { }

    @Input('data') log : ILogViewModel;

    getLogTypeString () : string {
        return LogType[this.log.logType];
    }

    ngOnInit() { }
}