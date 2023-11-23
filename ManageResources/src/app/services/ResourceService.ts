import { Inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ResourceService {
  constructor(private http: HttpClient) {
  }

  headers = new HttpHeaders()
  .set('Name', 'Karthick')
  .set('Password', '1234');

  baseUrl = "https://localhost:5000/v1/resources/"

  public getAllResources(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}filter`, data, {
      headers: this.headers
    });
  }

  public getFullPaths(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}fullpaths`,  data, {
      headers: this.headers
    });
  }

  public getResourceById(id: any): Observable<any> {
    return this.http.get(`${this.baseUrl}getresource?resourceId=${id}`,{
      headers: this.headers
    });
  }

  public createResource(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}upload`,  data, {
      headers: this.headers
    });
  }

  public createResources(data: any): Observable<any> {
    return this.http.post(`${this.baseUrl}bulkupload`,  data, {
      headers: this.headers
    });
  }

  public deleteResource(id: any): Observable<any> {
    return this.http.delete(`${this.baseUrl}delete?resourceId=${id}`,{
      headers: this.headers
    });
  }

  public deleteResources(data: any): Observable<any> {
    return this.http.delete(`${this.baseUrl}bulkdelete`, {
      headers: this.headers,
      body: data
    });
  }

  public downloadResource(id: any) {
    return this.http.get(`${this.baseUrl}download?resourceId=${id}`, { 
    responseType: 'blob',
    headers: this.headers 
  });
  }

  public downloadResources(data: any) {
    return this.http.post(`${this.baseUrl}bulkdownload`, data, { 
      responseType: 'blob',
      headers: this.headers
    });
  }
}
