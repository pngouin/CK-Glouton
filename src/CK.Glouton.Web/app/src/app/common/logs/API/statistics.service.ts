import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class StatisticsService {

    private statistics : string = "/api/stats";

    constructor(private httpClient: HttpClient) { }
    
    public getTotalLogCount(): Observable<number> {
        return this.httpClient
            .get<number>(`${this.statistics}/log/total`);
    }

    public getTotalExceptionCount(): Observable<number> {
        return this.httpClient
            .get<number>(`${this.statistics}/exception/total`);
    }

    public getAppNameCount(): Observable<number> {
        return this.httpClient
            .get<number>(`${this.statistics}/log/total`);
    }

    public getAppNames(): Observable<string[]> {
        return this.httpClient
            .get<string[]>(`${this.statistics}/appnames`);
    }

    public getLogCountByAppName(): Observable<{[index:string] : number}> {
        return this.httpClient
            .get<{[index:string] : number}>(`${this.statistics}/log/total/by/appname`);
    }

    public getExceptionCountByAppName(): Observable<{[index:string] : number}> {
        return this.httpClient
            .get<{[index:string] : number}>(`${this.statistics}/exception/total/by/appname`);
    }
}