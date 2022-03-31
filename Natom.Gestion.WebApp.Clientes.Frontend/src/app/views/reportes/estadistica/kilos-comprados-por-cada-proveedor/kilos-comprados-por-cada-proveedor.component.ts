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
  selector: 'app-kilos-comprados-por-cada-proveedor',
  styleUrls: ['./kilos-comprados-por-cada-proveedor.component.css'],
  templateUrl: './kilos-comprados-por-cada-proveedor.component.html'
})

export class KilosCompradosPorCadaProveedorComponent implements OnInit {
  filtroFechaDesdeValue: string;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.filtroFechaDesdeValue = "";
  }
  ngOnInit(): void {
    
  }

  decideClosure(event, datepicker) { const path = event.path.map(p => p.localName); if (!path.includes('ngb-datepicker')) { datepicker.close(); } }

  onFechaDesdeChange(newValue) {
    this.filtroFechaDesdeValue = newValue.day + "/" + newValue.month + "/" + newValue.year;
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

    // if (this.filtroFechaDesdeValue === undefined || this.filtroFechaDesdeValue.length === 0)
    // {
    //   this.confirmDialogService.showError("Debes seleccionar fecha 'Desde'.");
    //   return;
    // }
    
    this.apiService.OpenNewTab("reportes/estadistica/kilos-comprados-por-cada-proveedor?desde=" + encodeURIComponent(this.filtroFechaDesdeValue));
  }
}
