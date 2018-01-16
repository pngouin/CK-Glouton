import { Component, OnInit } from '@angular/core';
import { IData } from '../models/logField.model';

@Component({
    selector: 'ifttt',
    templateUrl: 'ifttt.component.html',
    styleUrls: ['ifttt.component.css']
})

export class IftttComponent implements OnInit {

    selectedField: string = "";
    selectedOperation: string = "";
    selectedInfo: string = "";


    fields: IData[] = [
        { label: "LogType", value: "LogType" },
        { label: "LogLevel", value: "LogLevel" },
        { label: "GroupDepth", value: "GroupDepth" },
        { label: "SourceFileName", value: "SourceFileName" },
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
        { label: "Contains", value: "Contains" },
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

    private IsParticular() {
        return this.selectedField == "LogType" ||
            this.selectedField == "LogLevel";
    }

    private IsNumber() {
        return this.selectedField == "LineNumber" || 
            this.selectedField == "GroupDepth";
    }

    private getParticularOperation(): IData[] {
        if (!this.IsParticular() && !this.IsNumber()) {
            return this.operations;
        }
        if (this.selectedField == "LogType") {
            this.selectedOperation = "EqualTo";
            return [{ label: "EqualTo", value: "EqualTo" }];
        }
        if (this.selectedField == "LogLevel") {
            this.selectedOperation = "Contains";
            return [{ label: "Contains", value: "Contains" }]
        }
        if (this.IsNumber()) {
            return this.numberOperations;
        }
    }

    private resetSelected() {
        this.selectedInfo = "";
        this.selectedOperation = ""
    }


    constructor() { }

    ngOnInit() { }
}