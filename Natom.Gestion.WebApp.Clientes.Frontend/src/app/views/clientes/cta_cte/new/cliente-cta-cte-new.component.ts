import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { MovimientoCajaDiariaDTO } from "src/app/classes/dto/cajas/movimiento-caja-diaria.dto";
import { ClienteCtaCteMovimientoDTO } from "src/app/classes/dto/clientes/cta-cte/cliente-cta-cte-movimiento.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";

@Component({
  selector: 'app-cliente-cta-cte-new-crud',
  styleUrls: ['./cliente-cta-cte-new.component.css'],
  templateUrl: './cliente-cta-cte-new.component.html'
})

export class ClienteCuentaCorrienteNewComponent implements OnInit {
  crud: CRUDView<ClienteCtaCteMovimientoDTO>;
  
  constructor(private route: ActivatedRoute,
              private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.crud = new CRUDView<ClienteCtaCteMovimientoDTO>(routeService);
    this.crud.model = new ClienteCtaCteMovimientoDTO();
    this.crud.model.tipo = "D";
    this.crud.model.usuarioNombre = authService.getCurrentUser().first_name;
    this.crud.model.encrypted_cliente_id = this.route.snapshot.paramMap.get('id');
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

    this.apiService.DoPOST<ApiResult<MovimientoCajaDiariaDTO>>("clientes/cta_cte/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Movimiento guardado correctamente.');
          this.routerService.navigate(['/clientes/cta_cte/' + this.crud.model.encrypted_cliente_id ]);
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
