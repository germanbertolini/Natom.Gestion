export class PrecioListDTO {
    public encrypted_id: string;
	public producto: string;
	public precio: number;
    public listaDePrecios: string;
    public aplicaDesdeFechaHora: Date;
    public aplicaDesdeDias: number;
    public esPorcentual: boolean;
}