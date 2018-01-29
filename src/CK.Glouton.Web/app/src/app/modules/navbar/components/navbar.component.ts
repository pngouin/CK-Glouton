import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/primeng';

@Component({
    selector: 'navbar',
    templateUrl: 'navbar.component.html',
    styleUrls: ['navbar.component.css']
})

export class NavbarComponent implements OnInit {
    items: MenuItem[];

    ngOnInit(): void {
        this.items = [
            {
                label : 'Logs',
                routerLink :  ['/log'],
                icon : 'fa-align-left'
            },
            {
                label : 'Stats',
                routerLink : ['/home'],
                icon : 'fa-pie-chart'
            },
            {
                label : 'Alert',
                routerLink : ['/ifttt'],
                icon: 'fa-bell-o'
            }
        ]
     }
}