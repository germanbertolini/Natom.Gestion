import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { DataTableDTO } from "../../../../classes/data-table-dto";
import { GoalDTO } from "src/app/classes/dto/goal.dto";
import { PlaceDTO } from "src/app/classes/dto/place.dto";
import { HorarioDTO } from "src/app/classes/dto/horario.dto";
import { DatePipe } from "@angular/common";

@Component({
  selector: 'app-horario-crud',
  styleUrls: ['./horario-crud.component.css'],
  templateUrl: './horario-crud.component.html'
})

export class HorarioCrudComponent implements OnInit {

  crud: CRUDView<HorarioDTO>;
  Places: Array<PlaceDTO>;
  aplica_fecha_desde: string;
  clienteId: string;

  constructor(private apiService: ApiService,
              private route: ActivatedRoute,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.crud = new CRUDView<HorarioDTO>(routeService);
    this.crud.model = new HorarioDTO();
    this.crud.model.encrypted_place_id = decodeURIComponent(this.route.snapshot.paramMap.get('place_id'));
    this.clienteId = decodeURIComponent(this.route.snapshot.paramMap.get('id_cliente'));
  }

  decideClosure(event, datepicker) { const path = event.path.map(p => p.localName); if (!path.includes('ngb-datepicker')) { datepicker.close(); } }

  onFechaAplicaDesdeChange(newValue) {
    this.crud.model.aplica_desde = new Date(newValue.year, newValue.month - 1, newValue.day, 0, 0, 0, 0);
  }

  fixTimeFormat(time: string): string {
    if (time === undefined || time === null || (time !== null && time.length === 0))
      return null;

    let parts = time.split(':');
    let final = parts[0].substring(0, 2).padStart(2, '0');
    if (parts.length > 1) {
      parts[1] = parts[1].substring(0, 2).padStart(2, '0');
      final = final + ":" + parts[1];
    }
    else {
      final = final + ":00";
    }
    return final;
  }

  getTimeNumber(time:string): number {
    let parts = time.split(':');
    return parseInt(parts[0]) * 60 + parseInt(parts[1]);
  }

  onCancelClick() {
    if (!this.crud.isNewMode)
      window.history.back();
    else {
      this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
        window.history.back();
      });
    }
  }

  onSaveClick() {
    this.crud.model.encrypted_place_id = decodeURIComponent(this.route.snapshot.paramMap.get('place_id'));

    this.crud.model.almuerzo_horario_desde = this.fixTimeFormat(this.crud.model.almuerzo_horario_desde);
    this.crud.model.almuerzo_horario_hasta = this.fixTimeFormat(this.crud.model.almuerzo_horario_hasta);
    
    if (this.aplica_fecha_desde === undefined || this.aplica_fecha_desde === null || this.aplica_fecha_desde.length === 0)
    {
      this.confirmDialogService.showError("Debes indicar 'Aplica desde'.");
      return;
    }

    if (this.crud.model.ingreso_tolerancia_mins === undefined || this.crud.model.ingreso_tolerancia_mins === null)
    {
      this.confirmDialogService.showError("Fichado: Debes indicar 'Tolerancia en INGRESO'.");
      return;
    }

    if (this.crud.model.ingreso_tolerancia_mins < 0)
    {
      this.confirmDialogService.showError("Fichado: 'Tolerancia en INGRESO' no puede ser inferior a CERO.");
      return;
    }

    if (this.crud.model.egreso_tolerancia_mins === undefined || this.crud.model.egreso_tolerancia_mins === null)
    {
      this.confirmDialogService.showError("Fichado: Debes indicar 'Tolerancia en EGRESO'.");
      return;
    }

    if (this.crud.model.egreso_tolerancia_mins < 0)
    {
      this.confirmDialogService.showError("Fichado: 'Tolerancia en EGRESO' no puede ser inferior a CERO.");
      return;
    }

    if (this.crud.model.almuerzo_horario_desde === undefined || this.crud.model.almuerzo_horario_desde.length === 0)
    {
      this.confirmDialogService.showError("Almuerzo: Debes indicar 'Rango horario DESDE'.");
      return;
    }

    if (this.crud.model.almuerzo_horario_hasta === undefined || this.crud.model.almuerzo_horario_hasta.length === 0)
    {
      this.confirmDialogService.showError("Almuerzo: Debes indicar 'Rango horario HASTA'.");
      return;
    }

    this.apiService.DoPOST<ApiResult<GoalDTO>>("horarios/save?encryptedClientId=" + encodeURIComponent(this.clienteId), this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Horario / Tolerancias guardado correctamente.');
          this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/horarios/' + encodeURIComponent(this.crud.model.encrypted_place_id)]);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("horarios/basics/data?encryptedPlaceId=" + encodeURIComponent(this.crud.model.encrypted_place_id) + "&encryptedClientId=" + encodeURIComponent(this.clienteId) + (!this.crud.isNewMode ? "&encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          if (response.data !== null) {
            this.crud.model = response.data;
          }
          
          
          setTimeout(function() {
            (<any>$('[data-toggle="tooltip"]')).tooltip();
          }, 300);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
    
    
  }

}
