import { NgModule } from '@angular/core';
import { querySearchbarComponent } from './components';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import * as components from './components';

@NgModule({
    imports: [
        CommonModule,
        FormsModule
    ],
    declarations: [
        ...Object.values(components)
    ],
    exports: [
        ...Object.values(components)
    ]
})
export class SearchbarModule { }