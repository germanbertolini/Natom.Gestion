export class ProductoDTO {
    public encrypted_id: string;
	public marca_encrypted_id: string;
	public codigo: string;
	public descripcionCorta: string;
	public descripcionLarga: string;
	public unidadPeso_encrypted_id: string;
	public categoria_encrypted_id: string;
	public pesoUnitario: number;
	public costo_unitario: number;
	public mueveStock: boolean;
}