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
  selector: 'app-user-confirm',
  styleUrls: ['./user-confirm.component.css'],
  templateUrl: './user-confirm.component.html'
})

export class UserConfirmComponent implements OnInit {
  email: string;
  secret: string;
  clave: string;
  clave2: string;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    
    this.clave = "";
    this.clave2 = "";

    let data = this.routeService.snapshot.paramMap.get('data');
    let dataDecoded = atob(data);
    var json = JSON.parse(dataDecoded);
    this.email = json.e;
    this.secret = json.s;
  }

  onSaveClick() {
    console.log("click");

    if (this.clave.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar una contraseña.");
      return;
    }

    if (this.clave2.length === 0)
    {
      this.confirmDialogService.showError("Debes repetir la contraseña.");
      return;
    }

    if (this.clave !== this.clave2)
    {
      this.confirmDialogService.showError("Las contraseñas no coinciden.");
      return;
    }

    let routerService = this.routerService;
    this.apiService.DoPOST<ApiResult<UserDTO>>("users/confirm?data=" + encodeURIComponent(btoa(JSON.stringify({ s: this.secret, p: this.clave }))), {}, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          routerService.navigate(['/login']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

  }

}
