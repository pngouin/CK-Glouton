import { Component, OnInit } from '@angular/core';
import { IData } from '../models/logField.model';
import { IExpression } from 'app/modules/ifttt/models/expression.model';
import { Input } from '@angular/core';

@Component({
    selector: 'ifttt',
    templateUrl: 'ifttt.component.html',
    styleUrls: ['ifttt.component.css']
})

export class IftttComponent implements OnInit {

    selectedField: string = "";
    selectedOperation: string = "";
    selectedInfo: string = "";

    @Input() expressions : IExpression[] = [
        {Field : "", Operation : "", Body : "" }
    ]

    fields: IData[] = [
        { label: "LogType", value: "LogType" },
        { label: "LogLevel", value: "LogLevel" },
        { label: "GroupDepth", value: "GroupDepth" },
        { label: "FileName", value: "FileName" },
        { label: "LineNumber", value: "LineNumber" },
        { label: "AppName", value: "AppName" },
        { label: "Text", value: "Text" },
        { label: "Tags", value: "Tags" },
        { label: "Exception.Message", value: "Exception.Message" },
        { label: "Exception.StackTrace", value: "Exception.StackTrace" }
    ]

    operations: IData[] = [
        { label: "EqualTo", value: "EqualTo" },
        { label: "Contains", value: "Contains" },
        { label: "StartsWith", value: "StartsWith" },
        { label: "EndsWith", value: "EndsWith" },
        { label: "NotEqualTo", value: "NotEqualTo" }
    ]

    numberOperations: IData[] = [
        { label: "EqualTo", value: "EqualTo" },
        { label: "NotEqualTo", value: "NotEqualTo" },
        { label: "GreaterThan", value: "GreaterThan" },
        { label: "GreaterThanOrEqualTo", value: "GreaterThanOrEqualTo" },
        { label: "LessThan", value: "LessThan" },
        { label: "LessThanOrEqualTo", value: "LessThanOrEqualTo" }
    ]

    logTypes: IData[] = [
        { label: "OpenGroup", value: "OpenGroup" },
        { label: "Line", value: "Line" },
        { label: "CloseGroup", value: "CloseGroup" },
    ]

    logLevels: IData[] = [
        { label: "None", value: "None" },
        { label: "Debug", value: "Debug" },
        { label: "Trace", value: "Trace" },
        { label: "Info", value: "Info" },
        { label: "Error", value: "Error" },
        { label: "Fatal", value: "Fatal" },
        { label: "IsFiltered", value: "IsFiltered" },
    ]

    private IsParticular( expression : IExpression ) : boolean {
        return expression.Field == "LogType" ||
        expression.Field == "LogLevel";
    }

    private IsNumber( expression : IExpression) : boolean {
        return expression.Field == "LineNumber" || 
            expression.Field == "GroupDepth";
    }

    private getParticularOperation(expression : IExpression): IData[] {
        if (!this.IsParticular(expression) && !this.IsNumber(expression)) {
            return this.operations;
        }
        if (expression.Field == "LogType") {
            expression.Operation = "EqualTo";
            return [{ label: "EqualTo", value: "EqualTo" }];
        }
        if (expression.Field == "LogLevel") {
            expression.Operation = "In";
            return [{ label: "In", value: "In" }]
        }
        if (this.IsNumber(expression)) {
            return this.numberOperations;
        }
    }

    private resetSelected( expression : IExpression) : void {
        if ((this.IsNumber(expression) && !this.isNumberOperation(expression.Operation)) || 
            (!this.IsNumber(expression) && this.isNumberOperation(expression.Operation))
        ){
            expression.Body = "";
            expression.Operation = "";
        }
    }

    private addExpression (index : number) : void {
        this.expressions.splice( index+1, 0, {
            Field : "", Operation : "", Body : ""
        } );
    }

    private isNumberOperation ( operation : string) : boolean {
        return this.numberOperations.findIndex( o => o.value == operation ) != -1;
    }

    private deleteExpression (index : number) : void {
        this.expressions.splice(index, 1);
    }

    constructor() { }

    ngOnInit() { }
}