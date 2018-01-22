import { Component, ChangeDetectorRef } from '@angular/core';
import { ViewChild } from '@angular/core';
import { IftttComponent, SenderChooserComponent } from 'app/modules/ifttt/components';
import { IAlertExpressionModel, ISender } from 'app/modules/ifttt/models/sender.model';
import { IftttService } from 'app/_services';

@Component({
  selector: 'ifttt-page',
  template: `
    <div class="ui-g">
      <div class="ui-g-6">
        <ifttt></ifttt>
      </div>
    <div class="ui-g-6">
      <senderChooser (onSend)="send()"></senderChooser>
    </div>
  `
})
export class IftttPageComponent {

  constructor (private iftttService : IftttService) { }

  @ViewChild(IftttComponent)
  private iftttComponent : IftttComponent;

  @ViewChild(SenderChooserComponent)
  private senderChooserComponent : SenderChooserComponent;

  send() {
    this.iftttComponent.expressions;
    this.senderChooserComponent.senders;

    let senders : ISender[] = [];

    for (let sender of this.senderChooserComponent.senders) {
      if (sender.value == null || typeof sender.value == 'undefined') continue;
      senders.push(sender.value);
    }

    let data : IAlertExpressionModel = {
      Expressions : this.iftttComponent.expressions,
      Senders : senders
    };

    this.iftttService.sendAlert(data).subscribe(d => console.log(d));
  }

}
