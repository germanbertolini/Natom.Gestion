export class PedidosListDTO {
    public encrypted_id: string;
    public numero: string;
    public venta_encrypted_id: string;
    public numeroVenta: string;
    public remito: string;
    public factura: string;
    public cliente: string;
    public fechaHora: Date;
    public peso_total_gramos: number;
    public usuario: string;
    public estado: string;
    public zona: string;
    public prepared: boolean;
    public fechaHoraPreparado: Date;
    public preparadoPor: string;
    public transporte: string;
    public enPreparacion: boolean;
    public anulado: boolean;
    public medio_de_pago: string;
}