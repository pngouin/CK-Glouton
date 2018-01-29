import { Component, ChangeDetectorRef } from '@angular/core';
import { ViewChild } from '@angular/core';
import { IftttComponent, SenderChooserComponent } from 'app/modules/ifttt/components';
import { IAlertExpressionModel, ISender } from 'app/modules/ifttt/models/sender.model';
import { IftttService } from 'app/_services';
import { MessageService } from 'primeng/components/common/messageservice';

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

  constructor(private iftttService: IftttService, private messageService: MessageService) { }

  @ViewChild(IftttComponent)
  private iftttComponent: IftttComponent;

  @ViewChild(SenderChooserComponent)
  private senderChooserComponent: SenderChooserComponent;

  send(): void {
    if (!this.iftttComponent.validate() || !this.senderChooserComponent.validate()) {
      this.messageService.add({ severity: 'warn', summary: 'Warning', detail: 'One or more fields are empty.' });
      return;
    }


    let senders: ISender[] = [];

    for (let sender of this.senderChooserComponent.senders) {
      if (sender.value === null || typeof sender.value === 'undefined') {
        continue;
      }
      senders.push(sender.value);
    }

    let data: IAlertExpressionModel = {
      Expressions: this.iftttComponent.expressions,
      Senders: senders
    };

    this.iftttService.sendAlert(data).subscribe(d => {
      this.messageService.add(
        {
          severity: 'success', summary: 'Success !', detail: 'Your alert has been correctly created.'
        });
      this.iftttComponent.clear();
    },
    error => this.messageService.add(
      {
        severity: 'error', summary: 'Error', detail: 'An error has occured.'
      })
    );
  }

}
