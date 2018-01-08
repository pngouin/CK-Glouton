import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { ILogViewModel, ISearchParameters } from '../models';

@Injectable()
export class LogService {

    private logEndpoint: string = '/api/log';

    constructor(
        private httpClient: HttpClient
    ) {
    }

    public getAll(appName: string): Observable<ILogViewModel[]> {
        if(appName === undefined || appName === null) {
            throw new Error('Param cannot be null!');
        }

        return this.httpClient
            .get<ILogViewModel[]>(`${this.logEndpoint}/all/${appName}`);
    }

    public getAllApplicationName(): Observable<string[]> {
        return this.httpClient
            .get<string[]>(`${this.logEndpoint}/appName`);
    }

    public getAllMonitorId(): Observable<string[]> {
        return this.httpClient
            .get<string[]>(`${this.logEndpoint}/monitorId`);
    }

    public filter(searchParameters: ISearchParameters): Observable<ILogViewModel[]> {
        if(searchParameters === null || searchParameters === undefined) {
            throw new Error('Params cannot be null!');
        }

        let params: HttpParams = new HttpParams();
        params = this.appendStringIfDefined(params, 'monitorId', searchParameters.monitorId);
        params = this.appendStringArrayIfDefined(params, 'appName', searchParameters.appName);
        params = this.appendDateIfDefined(params, 'from', searchParameters.from);
        params = this.appendDateIfDefined(params, 'to', searchParameters.to);
        params = this.appendStringArrayIfDefined(params, 'fields', searchParameters.fields);
        params = this.appendStringIfDefined(params, 'keyword', searchParameters.keyword);
        params = this.appendStringArrayIfDefined(params, 'logLevel', searchParameters.logLevel);
        params = this.appendNumberIfDefined(params, 'groupDepth', searchParameters.groupDepth);

        return this.httpClient
            .get<ILogViewModel[]>(`${this.logEndpoint}/filter`, {params: params});
    }

    public search(appName: string, query: string): Observable<ILogViewModel[]> {
        if(appName === undefined || appName === null) {
            throw new Error('Param cannot be null!');
        }

        return this.httpClient
            .get<ILogViewModel[]>(
                `${this.logEndpoint}/search/${appName}`,
                {params: this.appendStringIfDefined( new HttpParams(), 'query', query )}
            );
    }

    private appendStringIfDefined( params: HttpParams, key: string, value: string ): HttpParams {
        if(key === undefined || key === null) { return params; }
        if(value === undefined || value === null) { return params; }
        return params.append(key, value);
    }

    private appendStringArrayIfDefined( params: HttpParams, key: string, array: string[] ): HttpParams {
        if(key === undefined || key === null) { return params; }
        if(array === undefined || array === null) { return params; }
        array.forEach(e => params = this.appendStringIfDefined(params, key, e));
        return params;
    }

    private appendNumberIfDefined( params: HttpParams, key: string, value: number ): HttpParams {
        if(key === undefined || key === null) { return params; }
        if(value === undefined || value === null) { return params; }
        return params.append(key, value.toString());
    }

    private appendDateIfDefined( params: HttpParams, key: string, value: Date ): HttpParams {
        if(key === undefined || key === null) { return params; }
        if(value === undefined || value === null) { return params; }
        if(typeof value === 'string') { return params.append(key, value); }
        return params.append(key, value.toISOString());
    }
}