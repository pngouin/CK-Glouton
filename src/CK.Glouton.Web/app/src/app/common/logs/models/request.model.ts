export interface ISearchParameters {
    monitorId?: string;
    appName: string[];
    from?: Date;
    to?: Date;
    fields?: string[];
    keyword?: string;
    logLevel?: string[];
}