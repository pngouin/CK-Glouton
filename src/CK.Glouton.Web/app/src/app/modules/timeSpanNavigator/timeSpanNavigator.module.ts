import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SliderModule } from 'primeng/components/slider/slider';
import { RadioButtonModule } from 'primeng/components/radiobutton/radiobutton';
import { TimeSpanNavigatorComponent } from './components';


@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SliderModule,
        RadioButtonModule
   ],
   declarations: [
        TimeSpanNavigatorComponent
   ],
   exports: [
        TimeSpanNavigatorComponent
   ],
   schemas: [
        CUSTOM_ELEMENTS_SCHEMA
   ]
})
export class TimeSpanNavigatorModule {
}