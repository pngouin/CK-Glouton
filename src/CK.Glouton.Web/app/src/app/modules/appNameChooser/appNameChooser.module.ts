import { NgModule } from '@angular/core';
import { AppNameComponent } from './components';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core/src/metadata/ng_module';
import { AppNameService } from 'app/_services';
import { RadioButtonModule } from 'primeng/primeng';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        RadioButtonModule
    ],
    exports: [
        AppNameComponent
    ],
    declarations: [AppNameComponent],
    providers: [
        AppNameService
    ],
    schemas: [
        CUSTOM_ELEMENTS_SCHEMA
    ]
})
export class NameModule { }
