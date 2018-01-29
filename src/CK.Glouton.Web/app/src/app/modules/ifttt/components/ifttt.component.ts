import { Component, OnInit } from '@angular/core';
import { IData } from '../models/logField.model';
import { IExpression } from 'app/modules/ifttt/models/expression.model';
import { Input } from '@angular/core';
import { MessageService } from 'primeng/components/common/messageservice';
import {Message} from 'primeng/components/common/api';


@Component({
    selector: 'ifttt',
    templateUrl: 'ifttt.component.html',
    styleUrls: ['ifttt.component.css']
})

export class IftttComponent implements OnInit {

    ngOnInit(): void {}

    selectedField: string = '';
    selectedOperation: string = '';
    selectedInfo: string = '';

    expressions : IExpression[] = [
        {Field : '', Operation : '', Body : '' }
    ];

    constructor(private messageService : MessageService) { }

    fields: IData[] = [
        { label: 'LogType', value: 'LogType' },
        { label: 'LogLevel', value: 'LogLevel' },
        { label: 'GroupDepth', value: 'GroupDepth' },
        { label: 'FileName', value: 'FileName' },
        { label: 'LineNumber', value: 'LineNumber' },
        { label: 'AppName', value: 'AppName' },
        { label: 'Text', value: 'Text' },
        { label: 'Tags', value: 'Tags' },
        { label: 'Exception.Message', value: 'Exception.Message' },
        { label: 'Exception.StackTrace', value: 'Exception.StackTrace' }
    ];

    operations: IData[] = [
        { label: 'EqualTo', value: 'EqualTo' },
        { label: 'Contains', value: 'Contains' },
        { label: 'StartsWith', value: 'StartsWith' },
        { label: 'EndsWith', value: 'EndsWith' },
        { label: 'NotEqualTo', value: 'NotEqualTo' }
    ];

    numberOperations: IData[] = [
        { label: 'EqualTo', value: 'EqualTo' },
        { label: 'NotEqualTo', value: 'NotEqualTo' },
        { label: 'GreaterThan', value: 'GreaterThan' },
        { label: 'GreaterThanOrEqualTo', value: 'GreaterThanOrEqualTo' },
        { label: 'LessThan', value: 'LessThan' },
        { label: 'LessThanOrEqualTo', value: 'LessThanOrEqualTo' }
    ];

    logTypes: IData[] = [
        { label: 'OpenGroup', value: 'OpenGroup' },
        { label: 'Line', value: 'Line' },
        { label: 'CloseGroup', value: 'CloseGroup' },
    ];

    logLevels: IData[] = [
        { label: 'None', value: 'None' },
        { label: 'Debug', value: 'Debug' },
        { label: 'Trace', value: 'Trace' },
        { label: 'Info', value: 'Info' },
        { label: 'Error', value: 'Error' },
        { label: 'Fatal', value: 'Fatal' },
        { label: 'IsFiltered', value: 'IsFiltered' },
    ];

    private IsParticular( expression : IExpression ): boolean {
        return expression.Field === 'LogType' ||
        expression.Field === 'LogLevel';
    }

    private IsNumber( expression : IExpression): boolean {
        return expression.Field === 'LineNumber' ||
            expression.Field === 'GroupDepth';
    }

    private getParticularOperation(expression : IExpression): IData[] {
        if (!this.IsParticular(expression) && !this.IsNumber(expression)) {
            return this.operations;
        }
        if (expression.Field === 'LogType') {
            expression.Operation = 'EqualTo';
            return [{ label: 'EqualTo', value: 'EqualTo' }];
        }
        if (expression.Field === 'LogLevel') {
            expression.Operation = 'In';
            return [{ label: 'In', value: 'In' }];
        }
        if (this.IsNumber(expression)) {
            return this.numberOperations;
        }
    }

    private resetSelected( expression : IExpression): void {
        if ((this.IsNumber(expression) && !this.isNumberOperation(expression.Operation)) ||
            (!this.IsNumber(expression) && this.isNumberOperation(expression.Operation))
        ) {
            expression.Body = '';
            expression.Operation = '';
        }
    }

    private addExpression (index : number): void {
        if (this.expressions[index].Body === '') {
            this.messageService.add( {
                severity : 'warn', summary : 'Warn', detail : 'The expression need a body.'
            });
            return;
        }
        this.expressions.splice( index+1, 0, {
            Field : '', Operation : '', Body : ''
        } );
    }

    private isNumberOperation ( operation : string): boolean {
        return this.numberOperations.findIndex( o => o.value === operation ) !== -1;
    }

    private deleteExpression (index : number): void {
        if (index === 0) {
            this.messageService.add( {
                severity : 'error', summary : 'Error', detail : 'You can\'t delete the first expression.'
            });
            return;
        }
         this.expressions.splice(index, 1);
    }


    validate(): boolean {
        for (let property of this.expressions) {
            if (property.Body !== undefined &&
                property.Body !== '' &&
                property.Field !== undefined &&
                property.Field !== '' &&
                property.Operation !== undefined &&
                property.Operation !== '') {
                    continue;
                }
            return false;
        }
        return true;
    }

    clear(): void {
        this.expressions = [
            {Field : '', Operation : '', Body : '' }
        ];
    }
}