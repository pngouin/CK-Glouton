import { Component, ChangeDetectorRef } from '@angular/core';
import { ITimeSpanNavigatorSettings, Scale, IScaleEdge } from '../modules/timeSpanNavigator/models';
import { AppNameService } from 'app/_services';

@Component({
  selector: 'home',
  template: `
    <h3>Home Page</h3>
    <div>
      This is our home page!
    </div>
    <div>
      <timeSpanNavigator [configuration]="timeSpanNavigatorConfiguration" [edges]="edgesConfiguration" (onDateChange)="onDateChange($event)"></timeSpanNavigator>
    </div>
    <div *ngFor="let date of _dateRange" >
    {{ date | date:'EEEE, MMMM d, y, h:mm:ss' }}
</div>
  `
})
export class HomePageComponent {

  private _dateRange: Date[];

  constructor(private changeRef: ChangeDetectorRef, private appNameService : AppNameService) {
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

  onDateChange(date: Date[]) {
    this._dateRange = date;
    console.log(this._dateRange);
  }
}
