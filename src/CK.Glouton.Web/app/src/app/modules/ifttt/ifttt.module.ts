import { NgModule } from '@angular/core';
import { DropdownModule } from 'primeng/primeng';
import {InputTextModule} from 'primeng/primeng';
import * as components from './components';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import {ButtonModule} from 'primeng/primeng';

@NgModule({
    imports: [
        CommonModule,
        DropdownModule,
        InputTextModule,
        ButtonModule,
        BrowserModule, 
        FormsModule
    ],
    declarations: [
        ...Object.values(components)
    ],
    exports: [
        ...Object.values(components)
    ]
})
export class IftttModule { }
