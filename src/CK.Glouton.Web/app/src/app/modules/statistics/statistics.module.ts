import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChartModule } from 'primeng/primeng';

import * as components from './components';

@NgModule({
    imports: [
        CommonModule,
        ChartModule
    ],
    declarations: [
        ...Object.values(components)
    ],
    exports: [
        ...Object.values(components)
    ]
})
export class StatisticsModule { }
