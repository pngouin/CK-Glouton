import { Component, ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'ifttt-page',
  template: `
    <div class="ui-g">
      <div class="ui-g-6">
        <ifttt></ifttt>
      </div>
    <div class="ui-g-6">
      <senderChooser></senderChooser>
    </div>
  `
})
export class IftttPageComponent {
}
