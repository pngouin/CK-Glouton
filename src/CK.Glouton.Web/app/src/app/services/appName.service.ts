import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

const API_PATH: string = "/api/appname";

@Injectable()
export class AppNameService {

    private _appName : string

    constructor(private httpClient: HttpClient) { }
    
    public GetSelected() : string {
        return this._appName;
    }

    public GetAll() : Observable<string[]> {
        return this.httpClient.get<string[]>(API_PATH);
    }

}