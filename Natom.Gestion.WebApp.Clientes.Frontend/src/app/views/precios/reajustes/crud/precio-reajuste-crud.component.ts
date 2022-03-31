import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { MarcaDTO } from "src/app/classes/dto/marca.dto";
import { ListaDePreciosDTO } from "src/app/classes/dto/precios/lista-de-precios.dto";
import { PrecioReajusteDTO } from "src/app/classes/dto/precios/precio-reajuste.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { DataTableDTO } from "../../../../classes/data-table-dto";

@Component({
  selector: 'app-precio-reajuste-crud',
  styleUrls: ['./precio-reajuste-crud.component.css'],
  templateUrl: './precio-reajuste-crud.component.html'
})

export class PrecioReajusteCrudComponent implements OnInit {

  crud: CRUDView<PrecioReajusteDTO>;
  ListasDePrecios: Array<ListaDePreciosDTO>;
  Marcas: Array<MarcaDTO>;
  tipoReajuste: string;


  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.tipoReajuste = "";
    this.crud = new CRUDView<PrecioReajusteDTO>(routeService);
    this.crud.model = new PrecioReajusteDTO();
    this.crud.model.aplicoListaDePrecios_encrypted_id = "";
    this.crud.model.aplicoMarca_encrypted_id = "";
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.aplicoMarca_encrypted_id === undefined || this.crud.model.aplicoMarca_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar una Marca.");
      return;
    }

    if (this.crud.model.aplicoListaDePrecios_encrypted_id === undefined || this.crud.model.aplicoListaDePrecios_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar una Lista de precios.");
      return;
    }

    if (this.tipoReajuste === undefined || this.tipoReajuste.length <= 0)
    {
      this.confirmDialogService.showError("Debes seleccionar un Tipo de reajuste.");
      return;
    }

    if (this.crud.model.valor === undefined || this.crud.model.valor <= 0)
    {
      this.confirmDialogService.showError("Debes ingresar un Valor de reajuste válido.");
      return;
    }

    switch (this.tipoReajuste)
    {
      case "1":
        this.crud.model.esIncremento = true;
        this.crud.model.esPorcentual = true;
        break;
      case "2":
        this.crud.model.esIncremento = true;
        this.crud.model.esPorcentual = false;
        break;
      case "3":
        this.crud.model.esIncremento = false;
        this.crud.model.esPorcentual = true;
        break;
      case "4":
        this.crud.model.esIncremento = false;
        this.crud.model.esPorcentual = false;
        break;
    }

    this.apiService.DoPOST<ApiResult<PrecioReajusteDTO>>("precios/reajustes/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Reajuste aplicado correctamente.');
    this.routerService.navigate(['/precios/reajustes']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("precios/reajustes/basics/data" + (this.crud.isRenewMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          if (response.data.entity !== null)
            this.crud.model = response.data.entity;
          
          this.ListasDePrecios = <Array<ListaDePreciosDTO>>response.data.listasDePrecios;
          this.Marcas = <Array<MarcaDTO>>response.data.marcas;

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
