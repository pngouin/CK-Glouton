import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SliderModule } from 'primeng/components/slider/slider';
import { TimeSpanNavigatorComponent } from './components';

import * as components from './components';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SliderModule
   ],
   declarations: [
        ...Object.values(components)
   ],
   exports: [
        ...Object.values(components)
   ]
})
export class TimeSpanNavigatorModule {
}