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
  selector: 'app-clientes-que-no-compran-desde-fecha',
  styleUrls: ['./clientes-que-no-compran-desde-fecha.component.css'],
  templateUrl: './clientes-que-no-compran-desde-fecha.component.html'
})

export class ClientesQueNoCompranDesdeFechaComponent implements OnInit {
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

    if (this.filtroFechaDesdeValue === undefined || this.filtroFechaDesdeValue.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar fecha 'Desde'.");
      return;
    }
    
    this.apiService.OpenNewTab("reportes/listados/clientes-que-no-compran-desde-fecha?desde=" + encodeURIComponent(this.filtroFechaDesdeValue));
  }
}
