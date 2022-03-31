export class StockListDTO {
    public encrypted_id: string;
    public deposito: string;
    public producto: string;
    public tipo: string;
    public movido: number;
    public reservado: number;
    public stock: number;
    public fechaHora: Date;
    public fechaHoraControlado: Date;
    public observaciones: string;
}