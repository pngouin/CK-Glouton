import { Title } from '@angular/platform-browser/src/browser/title';

export interface IStatisticsData {
    totalLogCount: number,
    totalExceptionCount: number,
    totalAppNameCount: number,
    logCountByAppName: { [index: string]: number },
    exceptionCountByAppName: { [index: string]: number },
    appNames: string[]
}

export class StatisticsData implements IStatisticsData {
    totalLogCount: number;
    totalExceptionCount: number;
    totalAppNameCount: number;
    logCountByAppName: { [index: string]: number; };
    exceptionCountByAppName: { [index: string]: number; };
    appNames: string[];
}

export interface IChartData {
    data: {
        labels: string[],
        datasets: {
            data: number[],
            backgroundColor: string[]
        }[]
    },
    option? : {
        responsive? : boolean,
        title? : {
            display? : boolean,
            position? : string,
            padding? : number,
            fontSize? : number,
            text? : string
        },
        legend? : {
            display? : boolean,
            position? : string, 
            reverse? : boolean
        }
    }
}
