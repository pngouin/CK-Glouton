import { Component } from '@angular/core';
import { ITimeSpanNavigatorSettings, Scale } from '../modules/timeSpanNavigator/models';

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
    from: new Date('1900-01-01'),
    to: new Date(),
    scale: Scale.Hours
  };
}
