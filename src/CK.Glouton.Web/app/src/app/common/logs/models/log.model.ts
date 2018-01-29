export enum LogType {
    OpenGroup,
    Line,
    CloseGroup
}

export interface ILogViewModel {
    logType: LogType;
    logTime: Date;
    text: string;
    tags: string;
    sourceFileName: string;
    lineNumber: string;
    exception: IExceptionViewModel;
    logLevel:string;
    monitorId: string;
    groupDepth: number;
    previousEntryType: string;
    previousLogTime: Date;
    appName: string;
    conclusion: string;
    children: ILogViewModel[];
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