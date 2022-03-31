export class MovimientoStockDTO {
    public deposito_encrypted_id: string;
    public producto_encrypted_id: string;
    public tipo: string;
    public cantidad: number;
    public usuario_encrypted_id: string;
    public usuarioNombre: string;
    public observaciones: string;
    public esCompra: boolean;
    public proveedor_encrypted_id: string;
    public costoUnitario: number;
}