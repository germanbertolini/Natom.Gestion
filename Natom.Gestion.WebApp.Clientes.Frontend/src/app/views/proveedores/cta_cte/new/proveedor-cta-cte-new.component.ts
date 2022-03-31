import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { MovimientoCajaDiariaDTO } from "src/app/classes/dto/cajas/movimiento-caja-diaria.dto";
import { ProveedorCtaCteMovimientoDTO } from "src/app/classes/dto/proveedores/cta-cte/proveedor-cta-cte-movimiento.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";

@Component({
  selector: 'app-proveedor-cta-cte-new-crud',
  styleUrls: ['./proveedor-cta-cte-new.component.css'],
  templateUrl: './proveedor-cta-cte-new.component.html'
})

export class ProveedorCuentaCorrienteNewComponent implements OnInit {
  crud: CRUDView<ProveedorCtaCteMovimientoDTO>;
  
  constructor(private route: ActivatedRoute,
              private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.crud = new CRUDView<ProveedorCtaCteMovimientoDTO>(routeService);
    this.crud.model = new ProveedorCtaCteMovimientoDTO();
    this.crud.model.tipo = "D";
    this.crud.model.usuarioNombre = authService.getCurrentUser().first_name;
    this.crud.model.encrypted_proveedor_id = this.route.snapshot.paramMap.get('id');
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

    if (this.crud.model.observaciones === undefined || this.crud.model.observaciones === "")
    {
      this.confirmDialogService.showError("Debes ingresar una observación.");
      return;
    }

    this.apiService.DoPOST<ApiResult<MovimientoCajaDiariaDTO>>("proveedores/cta_cte/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Movimiento guardado correctamente.');
          this.routerService.navigate(['/proveedores/cta_cte/' + this.crud.model.encrypted_proveedor_id ]);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    setTimeout(function() {
      (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
    
  }

}
