import { HttpHeaders } from "@angular/common/http";
import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { DataTableDTO } from "src/app/classes/data-table-dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { PendingActivationSyncDTO } from "src/app/classes/dto/pending_activation.sync.dto";

@Component({
  selector: 'app-syncs-clientes-new',
  templateUrl: './syncs-clientes-new.component.html'
})
export class SyncsClientesNewComponent implements OnInit {

  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtSyncs: DataTables.Settings = {};
  Syncs: PendingActivationSyncDTO[];
  Noty: any;
  clienteId: string;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.clienteId = decodeURIComponent(this.routeService.snapshot.paramMap.get('id_cliente'));
  }

  onEnlazarClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let routerService = this.routerService;
    let clienteId = this.clienteId;

    this.confirmDialogService.showConfirm("¿Desea enlazar el sincronizador al cliente?", function () {  
      apiService.DoPOST<ApiResult<any>>("clientes/syncs/activate?encryptedId=" + encodeURIComponent(id) + "&clientId=" + encodeURIComponent(clienteId), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Sincronizador enlazado con éxito.');
                                                routerService.navigate(['/clientes/' + encodeURIComponent(clienteId) + "/syncs"]);
                                              }
                                            },
                                            (errorMessage) => {
                                              confirmDialogService.showError(errorMessage);
                                            });
      
    });
  }

  ngOnInit(): void {

    this.dtSyncs = {
      pagingType: 'simple_numbers',
      pageLength: 10,
      serverSide: true,
      processing: true,
      info: true,
      language: {
        emptyTable: '',
        zeroRecords: 'No hay registros',
        lengthMenu: 'Mostrar _MENU_ registros',
        search: 'Buscar:',
        info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
        infoEmpty: 'De 0 a 0 de 0 registros',
        infoFiltered: '(filtrados de _MAX_ registros totales)',
        paginate: {
          first: 'Primero',
          last: 'Último',
          next: 'Siguiente',
          previous: 'Anterior'
        },
      },
      ajax: (dataTablesParameters: any, callback) => {
        this.apiService.DoPOST<ApiResult<DataTableDTO<PendingActivationSyncDTO>>>("clientes/syncs/pending_activation/list", dataTablesParameters, /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          this.confirmDialogService.showError(response.message);
                        }
                        else {
                          callback({
                            recordsTotal: response.data.recordsTotal,
                            recordsFiltered: response.data.recordsFiltered,
                            data: [] //Siempre vacío para delegarle el render a Angular
                          });
                          this.Syncs = response.data.records;
                          if (this.Syncs.length > 0) {
                            $('.dataTables_empty').hide();
                          }
                          else {
                            $('.dataTables_empty').show();
                          }
                          setTimeout(function() {
                            (<any>$("tbody tr").find('[data-toggle="tooltip"]')).tooltip();
                          }, 300);
                        }

                        //Que refresque cada 5 segundos
                        let _dtElement = this.dtElement;
                        setTimeout(function() {
                          _dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
                            dtInstance.ajax.reload();
                          });
                        }, 5000);
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
      },
      columns: [
        { data: 'client_name' },
        { data: 'client_cuit' },
        { data: 'installation_alias' },
        { data: 'installer_name' },
        { data: '' } //BOTONERA
      ]
    };
  }

}
