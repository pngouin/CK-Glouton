import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ISender } from 'app/modules/ifttt/models/sender.model';
import { concat } from 'rxjs/operator/concat';
import { SelectItem } from 'primeng/api';
import { IftttService } from 'app/_services';
import { MessageService } from 'primeng/components/common/messageservice';
import {Message} from 'primeng/components/common/api';

@Component({
    selector: 'senderChooser',
    templateUrl: 'senderChooser.component.html',
    styleUrls : ['senderChooser.component.css']
})

export class SenderChooserComponent implements OnInit {
    constructor(private iftttService : IftttService, private messageService : MessageService) { }

    cache : string;
    selectedSender : ISender;
    selectedAddSender : string;
    selectedAddSenderName : string;

    senders : SelectItem[] = [
        {label : "Choose a sender", value : null}
    ];

    addSenderDropDown : SelectItem[] = [
        {label : "Choose a sender", value : null}
    ]

    @Output() onSend = new EventEmitter();

    ngOnInit() {
        this.iftttService.getAvaibleConfiguration().subscribe(
            d => {
                for (let name of d)
                {
                    this.addSenderDropDown.push( { label : name, value : name });
                }
            },
            error => this.messageService.add({
                severity : 'error', summary : 'Error', detail : 'The configuration recovery fail.'
            })
        );
     }

    addSender ( name : string, senderName : string ) : void {
        if (senderName == "" || typeof senderName === 'undefined') {
            this.messageService.add( {
                severity : 'warn', summary : 'Warning', detail : 'The sender configuration need a name.'
            })
            return;
        }

        if ( name == undefined || name == null || name == '') {
            this.messageService.add( {
                severity : 'warn', summary : 'Warning', detail : 'You must select a sender.'
            })
            return;
        }

        this.iftttService.getConfiguration(name).subscribe(
            d => this.senders.push({ label : senderName, value : {SenderType : name, Configuration : d}})
        );
    }

    getKeys () : string[] {
        return Object.keys(this.selectedSender.Configuration);
    }

    getPropertyType( propertyName : string ) : string {
        let p = Reflect.get(this.selectedSender.Configuration, propertyName);
        if(Array.isArray(p)) {
            return "Array"
        }
        else {
            return typeof(p);
        }
    }

    getPropertyValue( propertyName : string ) : string | number | Array<string> {
        return Reflect.get(this.selectedSender.Configuration, propertyName);
    }

    setConfigurationProperty( propertyName : string, value : string | number | Array<string> ) {
        Reflect.set(this.selectedSender.Configuration, propertyName, value);
    }

    addToArray( propertyName : string, value : string ) : void {
        if (value == "") return;
        let array = this.getPropertyValue(propertyName);
        if (!Array.isArray(array)) return;
        array.push(value);
        this.setConfigurationProperty(propertyName, array);
        this.cache = "";
    }

    deleteToArray(propertyName : string, index : number) : void {
        let array = this.getPropertyValue(propertyName);
        if (!Array.isArray(array)) return
        array.splice(index, 1);
        this.setConfigurationProperty(propertyName, array);
    }

    send() : void {
        this.onSend.emit();
    }

    validate() : boolean {
        for (let sender of this.senders) {
            if (sender.label == this.senders[0].label) {
                continue;
            }
            let value : any;
            for(let property of Object.keys(sender)) {
                value = this.getPropertyValue(property);
                switch(this.getPropertyType(property)) {
                    case 'array' :
                        if (value != undefined && value.length > 0) {
                            continue;
                        }
                        return false;
                    case 'string' :
                        if (value != undefined && value != '') {
                            continue;
                        }
                        return false;
                    case 'number' : 
                        if (value != undefined && value > 0) {
                            continue;
                        }
                        return false;
                    default : 
                        return false;
                }
            }
            return true;
        }
    }
}