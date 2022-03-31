import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { ProveedorDTO } from "src/app/classes/dto/proveedores/proveedor.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";

@Component({
  selector: 'app-proveedor-crud',
  styleUrls: ['./proveedor-crud.component.css'],
  templateUrl: './proveedor-crud.component.html'
})

export class ProveedorCrudComponent implements OnInit {

  crud: CRUDView<ProveedorDTO>;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<ProveedorDTO>(routeService);
    this.crud.model = new ProveedorDTO();
    this.crud.model.monto_cta_cte = 0;
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    //TAB GENERAL
    if (this.crud.model.esEmpresa) {
      if (this.crud.model.razonSocial === undefined || this.crud.model.razonSocial.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar la Razón social.");
        return;
      }

      if (this.crud.model.nombreFantasia === undefined || this.crud.model.nombreFantasia.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el Nombre fantasía.");
        return;
      }

      if (this.crud.model.numeroDocumento === undefined || this.crud.model.numeroDocumento.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el CUIT.");
        return;
      }

      if (!(/^[0-9]{2}-[0-9]{8}-[0-9]{1}$/.test(this.crud.model.numeroDocumento)))
      {
        this.confirmDialogService.showError("Debes ingresar un CUIT válido.");
        return;
      }
    }
    else
    {
      if (this.crud.model.nombre === undefined || this.crud.model.nombre.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el Nombre.");
        return;
      }

      if (this.crud.model.apellido === undefined || this.crud.model.apellido.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el Apellido.");
        return;
      }

      if (this.crud.model.numeroDocumento === undefined || this.crud.model.numeroDocumento.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el DNI.");
        return;
      }

      if (!(/^[0-9]{8}$/.test(this.crud.model.numeroDocumento)))
      {
        this.confirmDialogService.showError("Debes ingresar un DNI válido.");
        return;
      }
    }

    if (this.crud.model.domicilio === undefined || this.crud.model.domicilio.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar el Domicilio.");
      return;
    }

    if (this.crud.model.entreCalles === undefined || this.crud.model.entreCalles.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar las Entre calles.");
      return;
    }

    if (this.crud.model.localidad === undefined || this.crud.model.localidad.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar la Localidad.");
      return;
    }

    if (this.crud.model.monto_cta_cte === undefined || this.crud.model.monto_cta_cte < 0)
    {
      this.confirmDialogService.showError("El monto de la Cuenta Corriente no puede ser inferior a cero.");
      return;
    }

    //TAB CONTACTO
    //Todo opcional: No controlamos nada!
    

    this.apiService.DoPOST<ApiResult<ProveedorDTO>>("proveedores/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Proveedor guardado correctamente.');
          this.routerService.navigate(['/proveedores']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    if (this.crud.isEditMode) {
      this.apiService.DoGET<ApiResult<any>>("proveedores/basics/data" + (this.crud.isEditMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
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
    else {
      setTimeout(function() {
        (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
      }, 300);
    }
    
  }

}
