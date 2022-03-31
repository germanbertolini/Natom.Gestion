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
  selector: 'app-reportes-listados-ventas-por-producto-proveedor',
  styleUrls: ['./reportes-listados-ventas-por-producto-proveedor.component.css'],
  templateUrl: './reportes-listados-ventas-por-producto-proveedor.component.html'
})

export class ReportesListadosVentasPorProductoProveedorComponent implements OnInit {
  productoFilterText: string;
  producto_encrypted_id: string;
  productosSearch: ProductoListDTO[];
  proveedoresSearch: ProveedorDTO[];
  proveedor_search: string;
  proveedor_encrypted_id: string;
  filtroFechaDesdeValue: string;
  filtroFechaHastaValue: string;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.proveedor_encrypted_id = "";
    this.producto_encrypted_id = "";
    this.filtroFechaDesdeValue = "";
    this.filtroFechaHastaValue = "";
  }
  ngOnInit(): void {
    this.bindAutocompleteEvents<ProductoListDTO>("productoSearch", "productos/search?filter=", (data) => { this.productosSearch = data; }, () => { this.productosSearch = undefined; });
    this.bindAutocompleteEvents<ProveedorDTO>("proveedorSearch", "proveedores/search?filter=", (data) => { this.proveedoresSearch = data; }, () => { this.proveedoresSearch = undefined; });
  }

  decideClosure(event, datepicker) { const path = event.path.map(p => p.localName); if (!path.includes('ngb-datepicker')) { datepicker.close(); } }

  onFechaDesdeChange(newValue) {
    this.filtroFechaDesdeValue = newValue.day + "/" + newValue.month + "/" + newValue.year;
  }

  onFechaHastaChange(newValue) {
    this.filtroFechaHastaValue = newValue.day + "/" + newValue.month + "/" + newValue.year;
  }

  onProductoSearchSelectItem (producto: ProductoListDTO) {
    this.producto_encrypted_id = producto.encrypted_id;
    this.productoFilterText = "(" + producto.codigo + ") " + producto.marca + " " + producto.descripcion;
    this.productosSearch = undefined;
  }

  onConsultar() {
    // if (this.producto_encrypted_id === undefined || this.producto_encrypted_id.length === 0)
    // {
    //   this.confirmDialogService.showError("Debes buscar y seleccionar un Producto.");
    //   return;
    // }
  
    // if (this.proveedor_encrypted_id === undefined || this.proveedor_encrypted_id.length === 0)
    // {
    //   this.confirmDialogService.showError("Debes buscar y seleccionar un Proveedor.");
    //   return;
    // }

    if (this.filtroFechaDesdeValue === undefined || this.filtroFechaDesdeValue.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar fecha 'Desde'.");
      return;
    }

    if (this.filtroFechaHastaValue === undefined || this.filtroFechaHastaValue.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar fecha 'Hasta'.");
      return;
    }
    
    this.apiService.OpenNewTab("reportes/listados/ventas-por-producto-proveedor?proveedorId=" + encodeURIComponent(this.proveedor_encrypted_id) + "&productoId=" + encodeURIComponent(this.producto_encrypted_id) + "&desde=" + encodeURIComponent(this.filtroFechaDesdeValue) + "&hasta=" + encodeURIComponent(this.filtroFechaHastaValue));
  }

  onProveedorSearchSelectItem (proveedor: ProveedorDTO) {
    this.proveedor_search = proveedor.tipoDocumento + " " + proveedor.numeroDocumento + " /// " + (proveedor.esEmpresa ? proveedor.razonSocial : proveedor.nombre + " " + proveedor.apellido);
    this.proveedor_encrypted_id = proveedor.encrypted_id;
    this.proveedoresSearch = undefined;
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
