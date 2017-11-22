import { Component, ChangeDetectorRef } from '@angular/core';
import { ITimeSpanNavigatorSettings, Scale, IScaleEdge } from '../modules/timeSpanNavigator/models';

@Component({
  selector: 'home',
  template: `
    <h3>Home Page</h3>
    <div>
      This is our home page!
    </div>
    <div>
      <timeSpanNavigator
        [configuration]="timeSpanNavigatorConfiguration"
        [edges]="edgesConfiguration"
        (onDateChange)="onDateChange($event)">
      </timeSpanNavigator>
    </div>
    <div>
      <applicationNameSelector></applicationNameSelector>
    </div>
  `
})
export class HomePageComponent {

  private _dateRange: Date[];

  constructor() {
    this._dateRange = new Array<Date>();
  }

  timeSpanNavigatorConfiguration: ITimeSpanNavigatorSettings = {
    from: new Date('2017-11-01'),
    to: new Date(),
    scale: Scale.Hours
  };

  edgesConfiguration : IScaleEdge = {
    Years :  {min: 1, max: 2},
    Months :  {min: 2, max: 12},
    Days :  {min: 5, max: 31},
    Hours :  {min: 4, max: 24},
    Minutes :  {min: 10, max: 60},
    Seconds :  {min: 1, max: 60}
  };

  onDateChange(date: Date[]): void {
    this._dateRange = date;
  }
}
