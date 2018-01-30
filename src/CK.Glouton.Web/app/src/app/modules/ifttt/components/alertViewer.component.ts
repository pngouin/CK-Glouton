import { Component, OnInit } from '@angular/core';
import { IAlertData, IAlertDataViewModel, ISenderModel } from 'app/modules/ifttt/models/expression.model';
import { IftttService } from 'app/_services';
import { error } from 'util';
import { MessageService } from 'primeng/components/common/messageservice';
import { IAlertExpressionModel } from 'app/modules/ifttt/models/sender.model';

@Component({
    selector: 'alertviewer',
    templateUrl: 'alertViewer.component.html',
    styleUrls: ['alertViewer.component.css']
})

export class AlerViewerComponent implements OnInit {
    constructor(private iftttService: IftttService, private messageService: MessageService) { }

    alerts: IAlertData[];
    data: IAlertDataViewModel[] = [];

    ngOnInit(): void {
        this.update();
    }

    update(): void {
        this.iftttService.getAlert().subscribe(d => this.createDataViewModel(d),
            error => this.messageService.add({
                severity: 'error', summary: 'Error', detail: `An error have occured(${error.message})`
            }));
    }


    private createDataViewModel(alerts: IAlertData[]): void {
        let alertStr: string;
        for (let alert of alerts) {
            alertStr = this.concatenateSender(alert.senders);
            this.data.push({
                expressions: alert.expressions,
                senders: alertStr
            });
        }
    }

    private concatenateSender(senders: ISenderModel[]): string {
        let s: string = '';
        for (let sender of senders) {
            if (sender.senderType === undefined) {
                continue;
            }
            s += `${sender.senderType} `;
        }
        return s;
    }
}