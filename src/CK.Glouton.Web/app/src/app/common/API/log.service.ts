import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { ILogView } from '../logs/models';

@Injectable()
export class LogService {

    private logEndpoint: string = '/api/log';

    constructor(
        private httpClient: HttpClient
    ) {
    }

    public getAll(appName: string, max: number = 10): Observable<ILogView[]> {
        return this.httpClient
            .get<ILogView[]>(`${this.logEndpoint}/all/${appName}?max=${max}`);
    }

    public getAllApplicationName(): Observable<string[]> {
        return this.httpClient
            .get<string[]>(`${this.logEndpoint}/appName`);
    }

}