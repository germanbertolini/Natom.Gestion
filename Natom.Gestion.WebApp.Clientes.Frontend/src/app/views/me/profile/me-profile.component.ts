import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { UserDTO } from "src/app/classes/dto/user.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DataTableDTO } from "../../../classes/data-table-dto";

@Component({
  selector: 'app-me-profile',
  styleUrls: ['./me-profile.component.css'],
  templateUrl: './me-profile.component.html'
})

export class MeProfileComponent implements OnInit {
  crud: CRUDView<UserDTO>;
  dtDevices: DataTables.Settings = {};

  constructor(private httpClientService: HttpClient,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<UserDTO>(routeService);
    this.crud.model = new UserDTO();

    //MOCK
    this.crud.model.encrypted_id = "adssdadas123e213132";
    this.crud.model.first_name = "German";
    this.crud.model.last_name = "Bertolini";
    this.crud.model.picture_url = "https://lh3.googleusercontent.com/ogw/ADea4I77Za6iqEqbdUL2uqgk2F88wtfI43U8O3gxDBdbRg=s128-c-mo";
    this.crud.model.email = "german.bertolini@gmail.com";
    this.crud.model.registered_at = new Date('2020-12-28T00:00:00');
  }

  onCancelClick() {
      window.history.back();
  }

  onChangePasswordClick() {
    let notifier = this.notifierService;
    this.confirmDialogService.showConfirm("¿Realmente desea cambiar su clave?", function() {
      notifier.notify('info', 'Se ha enviado un mail a su casilla de correo.');
    });
  }

  ngOnInit(): void {
    setTimeout(function() {
      (<any>$("#device-crud").find('[data-toggle="tooltip"]')).tooltip();
    }, 300);
  }

}
