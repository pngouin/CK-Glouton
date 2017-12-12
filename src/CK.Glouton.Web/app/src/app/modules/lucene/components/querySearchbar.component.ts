import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'querySearchbar',
    templateUrl: 'querySearchbar.component.html'
})

export class QuerySearchbarComponent implements OnInit {

    private _query: string;

    onSubmit(): void {
        console.log(this._query);
    }

    ngOnInit(): void {
        this._query = '';
    }
}