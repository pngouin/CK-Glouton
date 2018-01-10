import { Component, OnInit } from '@angular/core';
import { Input } from '@angular/core';
import { IExceptionViewModel } from 'app/common/logs/models';

@Component({
    selector: 'exception',
    templateUrl: 'logException.component.html',
    styleUrls: ['logException.component.css']
})

export class LogExceptionComponent implements OnInit {
    constructor() { }

    @Input('data') exception : IExceptionViewModel

    ngOnInit() { }
}