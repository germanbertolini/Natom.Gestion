export class PedidoDetalleDTO {
    public encrypted_id: string;
    public pedido_encrypted_id: string;
    public producto_encrypted_id: string;
    public producto_descripcion: string;
    public producto_peso_gramos: number;
    public cantidad: number;
    public entregado: number;
    public deposito_encrypted_id: string;
    public deposito_descripcion: string;
    public precio_lista_encrypted_id: string;
    public precio_descripcion: string;
    public precio: number;
}