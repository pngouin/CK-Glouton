import { NgModule } from '@angular/core';
import { ApplicationNameSelectorComponent } from './components';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RadioButtonModule } from 'primeng/primeng';

import * as components from './components';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        RadioButtonModule
    ],
    declarations: [
        ...Object.values(components)
    ],
    exports: [
        ...Object.values(components)
    ]
})
export class NameModule { }
