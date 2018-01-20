import { Component, OnInit } from '@angular/core';
import { ISender, ISenderMailConfiguration, ISenderData } from 'app/modules/ifttt/models/sender.model';
import { concat } from 'rxjs/operator/concat';

@Component({
    selector: 'senderChooser',
    templateUrl: 'senderChooser.component.html',
    styleUrls : ['senderChooser.component.css']
})

export class SenderChooserComponent implements OnInit {
    constructor() { }

    contact : string;

    MailConfiguration : ISenderMailConfiguration = {
        Name : "",
        Email : "",
        SmtpUsername : "",
        SmtpPassword : "",
        SmtpAdress : "",
        SmptPort : -1,
        Contacts : []
    }

    MailSender : ISender = {
        Name : "Mail",
        Configuration : this.MailConfiguration
    }

    selectedSender : ISender = this.MailSender;

    Senders : ISenderData[] = [
        { label : "Mail", value : this.MailSender }
    ]

    ngOnInit() { }

    getConfigurationKeys ( configuration : object ) : string[] {
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

    addContact( propertyName : string, value : string ) : void {
        if (value == "") return;
        let array = this.getPropertyValue(propertyName);
        if (!Array.isArray(array)) return;
        array.push(value);
        this.setConfigurationProperty(propertyName, array);
    }

    deleteContact(propertyName : string, index : number) : void {
        let array = this.getPropertyValue(propertyName);
        if (!Array.isArray(array)) return
        array.splice(index, 1);
        this.setConfigurationProperty(propertyName, array);
    }
}