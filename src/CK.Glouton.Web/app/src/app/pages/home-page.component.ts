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
        {name: 'years', min: 1900, max: 2117},
        {name: 'months', min: 0, max: 12},
        {name: 'days', min: 0, max: 31},
        {name: 'hours', min: 0, max: 24},
        {name: 'minutes', min: 0, max: 60},
        {name: 'seconds', min: 0, max: 60}
    ],
    default: {
      from: new Date('1900-01-01'),
      to: new Date()
    }
  };
}
