import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MenubarModule } from 'primeng/menubar';

import * as components from './components';

@NgModule({
    imports: [
        CommonModule,
        MenubarModule
    ],
    declarations: [
        ...Object.values(components)
    ],
    exports: [
        ...Object.values(components)
    ]
})
export class NavbarModule { }
