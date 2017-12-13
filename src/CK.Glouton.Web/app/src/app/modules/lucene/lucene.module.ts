import { NgModule } from '@angular/core';
import { ApplicationNameSelectorComponent } from './components';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
    RadioButtonModule,
    CheckboxModule,
    InputTextModule,
    TabViewModule,
    ButtonModule
} from 'primeng/primeng';

import * as components from './components';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        RadioButtonModule,
        CheckboxModule,
        InputTextModule,
        TabViewModule,
        ButtonModule
    ],
    declarations: [
        ...Object.values(components)
    ],
    exports: [
        ...Object.values(components)
    ]
})
export class LucenePreferenceModule { }
