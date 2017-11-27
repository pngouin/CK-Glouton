import { Component, ChangeDetectorRef } from '@angular/core';
import { ITimeSpanNavigatorSettings, Scale, IScaleEdge } from '../modules/timeSpanNavigator/models';

@Component({
  selector: 'home',
  template: `
    <h3>Home Page</h3>
    <div>
      This is our home page!
    </div>
    <h4>Time Span Navigator Component</h4>
    <div>
      <timeSpanNavigator
        [configuration]="timeSpanNavigatorConfiguration"
        (onDateChange)="onDateChange($event)">
      </timeSpanNavigator>
      <div *ngFor="let d of _dateRangeStr">
        {{d | date:'EEEE, MMMM d, y, h:mm:ss'}}
      </div>
    </div>
    <h4>Application Name Selector</h4>
    <div>
      <applicationNameSelector></applicationNameSelector>
    </div>
  `
})
export class HomePageComponent {

  private _dateRange: Date[];
  private _dateRangeStr: string[];

  constructor() {
    this._dateRange = new Array<Date>();
    this._dateRangeStr = new Array<string>();
  }

  timeSpanNavigatorConfiguration: ITimeSpanNavigatorSettings = {
    from: new Date('2017-11-01'),
    to: new Date(),
    initialScale: Scale.Hours,
    edges: {
      Years: {min: 1, max: 2},
      Months: {min: 2, max: 12},
      Days: {min: 5, max: 31},
      Hours: {min: 4, max: 24},
      Minutes: {min: 10, max: 60},
      Seconds: {min: 1, max: 60}
    }
  };

  private onDateChange(date: Date[]): void {
    this._dateRange = date;
    this.updateDateStr();
  }

  private updateDateStr(): void {
    this._dateRangeStr[0] = this._dateRange[0].toString();
    this._dateRangeStr[1] = this._dateRange[1].toString();
  }
}
