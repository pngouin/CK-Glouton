import { Component } from '@angular/core';

@Component({
  selector: 'home',
  template: `
    <div class="ui-g">
      <div class="ui-g-4">
        <querySearchbar></querySearchbar>
        <h3>Criticity Level</h3>
        <criticitySelector></criticitySelector>

        <h3>Current AppNames</h3>
        <applicationNameSelector></applicationNameSelector>
      </div>
      <div class="ui-g-8">

      </div>
    </div>
  `
})
export class LogViewPageComponent {
}