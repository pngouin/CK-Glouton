import { Component, Output, EventEmitter } from '@angular/core';

@Component({
    selector: 'querySearchbar',
    templateUrl: 'querySearchbar.component.html'
})
export class QuerySearchbarComponent {

    private _query: string;

    @Output() searchEmitter: EventEmitter<string> = new EventEmitter();

    constructor(
    ) {
        this._query = '';
    }

    onClick(): void {
        this.searchEmitter.emit(this._query);
    }
}