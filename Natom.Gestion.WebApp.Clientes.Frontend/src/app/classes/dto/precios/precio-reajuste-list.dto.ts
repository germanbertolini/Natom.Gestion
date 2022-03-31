export class PrecioReajusteListDTO {
    public encrypted_id: string;
	public usuario: string;
	public tipoReajuste: string;
    public esPorcentual: boolean;
	public valorReajuste: number;
    public aplicoMarca: string;
    public aplicoListaDePrecios: string;
    public aplicaDesdeFechaHora: Date;
}