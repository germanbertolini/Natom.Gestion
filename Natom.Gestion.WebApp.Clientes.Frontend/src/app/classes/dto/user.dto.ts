import { PermisoDTO } from "./permiso.dto";

export class UserDTO {
    public encrypted_id: string;
    public first_name: string;
    public last_name: string;
    public picture_url: string;
    public email: string;
    public status: string;
    public registered_at: Date;
    public permisos: string[];
}