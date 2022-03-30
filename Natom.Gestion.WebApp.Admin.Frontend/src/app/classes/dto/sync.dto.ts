export class SyncDTO {
    public encrypted_instance_id: string;
    public installation_alias: string;
    public installed_by: string;
    public installed_at: Date;
    public activated_at: Date;
    public activo: boolean;
    public last_sync_at: Date;
    public devices_count: number;
}