import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { TransferenciaEntreCajasDTO } from "src/app/classes/dto/cajas/transferencia-entre-cajas.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";

@Component({
  selector: 'app-caja-transferencia',
  styleUrls: ['./caja-transferencia.component.css'],
  templateUrl: './caja-transferencia.component.html'
})

export class CajaTransferenciaComponent implements OnInit {
  crud: CRUDView<TransferenciaEntreCajasDTO>;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<TransferenciaEntreCajasDTO>(routeService);
    this.crud.model = new TransferenciaEntreCajasDTO();
    this.crud.model.origen = "diaria";
    this.crud.model.destino = "fuerte";
    this.crud.model.usuarioNombre = authService.getCurrentUser().first_name;
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Cancelar?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.origen === this.crud.model.destino)
    {
      this.confirmDialogService.showError("La caja 'Origen' no puede ser la misma que 'Destino'.");
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

    this.apiService.DoPOST<ApiResult<TransferenciaEntreCajasDTO>>("cajas/transfer", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Transferencia registrada correctamente.');
          window.history.back(); //this.routerService.navigate(['/cajas/' + this.crud.model.origen]);
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
