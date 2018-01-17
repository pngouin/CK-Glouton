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

    addContact( mailConfiguration : ISenderMailConfiguration, mail : string ) : void {
        mailConfiguration.Contacts.push(mail);
        this.contact = "";
    }

    deleteContact(mailConfiguration : ISenderMailConfiguration, index : number) : void {
        mailConfiguration.Contacts.splice(index, 1);
    }
}