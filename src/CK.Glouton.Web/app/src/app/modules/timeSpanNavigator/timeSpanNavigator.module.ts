import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TimeSpanNavigatorComponent } from './components';

@NgModule({
    imports: [
        CommonModule,
   ],
   declarations: [
        TimeSpanNavigatorComponent
   ],
   exports: [
        TimeSpanNavigatorComponent
   ]
})
export class TimeSpanNavigatorModule {
}