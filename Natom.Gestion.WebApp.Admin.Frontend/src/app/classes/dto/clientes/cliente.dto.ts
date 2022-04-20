import { ClienteMontoDTO } from "./cliente.monto.dto";

export class ClienteDTO {
    public encrypted_id: string;
	public nombre: string;
	public apellido: string;
	public razonSocial: string;
	public nombreFantasia: string;
	public tipoDocumento_encrypted_id: string;
	public tipoDocumento: string;
	public zona_encrypted_id: string;
	public zona: string;
	public numeroDocumento: string;
	public domicilio: string;
	public entreCalles: string;
	public localidad: string;
	public esEmpresa: boolean;
	public contactoTelefono1: string;
	public contactoTelefono2: string;
	public contactoEmail1: string;
	public contactoEmail2: string;
	public contactoObservaciones: string;
	public montos: Array<ClienteMontoDTO>;
}