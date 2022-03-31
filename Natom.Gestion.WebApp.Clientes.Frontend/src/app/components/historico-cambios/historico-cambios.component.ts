import { Component, Input, OnInit } from '@angular/core';  
import { HistoricoListDTO } from 'src/app/classes/dto/historico-cambios/historico-list.dto';
import { ApiResult } from 'src/app/classes/dto/shared/api-result.dto';
import { ApiService } from 'src/app/services/api.service';
import { ConfirmDialogService } from '../confirm-dialog/confirm-dialog.service';
import { HistoricoCambiosService } from './historico-cambios.service';  
  
@Component({  
    selector: 'app-historico-cambios',  
    templateUrl: 'historico-cambios.component.html',  
    styleUrls: ['historico-cambios.component.css']  
})  
  
export class HistoricoCambiosComponent implements OnInit {  
    message: any; 
    Historico: Array<HistoricoListDTO>; 
    constructor(  
        private historicoCambiosService: HistoricoCambiosService,
        private confirmDialogService: ConfirmDialogService,
        private apiService: ApiService
    ) { }  
  
    ngOnInit(): any {  
       /** 
        *   This function waits for a message from alert service, it gets 
        *   triggered when we call this from any other component 
        */  
        this.historicoCambiosService.getMessage().subscribe(message => {  
            this.message = message;
            this.Historico = new Array<HistoricoListDTO>();

            if (message !== undefined && message !== null) {
                this.apiService.DoGET<ApiResult<HistoricoListDTO[]>>("historicoCambios/get?entity=" + message.entity + "&encrypted_id=" + encodeURIComponent(message.encrypted_id), /*headers*/ null,
                (response) => {
                  if (!response.success) {
                    this.confirmDialogService.showError(response.message);
                  }
                  else {
                    this.Historico = response.data;
                  }
                },
                (errorMessage) => {
                  this.confirmDialogService.showError(errorMessage);
                });
            }
        });  
    }  
}  