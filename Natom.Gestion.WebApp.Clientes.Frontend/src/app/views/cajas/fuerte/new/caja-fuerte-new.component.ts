import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { fromEvent } from 'rxjs';
import { map, distinctUntilChanged, debounceTime, mergeMap } from 'rxjs/operators';
import { MovimientoCajaFuerteDTO } from "src/app/classes/dto/cajas/movimiento-caja-fuerte.dto";
import { ProveedorDTO } from "src/app/classes/dto/proveedores/proveedor.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { AutocompleteResultDTO } from "src/app/classes/dto/shared/autocomplete-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";

@Component({
  selector: 'app-caja-fuerte-new-crud',
  styleUrls: ['./caja-fuerte-new.component.css'],
  templateUrl: './caja-fuerte-new.component.html'
})

export class CajaFuerteNewComponent implements OnInit {
  crud: CRUDView<MovimientoCajaFuerteDTO>;
  proveedoresSearch: ProveedorDTO[];
  proveedor_search: string;
  proveedor_encrypted_id: string;
  proveedor_saldo_deudor: number;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<MovimientoCajaFuerteDTO>(routeService);
    this.crud.model = new MovimientoCajaFuerteDTO();
    this.crud.model.tipo = "";
    this.crud.model.medio_de_pago = "";
    this.crud.model.usuarioNombre = authService.getCurrentUser().first_name;
    this.proveedor_saldo_deudor = null;
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.tipo === "")
    {
      this.confirmDialogService.showError("Debes seleccionar el Tipo de movimiento.");
      return;
    }

    if (this.crud.model.importe === undefined)
    {
      this.confirmDialogService.showError("Debes ingresar el monto.");
      return;
    }

    if (this.crud.model.importe <= 0)
    {
      this.confirmDialogService.showError("Debes ingresar un monto válido.");
      return;
    }

    if (this.crud.model.esCtaCte) {
      if (this.crud.model.proveedor_encrypted_id === undefined || this.crud.model.proveedor_encrypted_id === null || this.crud.model.proveedor_encrypted_id.length === 0)
      {
        this.confirmDialogService.showError("Debes seleccionar el Proveedor a cancelar saldo.");
        return;
      }
    }

    if (this.crud.model.observaciones === undefined || this.crud.model.observaciones === "")
    {
      this.confirmDialogService.showError("Debes ingresar una observación.");
      return;
    }

    if (this.crud.model.medio_de_pago === undefined || this.crud.model.medio_de_pago === "")
    {
      this.confirmDialogService.showError("Debes seleccionar un medio de pago.");
      return;
    }

    this.apiService.DoPOST<ApiResult<MovimientoCajaFuerteDTO>>("cajas/fuerte/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Movimiento guardado correctamente.');
          this.routerService.navigate(['/cajas/fuerte']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  onProveedorSearchSelectItem (proveedor: ProveedorDTO) {
    this.proveedor_search = proveedor.tipoDocumento + " " + proveedor.numeroDocumento + " /// " + (proveedor.esEmpresa ? proveedor.razonSocial : proveedor.nombre + " " + proveedor.apellido);
    this.crud.model.proveedor_encrypted_id = proveedor.encrypted_id;
    this.proveedoresSearch = undefined;
    this.getSaldoDeudor();
  }

  getSaldoDeudor() {
    this.apiService.DoGET<ApiResult<number>>("proveedores/saldos/deudor?encryptedId=" + encodeURIComponent(this.crud.model.proveedor_encrypted_id), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.proveedor_saldo_deudor = response.data;
          if (this.proveedor_saldo_deudor <= 0) {
            this.notifierService.notify('success', 'El proveedor NO presenta saldo deudor.');
          }
          else {
            this.notifierService.notify('warning', 'El proveedor presenta saldo deudor.');
          }
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.bindAutocompleteEvents<ProveedorDTO>("proveedorSearch", "proveedores/search?filter=", (data) => { this.proveedoresSearch = data; }, () => { this.proveedoresSearch = undefined; });

    setTimeout(function() {
      (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
    
  }

  bindAutocompleteEvents<TDTO>(id: string, url: string, onResult: (data: TDTO[]) => void, onBlur: () => void) {
    let observableKeyUp = fromEvent(document.getElementById(id), 'keyup')
      .pipe (
        map(value => (<any>document.getElementById(id)).value),
        debounceTime(500),
        distinctUntilChanged(),
        mergeMap((search) => {
          return this.apiService.DoGETWithObservable(url + encodeURIComponent(search), /* headers */ null);
        })
      )
      observableKeyUp.subscribe((data) => {
        var result = <ApiResult<AutocompleteResultDTO<TDTO>>>data;
        if (!result.success) {
          this.confirmDialogService.showError("Se ha producido un error interno.");
        }
        else {
          onResult(result.data.results);
        }
    });

    let observableBlur = fromEvent(document.getElementById(id), 'blur');
    observableBlur.subscribe(() => {
      setTimeout(onBlur, 200);
    });
  }

}
