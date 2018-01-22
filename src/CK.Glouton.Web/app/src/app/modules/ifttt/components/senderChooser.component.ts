import { Component, OnInit } from '@angular/core';
import { ISender, MailSender } from 'app/modules/ifttt/models/sender.model';
import { concat } from 'rxjs/operator/concat';
import { SelectItem } from 'primeng/api';
import { IftttService } from 'app/_services';

@Component({
    selector: 'senderChooser',
    templateUrl: 'senderChooser.component.html',
    styleUrls : ['senderChooser.component.css']
})

export class SenderChooserComponent implements OnInit {
    constructor(private iftttService : IftttService) { }

    contact : string;
    selectedSender : ISender;
    selectedAddSender : string;
    selectedAddSenderName : string;

    senders : SelectItem[] = [] = [
        {label : "Choose a sender", value : null}
    ];

    addSenderDropDown : SelectItem[] = [
        {label : "Choose a sender", value : null}
    ]

    ngOnInit() {
        this.iftttService.getAvaibleConfiguration().subscribe(
            d => {
                for (let name of d)
                {
                    this.addSenderDropDown.push( { label : name, value : name });
                }
            }
        );
     }

    addSender ( name : string, senderName : string ) : void {
        if (senderName == "" || typeof senderName === 'undefined') return;
        this.iftttService.getConfiguration(name).subscribe(
            d => this.senders.push({ label : senderName, value : {Name : name, Configuration : d}})
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
    }

    deleteToArray(propertyName : string, index : number) : void {
        let array = this.getPropertyValue(propertyName);
        if (!Array.isArray(array)) return
        array.splice(index, 1);
        this.setConfigurationProperty(propertyName, array);
    }
}