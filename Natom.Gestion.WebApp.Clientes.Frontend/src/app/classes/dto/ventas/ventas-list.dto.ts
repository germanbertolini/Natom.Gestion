export class VentasListDTO {
    public encrypted_id: string;
    public numero: string;
    public numeroVenta: string;
    public remitos: Array<string>;
    public pedidos: Array<string>;
    public factura: string;
    public cliente: string;
    public fechaHora: Date;
    public peso_total_gramos: number;
    public usuario: string;
    public anulado: boolean;
    public medio_de_pago: string;
}