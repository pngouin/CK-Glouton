export interface IStatisticsData {
    totalLogCount : number,
    totalExceptionCount: number,
    totalAppNameCount: number,
    logCountByAppName : {[index: string] : number},
    exceptionCountByAppName : {[index: string] : number},
    appNames : string[]
}

export class StatisticsData implements IStatisticsData {
    totalLogCount: number;
    totalExceptionCount: number;
    totalAppNameCount: number;
    logCountByAppName: { [index: string]: number; };
    exceptionCountByAppName: { [index: string]: number; };
    appNames: string[];
}