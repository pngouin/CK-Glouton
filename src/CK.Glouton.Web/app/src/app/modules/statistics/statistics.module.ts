import { NgModule } from '@angular/core';

import * as components from './components';

@NgModule({
    declarations: [
        ...Object.values(components)
    ],
    exports: [
        ...Object.values(components)
    ]
})
export class StatisticsModule { }
