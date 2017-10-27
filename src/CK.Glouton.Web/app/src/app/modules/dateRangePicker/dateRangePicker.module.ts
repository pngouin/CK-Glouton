import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DateRangePickerComponent } from './components';

@NgModule({
    imports: [
        CommonModule,
   ],
   declarations: [
        DateRangePickerComponent
   ],
   exports: [
        DateRangePickerComponent
   ]
})
export class DateRangePickerModule {
}