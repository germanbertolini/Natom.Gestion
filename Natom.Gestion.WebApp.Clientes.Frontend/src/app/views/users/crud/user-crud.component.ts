import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { PermisoDTO } from "src/app/classes/dto/permiso.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { UserDTO } from "src/app/classes/dto/user.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { DataTableDTO } from "../../../classes/data-table-dto";

@Component({
  selector: 'app-user-crud',
  styleUrls: ['./user-crud.component.css'],
  templateUrl: './user-crud.component.html'
})

export class UserCrudComponent implements OnInit {
  permisos: PermisoDTO[];
  crud: CRUDView<UserDTO>;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.crud = new CRUDView<UserDTO>(routeService);
    this.crud.model = new UserDTO();
    this.crud.model.permisos = new Array<string>();
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.first_name === undefined || this.crud.model.first_name === "")
    {
      this.confirmDialogService.showError("Debes ingresar un Nombre.");
      return;
    }

    if (this.crud.model.last_name === undefined || this.crud.model.last_name === "")
    {
      this.confirmDialogService.showError("Debes ingresar un Apellido.");
      return;
    }

    if (this.crud.model.email === undefined || this.isValidEmail(this.crud.model.email))
    {
      this.confirmDialogService.showError("Debes ingresar un Email válido.");
      return;
    }

    if (this.crud.model.permisos.length < 1)
    {
      this.confirmDialogService.showError("Debes seleccionar al menos un permiso.");
      return;
    }

    this.apiService.DoPOST<ApiResult<UserDTO>>("users/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Usuario guardado correctamente.');
          this.routerService.navigate(['/users']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  onSwitchPermisoChanged(event, encrypted_id: string) {
    if (event.target.checked)
      this.crud.model.permisos.push(encrypted_id);
    else
    {
      this.crud.model.permisos.forEach ((element, index) => {
        if(element === encrypted_id)
          this.crud.model.permisos.splice(index, 1);
      });
    }
  }

  containsPermiso(id: string): boolean {
    return this.crud.model.permisos.indexOf(id) >= 0;
  }
  
  isValidEmail(search:string):boolean
  {
      var regexp = new RegExp(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
      return !regexp.test(search);
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("users/basics/data" + (this.crud.isEditMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          if (response.data.entity !== null)
            this.crud.model = response.data.entity;

          if (this.crud.model.permisos === null)
            this.crud.model.permisos = new Array<string>();

          this.permisos = response.data.permisos;
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
                      
    setTimeout(function() {
      (<any>$("#user-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
    
  }

}
