import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class IftttService {

    endpoint: string = "/api/alert";

    constructor(private httpClient: HttpClient) { }

    getAvaibleConfiguration () : Observable<string[]> {
        return this.httpClient.get<string[]>(`${this.endpoint}/configuration`);
    }

    getConfiguration (configurationName : string ) : Observable<Object> {
        return this.httpClient.get<string[]>(`${this.endpoint}/configuration/${configurationName}`);
    }
    
}