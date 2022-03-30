export class UserDTO {
    public encrypted_id: string;
    public cliente_encrypted_id: string;
    public first_name: string;
    public last_name: string;
    public picture_url: string;
    public email: string;
    public state: string;
    public registered_at: Date;
    public business_name: string;
    public business_role_name: string;
    public country_icon: string;
    public permisos: string[];
    public me: boolean;
}