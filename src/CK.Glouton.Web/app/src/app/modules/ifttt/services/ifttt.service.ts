import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IAlertExpressionModel } from 'app/modules/ifttt/models/sender.model';

@Injectable()
export class IftttService {

    endpoint: string = "/api/alert";

    constructor(private httpClient: HttpClient) { }

    getAvaibleConfiguration () : Observable<string[]> {
        return this.httpClient.get<string[]>(`${this.endpoint}/configuration`);
    }

    getConfiguration (configurationName : string ) : Observable<Object> {
        return this.httpClient.get<Object>(`${this.endpoint}/configuration/${configurationName}`);
    }
    
    sendAlert ( expressionModel : IAlertExpressionModel) : Observable<Object> {
        return this.httpClient.post<Object>(`${this.endpoint}/add`, expressionModel);
    }
}