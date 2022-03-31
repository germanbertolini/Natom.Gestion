import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { CookieService } from "ngx-cookie-service";
import { Observable } from "rxjs";
import { SpinnerLoadingService } from "../components/spinner-loading/spinner-loading.service";
import { JsonAppConfigService } from "./json-app-config.service";

@Injectable({
    providedIn: 'root'
  })
export class ApiService {
    onForbidden: () => void = null;

    constructor(private jsonAppConfig: JsonAppConfigService,
                private httpClient: HttpClient,
                private cookieService: CookieService,
                private loadingService: SpinnerLoadingService) {

    }

    public SetOnForbiddenAction(onForbiddenAction: () => void) {
        this.onForbidden = onForbiddenAction;
    }

    public OpenNewTab(relativeUrl: string) {
        window.open(this.jsonAppConfig.baseURL + relativeUrl, "_blank");
    }

    public DoGETWithObservable(relativeUrl: string, headers: HttpHeaders = null) : Observable<Object> {
        headers = this.SetAPIHeaders(headers);
        return this.httpClient.get(this.jsonAppConfig.baseURL + relativeUrl, { headers: headers });
    }
    
    public DoGET<TResponse>(relativeUrl: string, headers: HttpHeaders = null, onSuccess: (response: TResponse) => void, onError: (errorMessage: string) => void) {
        this.loadingService.show();
        headers = this.SetAPIHeaders(headers);
        this.DoGETWithObservable(relativeUrl, headers)
                .subscribe({
                    next: response => {
                        onSuccess(<TResponse>response);
                        this.loadingService.hide();
                    },
                    error: error => {
                        if (error.status === 403 && this.onForbidden !== null) {
                            this.onForbidden();
                        }
                        else {
                            onError(error.message);
                        }
                        this.loadingService.hide();
                    }
                });
    }

    public DoHiddenGET<TResponse>(relativeUrl: string, headers: HttpHeaders = null, onSuccess: (response: TResponse) => void, onError: (errorMessage: string) => void) {
        headers = this.SetAPIHeaders(headers);
        this.DoGETWithObservable(relativeUrl, headers)
                .subscribe({
                    next: response => {
                        onSuccess(<TResponse>response);
                    },
                    error: error => {
                        if (error.status === 403 && this.onForbidden !== null) {
                            this.onForbidden();
                        }
                        else {
                            onError(error.message);
                        }
                    }
                });
    }

    public DoPOST<TResponse>(relativeUrl: string, body: any, headers: HttpHeaders = null, onSuccess: (response: TResponse) => void, onError: (errorMessage: string) => void) {
        this.loadingService.show();
        headers = this.SetAPIHeaders(headers);
        this.httpClient
                .post(this.jsonAppConfig.baseURL + relativeUrl, body, { headers: headers })
                .subscribe({
                    next: response => {
                        onSuccess(<TResponse>response);
                        this.loadingService.hide();
                    },
                    error: error => {
                        if (error.status === 403 && this.onForbidden !== null) {
                            this.onForbidden();
                        }
                        else {
                            onError(error.message);
                        }
                        this.loadingService.hide();    
                    }
                });
    }

    public DoDELETE<TResponse>(relativeUrl: string, headers: HttpHeaders = null, onSuccess: (response: TResponse) => void, onError: (errorMessage: string) => void) {
        this.loadingService.show();
        headers = this.SetAPIHeaders(headers);
        this.httpClient
                .delete(this.jsonAppConfig.baseURL + relativeUrl, { headers: headers })
                .subscribe({
                    next: response => {
                        onSuccess(<TResponse>response);
                        this.loadingService.hide();
                    },
                    error: error => {
                        if (error.status === 403 && this.onForbidden !== null) {
                            this.onForbidden();
                        }
                        else {
                            onError(error.message);
                        }
                        this.loadingService.hide();   
                    }
                });
    }

    private SetAPIHeaders (headers: HttpHeaders) : HttpHeaders {
        if (headers === null)
            headers = new HttpHeaders();

        let token = atob(this.cookieService.get('Auth.Current.Token'));
        if (!headers.has("Authorization") && token !== "")
            headers = headers.append("Authorization", "Bearer " + token.substring(1, token.length - 1));

        return headers;
    }
}