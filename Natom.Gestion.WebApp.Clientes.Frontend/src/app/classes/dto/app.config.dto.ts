import { NegocioConfigDTO } from "./negocio/negocio.config.dto";
import { FeatureFlagsDTO } from "./_feature_flags/feature-flags.dto";

export class AppConfigDTO {
    public feature_flags: FeatureFlagsDTO;
    public negocio_config: NegocioConfigDTO;
}