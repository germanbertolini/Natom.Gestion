import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { MarcaDTO } from "src/app/classes/dto/marca.dto";
import { ListaDePreciosDTO } from "src/app/classes/dto/precios/lista-de-precios.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { DataTableDTO } from "../../../classes/data-table-dto";

@Component({
  selector: 'app-lista-de-precios-crud',
  styleUrls: ['./lista-de-precios-crud.component.css'],
  templateUrl: './lista-de-precios-crud.component.html'
})

export class ListaDePreciosCrudComponent implements OnInit {

  crud: CRUDView<ListaDePreciosDTO>;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<ListaDePreciosDTO>(routeService);
    this.crud.model = new ListaDePreciosDTO();
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.descripcion === undefined || this.crud.model.descripcion.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar un Nombre / Descripción.");
      return;
    }

    this.apiService.DoPOST<ApiResult<MarcaDTO>>("listasdeprecios/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Lista de precios guardada correctamente.');
          this.routerService.navigate(['/listas-de-precios']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("listasdeprecios/basics/data" + (this.crud.isEditMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          if (response.data.entity !== null)
            this.crud.model = response.data.entity;

            setTimeout(function() {
              (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
            }, 300);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
    
  }

}
