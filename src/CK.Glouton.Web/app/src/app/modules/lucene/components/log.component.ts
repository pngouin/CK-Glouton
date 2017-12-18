import { Component, OnInit } from '@angular/core';
import { Input } from '@angular/core/';
import { ILogViewModel } from 'app/common/logs/models';
import { Output } from '@angular/core';
import { EventEmitter } from '@angular/core';

@Component({
    selector: 'log',
    templateUrl: 'log.component.html',
    styleUrls: ['log.component.css']
})

export class LogComponent implements OnInit {

    @Input('data')
    log : ILogViewModel;

    @Output() childrenClick = new EventEmitter<ILogViewModel>();

    constructor() {  }

    marginLeft() : number {
        return this.log.groupDepth * 15;
    }

    getColor () : string {
        if (this.log.logLevel.indexOf('Fatal') >= 0)
            return "#2d2e30";
        if (this.log.logLevel.indexOf('Warn') >= 0)
            return "#f0ad4e";
        if (this.log.logLevel.indexOf('Info') >= 0)
            return "#5bc0de";
        if (this.log.logLevel.indexOf('Trace') >= 0)
            return "#a4a4a5";
        if (this.log.logLevel.indexOf('Error') >= 0)
            return "#d9534f";
        if (this.log.logLevel.indexOf('Debug') >= 0)
            return "#701ac0";
    }

    getText() : string {
        const maxLength = 80 - this.log.groupDepth * 15;
        if (!this.log.text)
            return "";
        if ( this.log.text.length > maxLength)
         return this.log.text.substr( 0, maxLength ) + " [...]";
        return this.log.text;
    }

    onClick(log : ILogViewModel) : void
    {
        console.log(log);
        this.childrenClick.emit(log);
    }

    ngOnInit() { }
}