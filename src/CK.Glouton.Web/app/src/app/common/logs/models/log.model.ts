export enum LogType {
    OpenGroup,
    Line,
    CloseGroup
}

export interface ILogViewModel {
    logType: LogType;
    logTime: string;
    text: string;
    tags: string;
    sourceFileName: string;
    lineNumber: string;
    exception: IExceptionViewModel;
    logLevel:string;
    monitorId: string;
    groupDepth: string;
    previousEntryType: string;
    previousLogTime: string;
    appName: string;
    conclusion: string;
}

export interface IInnerExceptionViewModel {
    stackTrace: string;
    message: string;
    fileName: string;
}

export interface IExceptionViewModel {
    innerException: IInnerExceptionViewModel;
    aggregatedExceptions: IExceptionViewModel[];
    message: string;
    stackTrace: string;
}