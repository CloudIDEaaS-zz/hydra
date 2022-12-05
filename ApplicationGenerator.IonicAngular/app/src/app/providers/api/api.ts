import { HttpClient, HttpParams, HttpHeaders, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

/**
 * Api is a generic REST Api handler. Set your API url first.
 */
@Injectable()
export class Api {
    baseUrl: string = "http://localhost:39049";
    servicesUrl: string = this.baseUrl + "/Services/api";
    public accessToken: string;

  constructor(public http: HttpClient) {
  }

  private addToken(headers : HttpHeaders): HttpHeaders {

    if (this.accessToken) {
      headers = headers.append("Authorization", "Bearer " + this.accessToken);
    }

    return headers;
  }

  getHeaders(): HttpHeaders
  {
    var headers = new HttpHeaders()
      .append('Access-Control-Allow-Origin', 'http://localhost:8100')
      .append('Access-Control-Allow-Methods', 'GET');

    headers = this.addToken(headers);

    return headers;
  }

  authenticate(endpoint: string, accountInfo: { userName: string, password: string }, authorizationBasic: string) {

    let headers = new HttpHeaders();
    let data = "grant_type=password&username=" + accountInfo.userName + "&password=" + accountInfo.password;

    headers = headers.append("Authorization", "Basic " + authorizationBasic);
    headers = headers.set("Content-Type", "application/x-www-form-urlencoded");

    return this.http.post(this.baseUrl + '/' + endpoint, data, { headers: headers });
  }

  getRaw(endpoint: string, params?: any, reqOpts?: any) {
    if (!reqOpts) {
      reqOpts = this.getHeaders();
    }

    // Support easy query params for GET requests
    if (reqOpts.params) {
      reqOpts.params = new HttpParams();
      for (let k in params) {
        reqOpts.params = reqOpts.params.set(k, params[k]);
      }
    }

    return this.http.get(this.servicesUrl + '/' + endpoint, reqOpts);
  }

  get<T>(endpoint: string, params?: any, reqOpts?: any) : Observable<T> {
    if (!reqOpts) {
      reqOpts = this.getHeaders();
    }

    // Support easy query params for GET requests
    if (reqOpts.params) {
      reqOpts.params = new HttpParams();
      for (let k in params) {
        reqOpts.params = reqOpts.params.set(k, params[k]);
      }
    }

    return this.http.get<T>(this.servicesUrl + '/' + endpoint, {
      headers: reqOpts
    });
  }
  
  post(endpoint: string, body: any, reqOpts?: any) {
    if (!reqOpts) {
      reqOpts = this.getHeaders();
    }

    return this.http.post(this.servicesUrl + '/' + endpoint, body, reqOpts);
  }

  put(endpoint: string, body: any, reqOpts?: any) {
    if (!reqOpts) {
      reqOpts = this.getHeaders();
    }

    return this.http.put(this.servicesUrl + '/' + endpoint, body, reqOpts);
  }

  delete(endpoint: string, reqOpts?: any) {
    if (!reqOpts) {
      reqOpts = this.getHeaders();
    }

    return this.http.delete(this.servicesUrl + '/' + endpoint, reqOpts);
  }

  patch(endpoint: string, body: any, reqOpts?: any) {
    if (!reqOpts) {
      reqOpts = this.getHeaders();
    }

    return this.http.patch(this.servicesUrl + '/' + endpoint, body, reqOpts);
  }
}
