export interface IExpression {
    Field : string;
    Operation : string;
    Body : string;
}

export interface IAlertData {
    expression: IExpression[];
    senders: [
        {
            senderType: string;
            configuration: any;
        }
    ];
}