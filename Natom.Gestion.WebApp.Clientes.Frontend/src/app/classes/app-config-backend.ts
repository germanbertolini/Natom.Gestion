import { AppConfigDTO } from "./dto/app.config.dto";
import { NegocioConfigDTO } from "./dto/negocio/negocio.config.dto";

export class AppConfigBackend {
    public current: AppConfigDTO;

    constructor() {
        this.current = new AppConfigDTO();
        this.current.negocio_config = new NegocioConfigDTO();
        this.current.negocio_config.logo_base64 = "";
    }
}