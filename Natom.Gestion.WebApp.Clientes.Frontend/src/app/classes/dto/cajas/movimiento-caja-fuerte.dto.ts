export class MovimientoCajaFuerteDTO {
    public encrypted_id: string;
    public fechaHora: Date;
    public usuarioNombre: String;
    public tipo: String;
    public medio_de_pago: string;
    public pago_referencia: string;
    public importe: number;
    public proveedor_encrypted_id: string;
    public observaciones: String;
    public esCtaCte: boolean;
    public esCheque: boolean;
}