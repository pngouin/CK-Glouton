import { Component, ViewChild } from '@angular/core';
import { ITimeSpanNavigatorSettings, Scale } from 'app/modules/timeSpanNavigator/models';
import { LogViewerComponent } from 'app/modules/lucene/components';

@Component({
  selector: 'home',
  template: `
    <div class="ui-g">
      <div class="ui-g-2">
        <querySearchbar (searchEmitter)="searchEvent($event)"></querySearchbar>
        <h3>Criticity Level</h3>
        <criticitySelector></criticitySelector>

        <h3>Current AppNames</h3>
        <applicationNameSelector></applicationNameSelector>
      </div>
      <div class="ui-g-10">
        <div class="ui-g-12">
          <timeSpanNavigator [configuration]="timeSpanNavigatorConfiguration"></timeSpanNavigator>
        </div>
        <div class="ui-g-12">
          <logViewer></logViewer>
        </div>
      </div>
    </div>
  `
})
export class LogViewPageComponent {
  @ViewChild(LogViewerComponent) private logViewer: LogViewerComponent;

  private timeSpanNavigatorConfiguration: ITimeSpanNavigatorSettings = {
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

  private searchEvent(event: string): void {
    this.logViewer.getLogs(event);
  }
}