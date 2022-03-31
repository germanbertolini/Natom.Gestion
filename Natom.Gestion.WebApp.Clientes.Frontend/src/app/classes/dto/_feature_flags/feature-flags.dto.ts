import { FeatureFlagsAccesoDTO } from "./feature-flags.acceso.dto";
import { FeatureFlagsClientesDTO } from "./feature-flags.clientes.dto";
import { FeatureFlagsPedidosDTO } from "./feature-flags.pedidos.dto";
import { FeatureFlagsStockDTO } from "./feature-flags.stock.dto";

export class FeatureFlagsDTO {
    public acceso: FeatureFlagsAccesoDTO;
    public clientes: FeatureFlagsClientesDTO;
    public stock: FeatureFlagsStockDTO;
    public pedidos: FeatureFlagsPedidosDTO;
}