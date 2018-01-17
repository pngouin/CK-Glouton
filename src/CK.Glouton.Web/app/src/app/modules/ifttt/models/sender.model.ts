export interface ISender {
    Name : string,
    Configuration : Object
}

export interface ISenderMailConfiguration {
    Name : string,
    Email : string,
    SmtpUsername : string,
    SmtpPassword : string,
    SmtpAdress : string,
    SmptPort : number,
    Contacts : string[]
}

export interface ISenderData {
    label : string,
    value : ISender
}