import { NgModule } from '@angular/core';

import * as components from './components';

@NgModule({
    imports: [],
    declarations: [
        ...Object.values(components)
    ],
    exports: [
        ...Object.values(components)
    ]
})
export class IftttModule { }
