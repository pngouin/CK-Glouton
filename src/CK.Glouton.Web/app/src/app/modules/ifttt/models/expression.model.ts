export interface IExpression {
    Field: string;
    Operation: string;
    Body: string;
}

export interface IAlertData {
    expressions: IExpression[];
    senders: ISenderModel[];
}

export interface IAlertDataViewModel {
    expressions: IExpression[];
    senders: string;
}

export interface ISenderModel {
    senderType: string;
    configuration: any;
}