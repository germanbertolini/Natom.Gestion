import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { AppConfig } from "../classes/app-config";

@Injectable({
    providedIn: 'root'
})
export class JsonAppConfigService extends AppConfig {
    constructor(private http: HttpClient) {
        super();
    }

    //Esta función necesita retornar un promise
    load() {
        return this.http.get<AppConfig>('assets/app.config.json')
                .toPromise()
                .then(data => {
                    this.baseURL = data.baseURL;
                })
                .catch(() => {
                    console.error("No se pudo obtener el app.config.json");
                });
    }
}