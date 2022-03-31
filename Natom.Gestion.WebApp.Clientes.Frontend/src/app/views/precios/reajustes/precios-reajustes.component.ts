import { HttpClient } from "@angular/common/http";
import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { ListaDePreciosDTO } from "src/app/classes/dto/precios/lista-de-precios.dto";
import { PrecioReajusteListDTO } from "src/app/classes/dto/precios/precio-reajuste-list.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { DataTableDTO } from '../../../classes/data-table-dto';
import { ConfirmDialogService } from "../../../components/confirm-dialog/confirm-dialog.service";

@Component({
  selector: 'app-precios-reajustes',
  styleUrls: ['./precios-reajustes.component.css'],
  templateUrl: './precios-reajustes.component.html'
})
export class PreciosReajustesComponent implements OnInit {

  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  Reajustes: PrecioReajusteListDTO[];
  ListasDePrecios: Array<ListaDePreciosDTO>;
  Noty: any;
  filterListaValue: string;
  filterStatusValue: string;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.filterStatusValue = "ACTIVOS";
    this.filterListaValue = "";
  }

  onFiltroEstadoChange(newValue: string) {
    this.filterStatusValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onFiltroListaDePreciosChange(newValue: string) {
    this.filterListaValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onDeleteClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("¿Desea anular el reajuste? Esta acción restaurará los precios anteriores.", function () {  
      apiService.DoDELETE<ApiResult<any>>("precios/reajustes/disable?encryptedId=" + encodeURIComponent(id), /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Reajuste anulado con éxito.');
                                                dataTableInstance.then((dtInstance: DataTables.Api) => {
                                                  dtInstance.ajax.reload()
                                                });
                                              }
                                            },
                                            (errorMessage) => {
                                              confirmDialogService.showError(errorMessage);
                                            });
    });
  }

  ngOnInit(): void {

    this.dtIndex = {
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
        this.apiService.DoPOST<ApiResult<DataTableDTO<PrecioReajusteListDTO>>>("precios/reajustes/list?status=" + encodeURIComponent(this.filterStatusValue) + "&lista=" + encodeURIComponent(this.filterListaValue), dataTablesParameters, /*headers*/ null,
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
                          this.Reajustes = response.data.records;
                          this.ListasDePrecios = <Array<ListaDePreciosDTO>>response.data.extraData.listasDePrecios;
                          if (this.Reajustes.length > 0) {
                            $('.dataTables_empty').hide();
                          }
                          else {
                            $('.dataTables_empty').show();
                          }
                          setTimeout(function() {
                            (<any>$("tbody tr").find('[data-toggle="tooltip"]')).tooltip();
                          }, 300);
                        }
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
      },
      columns: [
        { data: 'marca' },
        { data: 'tipo' },
        { data: 'valor' },
        { data: 'listaDePrecios' },
        { data: 'aplicaDesde' },
        { data: 'reajustadoPor', orderable: false },
        { data: '', orderable: false } //BOTONERA
      ]
    };
  }

}
