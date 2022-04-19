import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ApiResult } from "../classes/dto/shared/api-result.dto";
import { AppConfigBackend } from "../classes/app-config-backend";
import { JsonAppConfigService } from "./json-app-config.service";
import { AppConfigDTO } from "../classes/dto/app.config.dto";
import { ApiService } from "./api.service";
import { CookieService } from "ngx-cookie-service";

@Injectable({
    providedIn: 'root'
})
export class AppConfigService extends AppConfigBackend {
    constructor(private cookieService: CookieService,
                private apiService: ApiService,
                private jsonAppConfig: JsonAppConfigService) {
        super();
    }

    //Esta función necesita retornar un promise
    load() {
        let cookieAuth = this.cookieService.get('Auth.Current.User');
        
        if (cookieAuth !== undefined && cookieAuth !== null && cookieAuth.length > 0)
            this.apiService.DoGET<ApiResult<AppConfigDTO>>("negocio/app/config", /*headers*/ null,
                                (response) => {
                                    if (!response.success) {
                                        console.log("No se pudo obtener AppConfig: " + response.message);
                                    }
                                    else {
                                        this.current = response.data;
                                    }
                                },
                                (errorMessage) => {
                                    console.log("No se pudo obtener AppConfig: " + errorMessage);
                                });
    }
}