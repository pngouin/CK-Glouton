import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IAlertExpressionModel } from 'app/modules/ifttt/models/sender.model';
import { IAlertData } from 'app/modules/ifttt/models/expression.model';

@Injectable()
export class IftttService {

    endpoint: string = '/api/alert';

    constructor(private httpClient: HttpClient) { }

    getAvaibleConfiguration (): Observable<string[]> {
        return this.httpClient.get<string[]>(`${this.endpoint}/configuration`);
    }

    getConfiguration (configurationName : string ): Observable<Object> {
        return this.httpClient.get<Object>(`${this.endpoint}/configuration/${configurationName}`);
    }

    sendAlert ( expressionModel : IAlertExpressionModel): Observable<void> {
        return this.httpClient.post<void>(`${this.endpoint}/add`, expressionModel);
    }

    getAlert (): Observable<IAlertData[]> {
        return this.httpClient.get<IAlertData[]>(`${this.endpoint}/all`);
    }
}