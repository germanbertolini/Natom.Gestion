import { PedidoDetalleDTO } from "./pedido-detalle.dto";

export class PedidoDTO {
    public encrypted_id: string;
    public numero: string;
    public fechaHora: Date;
    public cliente_encrypted_id: string;
    public retira_personalmente: boolean;
    public entrega_estimada_fecha: Date;
    public entrega_estimada_rango_horario_encrypted_id: string;
    public entrega_observaciones: string;
    public entrega_domicilio: string;
    public entrega_entre_calles: string;
    public entrega_localidad: string;
    public entrega_telefono1: string;
    public entrega_telefono2: string;
    public entregado: boolean;
    public usuario: string;
    public numero_remito: string;
    public medio_de_pago: string;
    public observaciones: string;

    public detalle: PedidoDetalleDTO[];
}