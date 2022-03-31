import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { ApiResult } from "../classes/dto/shared/api-result.dto";
import { FeatureFlagsDTO } from "../classes/dto/_feature_flags/feature-flags.dto";
import { FeatureFlags } from "../classes/feature-flags";
import { JsonAppConfigService } from "./json-app-config.service";

@Injectable({
    providedIn: 'root'
})
export class FeatureFlagsService extends FeatureFlags {
    constructor(private http: HttpClient,
                private jsonAppConfig: JsonAppConfigService) {
        super();
    }

    //Esta función necesita retornar un promise
    load() {
        return this.http.get<ApiResult<FeatureFlagsDTO>>(this.jsonAppConfig.baseURL + 'auth/feature_flags')
                .toPromise()
                .then(data => {
                    this.current = data.data;
                })
                .catch(() => {
                    console.error("No se pudo obtener el json para FeatureFlagsService");
                });
    }
}