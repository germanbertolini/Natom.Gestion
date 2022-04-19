import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { MarcaDTO } from "src/app/classes/dto/marca.dto";
import { NegocioConfigDTO } from "src/app/classes/dto/negocio/negocio.config.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";

@Component({
  selector: 'app-negocio-config',
  styleUrls: ['./negocio-config.component.css'],
  templateUrl: './negocio-config.component.html'
})

export class NegocioConfigComponent implements OnInit {

  model: NegocioConfigDTO;
  ImageBaseData:string | ArrayBuffer=null;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.model = new NegocioConfigDTO();
  }

  handleFileInput(files: FileList) {
    let me = this;
    let file = files[0];
    let reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = function () {

      me.ImageBaseData=reader.result;

      if (me.ImageBaseData != null)
        me.model.logo_base64 = me.ImageBaseData.toString();

    };
    
    reader.onerror = function (error) {
      console.log('Error: ', error);
    };
  }

  onSaveClick() {
    if (this.model.razon_social === undefined || this.model.razon_social.length === null || this.model.razon_social.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar una Razón social.");
      return;
    }

    if (this.model.tipo_documento === undefined || this.model.tipo_documento.length === null || this.model.tipo_documento.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar un Tipo de documento.");
      return;
    }

    if (this.model.numero_documento === undefined || this.model.numero_documento.length === null || this.model.numero_documento.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar un Número de documento.");
      return;
    }

    // if (this.model.domicilio === undefined || this.model.domicilio.length === null || this.model.domicilio.length === 0)
    // {
    //   this.confirmDialogService.showError("Debes ingresar un Domicilio.");
    //   return;
    // }

    // if (this.model.localidad === undefined || this.model.localidad.length === null || this.model.localidad.length === 0)
    // {
    //   this.confirmDialogService.showError("Debes ingresar una Localidad.");
    //   return;
    // }

    // if (this.model.telefono === undefined || this.model.telefono.length === null || this.model.telefono.length === 0)
    // {
    //   this.confirmDialogService.showError("Debes ingresar un Teléfono.");
    //   return;
    // }

    // if (this.model.email === undefined || this.model.email.length === null || this.model.email.length === 0)
    // {
    //   this.confirmDialogService.showError("Debes ingresar un Email.");
    //   return;
    // }

    this.apiService.DoPOST<ApiResult<MarcaDTO>>("negocio/config/save", this.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Datos guardados correctamente.');
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<NegocioConfigDTO>>("negocio/config", /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.model = response.data;
          this.ImageBaseData = this.model.logo_base64;
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
    
      setTimeout(function() {
        (<any>$('[data-toggle="tooltip"]')).tooltip();
      }, 300);
  }

}
