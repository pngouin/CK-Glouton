import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class LogService {

    private logEndpoint: string = '/api/log';

    constructor(
        private httpClient: HttpClient
    ) {
    }

    public getAllApplicationName(): Observable<string[]> {
        return this.httpClient
            .get<string[]>(`${this.logEndpoint}/app`);
    }

}