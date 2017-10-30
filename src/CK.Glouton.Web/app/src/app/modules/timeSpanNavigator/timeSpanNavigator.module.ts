import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TimeSpanNavigatorComponent } from './components';


@NgModule({
    imports: [
        CommonModule,
        FormsModule
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