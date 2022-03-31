export class VentaDetalleDTO {
    public encrypted_id: string;
    public pedido_encrypted_id: string;
    public pedido_detalle_encrypted_id: string;
    public pedido_numero: string;
    public producto_encrypted_id: string;
    public producto_descripcion: string;
    public producto_peso_gramos: number;
    public cantidad: number;
    public cantidadPedido: number;
    public deposito_encrypted_id: string;
    public deposito_descripcion: string;
    public precio_lista_encrypted_id: string;
    public precio_descripcion: string;
    public precio: number;
    public numero_remito: string;
}