import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { fromEvent } from 'rxjs';
import { map, distinctUntilChanged, debounceTime, mergeMap } from 'rxjs/operators';
import { NotifierService } from "angular-notifier";
import { ProductoListDTO } from "src/app/classes/dto/productos/producto-list.dto";
import { MovimientoStockDTO } from "src/app/classes/dto/stock/movimiento-stock.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { AutocompleteResultDTO } from "src/app/classes/dto/shared/autocomplete-result.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { DepositoDTO } from "src/app/classes/dto/stock/deposito.dto";
import { ProveedorDTO } from "src/app/classes/dto/proveedores/proveedor.dto";

@Component({
  selector: 'app-stock-new-crud',
  styleUrls: ['./stock-new.component.css'],
  templateUrl: './stock-new.component.html'
})

export class StockNewComponent implements OnInit {
  crud: CRUDView<MovimientoStockDTO>;
  stockActual: number;
  valor: number;
  productoFilterText: string;
  productosSearch: ProductoListDTO[];
  proveedoresSearch: ProveedorDTO[];
  Depositos: Array<DepositoDTO>;
  proveedor_search: string;
  proveedor_encrypted_id: string;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<MovimientoStockDTO>(routeService);
    this.crud.model = new MovimientoStockDTO();
    this.crud.model.esCompra = true;
    this.crud.model.tipo = "";
    this.crud.model.deposito_encrypted_id = "";
    this.crud.model.usuarioNombre = authService.getCurrentUser().first_name;
  }

  onProductoSearchSelectItem (producto: ProductoListDTO) {
    this.crud.model.producto_encrypted_id = producto.encrypted_id;
    this.productoFilterText = "(" + producto.codigo + ") " + producto.marca + " " + producto.descripcion;
    this.productosSearch = undefined;
    this.consultarStock();
  }


  consultarStock() {
    if (this.crud.model.producto_encrypted_id !== undefined && this.crud.model.producto_encrypted_id !== ""
        && this.crud.model.deposito_encrypted_id !== undefined && this.crud.model.deposito_encrypted_id !== "") {
      
      this.apiService.DoGET<ApiResult<any>>("stock/quantity?producto=" + encodeURIComponent(this.crud.model.producto_encrypted_id) + "&deposito=" + encodeURIComponent(this.crud.model.deposito_encrypted_id), /*headers*/ null,
          (response) => {
            if (!response.success) {
              this.confirmDialogService.showError(response.message);
            }
            else {
              this.stockActual = <number>response.data;
    
              setTimeout(function() {
                (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
              }, 300);
            }
          },
          (errorMessage) => {
            this.confirmDialogService.showError(errorMessage);
          });

    }
    else {
      this.stockActual = null;
    }
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.producto_encrypted_id === undefined || this.crud.model.producto_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes buscar y seleccionar un Producto.");
      return;
    }
  
    if (this.crud.model.deposito_encrypted_id === undefined || this.crud.model.deposito_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar un Depósito.");
      return;
    }

    if (this.crud.model.tipo === undefined || this.crud.model.tipo.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar el Tipo de movimiento.");
      return;
    }

    if (this.crud.model.cantidad === undefined || this.crud.model.cantidad <= 0)
    {
      this.confirmDialogService.showError("Debes ingresar una cantidad válida.");
      return;
    }

    if (this.crud.model.observaciones === undefined || this.crud.model.observaciones.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar una Observación para dejar asentado por qué se hizo este movimiento.");
      return;
    }

    // if (this.crud.model.esCompra) {
    //   if (this.crud.model.tipo !== "I")
    //   {
    //     this.confirmDialogService.showError("Si ES COMPRA entonces el movimiento debe ser INGRESO.");
    //     return;
    //   }

    //   if (this.crud.model.proveedor_encrypted_id === undefined || this.crud.model.proveedor_encrypted_id.length === 0)
    //   {
    //     this.confirmDialogService.showError("Debes buscar y seleccionar un Proveedor.");
    //     return;
    //   }

    //   if (this.crud.model.costoUnitario === undefined || this.crud.model.costoUnitario <= 0)
    //   {
    //     this.confirmDialogService.showError("Costo unitario inválido.");
    //     return;
    //   }
    // }
    // else
    {
      this.crud.model.proveedor_encrypted_id = null;
      this.crud.model.costoUnitario = null;
    }

    this.apiService.DoPOST<ApiResult<MovimientoStockDTO>>("stock/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Movimiento guardado correctamente.');
          this.routerService.navigate(['/stock']);
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
  }

  ngOnInit(): void {

    this.bindAutocompleteEvents<ProductoListDTO>("productoSearch", "productos/search?filter=", (data) => { this.productosSearch = data; }, () => { this.productosSearch = undefined; });
    this.bindAutocompleteEvents<ProveedorDTO>("proveedorSearch", "proveedores/search?filter=", (data) => { this.proveedoresSearch = data; }, () => { this.proveedoresSearch = undefined; });

    this.apiService.DoGET<ApiResult<any>>("stock/basics/data", /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.Depositos = <Array<DepositoDTO>>response.data.depositos;

          setTimeout(function() {
            (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
          }, 300);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
    
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
