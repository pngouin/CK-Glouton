import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/primeng';

@Component({
    selector: 'navbar',
    templateUrl: 'navbar.component.html'
})

export class NavbarComponent implements OnInit {
    constructor() { }

    items: MenuItem[];

    ngOnInit() {
        this.items = [
            {
                label : 'Logs',
                routerLink :  ['/log']
            },
            {
                label : 'Stats',
                routerLink : ['/home']
            },
            {
                label : 'IFTTT',
                routerLink : ['/ifttt']
            }
        ]
     }
}