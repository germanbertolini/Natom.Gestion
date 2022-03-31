export class PrecioReajusteDTO {
    public encrypted_id: string;
	public usuario: string;
	public esIncremento: boolean;
	public esPorcentual: boolean;
	public valor: number;
	public aplicoMarca_encrypted_id: string;
	public aplicoListaDePrecios_encrypted_id: string;
}