import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { PlaceDTO } from "src/app/classes/dto/place.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { DataTableDTO } from "../../../../classes/data-table-dto";

@Component({
  selector: 'app-place-crud',
  styleUrls: ['./place-crud.component.css'],
  templateUrl: './place-crud.component.html'
})

export class PlaceCrudComponent implements OnInit {

  crud: CRUDView<PlaceDTO>;
  clienteId: string;

  constructor(private apiService: ApiService,
              private route: ActivatedRoute,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<PlaceDTO>(routeService);
    this.crud.model = new PlaceDTO();
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

    this.apiService.DoPOST<ApiResult<PlaceDTO>>("places/save?encryptedId=" + encodeURIComponent(this.clienteId), this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Planta / Oficina guardada correctamente.');
          this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/places']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("places/basics/data" + (this.crud.isEditMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          if (response.data.entity !== null)
            this.crud.model = response.data.entity;

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
