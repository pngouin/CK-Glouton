import { Component } from '@angular/core';
import { ITimeSpanNavigatorSettings } from '../modules/timeSpanNavigator/models';

@Component({
  selector: 'home',
  template: `
    <h3>Home Page</h3>
    <div>
      This is our home page!
    </div>
    <div>
      <timeSpanNavigator [configuration]= "timeSpanNavigatorConfiguration"></timeSpanNavigator>
    </div>
  `
})
export class HomePageComponent {
  timeSpanNavigatorConfiguration: ITimeSpanNavigatorSettings = {
    scales: [
        {name: 'years', min: 1900, max: 2117, step: 1},
        {name: 'months', min: 0, max: 12, step: 1 },
        {name: 'days', min: 0, max: 31, step: 1},
        {name: 'hours', min: 0, max: 24, step: 1},
        {name: 'minutes', min: 0, max: 60, step: 1},
        {name: 'seconds', min: 0, max: 60, step: 1}
    ],
    default: {
      scale: 'hours',
      from: 0,
      to: 12
    }
  };
}
