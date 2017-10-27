export enum LogType {
    Opengroup,
    Line,
    CloseGroup
}

export interface ILogView {
    logType: LogType;
    exception: IExceptionViewModel;
    logTime: string;
    logLevel:string;
    monitorId: string;
    groupDepth: string;
    previousEntryType: string;
    previousLogTime: string;
    appId: string;
    sourceFileName: string;
    lineNumber: string;
}

export interface IInnerExceptionViewModel {
    stack: string;
    details: string;
    fileName: string;
}

export interface IExceptionViewModel {
    innerException: IInnerExceptionViewModel;
    message: string;
    stack: string;
}