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

@Component({
  selector: 'app-goal-crud',
  styleUrls: ['./goal-crud.component.css'],
  templateUrl: './goal-crud.component.html'
})

export class GoalCrudComponent implements OnInit {

  crud: CRUDView<GoalDTO>;
  Places: Array<PlaceDTO>;
  clienteId: string;

  constructor(private apiService: ApiService,
              private route: ActivatedRoute,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.crud = new CRUDView<GoalDTO>(routeService);
    this.crud.model = new GoalDTO();
    this.crud.model.place_encrypted_id = "";
    this.crud.model.place_encrypted_id = decodeURIComponent(this.route.snapshot.paramMap.get('place_id'));
    this.clienteId = decodeURIComponent(this.route.snapshot.paramMap.get('id_cliente'));
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.name === undefined || this.crud.model.name.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar un Nombre.");
      return;
    }

    if (this.crud.model.address === undefined || this.crud.model.address.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar una Dirección.");
      return;
    }

    if (this.crud.model.place_encrypted_id === undefined || this.crud.model.place_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar una Planta / Oficina.");
      return;
    }

    this.apiService.DoPOST<ApiResult<GoalDTO>>("goals/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Portería / Acceso guardado correctamente.');
          this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/goals/' + encodeURIComponent(this.crud.model.place_encrypted_id)]);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("goals/basics/data?clienteEncryptedId=" + encodeURIComponent(this.clienteId) + (this.crud.isEditMode ? "&encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          if (response.data.entity !== null)
            this.crud.model = response.data.entity;

            this.Places = <Array<PlaceDTO>>response.data.places;

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
