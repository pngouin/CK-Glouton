import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'querySearchbar',
    templateUrl: 'querySearchbar.component.html'
})

export class querySearchbarComponent implements OnInit {
    
    private _query :string;

    OnSubmit() :void {
        console.log(this._query);
    }

    ngOnInit(){
        this._query = "";
    }
}