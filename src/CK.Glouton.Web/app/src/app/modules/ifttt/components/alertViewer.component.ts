import { Component, OnInit } from '@angular/core';
import { IAlertData } from 'app/modules/ifttt/models/expression.model';
import { IftttService } from 'app/_services';
import { error } from 'util';
import { MessageService } from 'primeng/components/common/messageservice';

@Component({
    selector: 'alertviewer',
    templateUrl: 'alertViewer.component.html',
    styleUrls: ['alertViewer.component.css']
})

export class AlerViewerComponent implements OnInit {
    constructor(private iftttService: IftttService, private messageService: MessageService) { }

    data: IAlertData[];

    ngOnInit(): void {
        this.update();
    }

    update(): void {
        this.iftttService.getAlert().subscribe(d => this.data = d,
            error => this.messageService.add({
                severity: 'error', summary: 'Error', detail: `An error have occured(${error.message})`
            }));
    }

    concatenateSender(senders: string[]): string {
        let s: string = '';
        for (let sender of senders) {
            s.concat(`${sender},`);
        }
        return s;
    }
}