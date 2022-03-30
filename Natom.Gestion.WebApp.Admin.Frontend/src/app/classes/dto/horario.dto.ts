export class HorarioDTO {
    public encrypted_id: string;
    public encrypted_place_id: string;
    public configuro_fecha_hora: Date;
    public ingreso_tolerancia_mins: number;
    public egreso_tolerancia_mins: number;
    public almuerzo_horario_desde: string;
    public almuerzo_horario_hasta: string;
    public almuerzo_tiempo_limite_mins: number;
    public aplica_desde: Date;
    public aplica_hasta: Date;
    public status: string;
}