import { NgModule } from '@angular/core';
import { DropdownModule } from 'primeng/primeng';
import {InputTextModule} from 'primeng/primeng';
import * as components from './components';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import {ButtonModule, MessageModule} from 'primeng/primeng';
import {KeyFilterModule} from 'primeng/keyfilter';
import {TableModule} from 'primeng/table';

@NgModule({
    imports: [
        CommonModule,
        DropdownModule,
        InputTextModule,
        ButtonModule,
        BrowserModule,
        TableModule,
        KeyFilterModule,
        MessageModule,
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
