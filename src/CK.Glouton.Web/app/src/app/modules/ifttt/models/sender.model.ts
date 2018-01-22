import { IExpression } from 'app/modules/ifttt/models/expression.model';

export interface ISender {
    Name : string,
    Configuration : Object
}

export interface ISender {
    SenderType : string,
    Configuration : Object
}

export interface IAlertExpressionModel {
    Expressions : IExpression[],
    Senders : object[]
}