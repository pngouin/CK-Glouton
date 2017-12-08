import { Component, OnInit } from '@angular/core';
import { IFilter } from 'app/common/logs/models';

@Component({
    selector: 'criticitySelector',
    templateUrl: 'criticitySelector.component.html'
})


export class criticitySelectorComponent implements OnInit {
    
    private _filter: logFilter;

    GetFilters() :string[]{
        return this._filter.getArray();
    }

    OnChange() :void{
        console.log(this._filter.getArray());
    }

    ngOnInit(){
        this._filter = new logFilter();
    }
}

class logFilter implements IFilter {
    debug: boolean;
    trace: boolean;
    info: boolean;
    warn: boolean;
    error: boolean;
    fatal: boolean;

    constructor() {
        this.debug = false;
        this.trace = false;
        this.info = false;
        this.warn = false;
        this.error = false;
        this.fatal = false;
    }

    getArray() :string[]{
        let array = new Array();
        if (this.debug) array.push("debug");
        if (this.trace) array.push("trace");
        if (this.info) array.push("info");
        if (this.warn) array.push("warn");
        if (this.error) array.push("error");
        if (this.fatal) array.push("fatal"); 
        return array;
    }
}