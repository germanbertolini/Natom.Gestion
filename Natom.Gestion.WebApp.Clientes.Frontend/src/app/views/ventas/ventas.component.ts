import { HttpClient } from "@angular/common/http";
import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { MarcaDTO } from "src/app/classes/dto/marca.dto";
import { PedidosListDTO } from "src/app/classes/dto/pedidos/pedidos-list.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { VentasListDTO } from "src/app/classes/dto/ventas/ventas-list.dto";
import { HistoricoCambiosService } from "src/app/components/historico-cambios/historico-cambios.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { DataTableDTO } from '../../classes/data-table-dto';
import { ConfirmDialogService } from "../../components/confirm-dialog/confirm-dialog.service";

@Component({
  selector: 'app-ventas',
  styleUrls: ['./ventas.component.css'],
  templateUrl: './ventas.component.html'
})
export class VentasComponent implements OnInit {
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  Ventas: VentasListDTO[];
  Noty: any;
  filterStatusValue: string;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService,
              private historicoCambiosService: HistoricoCambiosService) {
    this.filterStatusValue = "TODOS";
  }

  onFiltroEstadoChange(newValue: string) {
    this.filterStatusValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onPrintVentaClick(id: string) {
    this.apiService.OpenNewTab("ventas/imprimir/comprobante?encryptedId=" + encodeURIComponent(id));
  }

  onVerHistoricoCambiosClick(id: string) {
    this.historicoCambiosService.show("Venta", id);
  }

  onAnularFacturacionClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("¿Desea anular la Facturación?", function () {  
      apiService.DoPOST<ApiResult<any>>("ventas/anular?encryptedId=" + encodeURIComponent(id), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Facturación anulada.');
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
      paging: true,
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
        }
      },
      order: [[0, "desc"]],
      ajax: (dataTablesParameters: any, callback) => {
          this.apiService.DoPOST<ApiResult<DataTableDTO<VentasListDTO>>>("ventas/list?status=" + this.filterStatusValue, dataTablesParameters, /*headers*/ null,
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
                            this.Ventas = response.data.records;
                            if (this.Ventas.length > 0) {
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
        { data: 'venta' },
        { data: 'documento' },
        { data: 'cliente' },
        { data: 'fechaHora' },
        { data: 'pedidos', orderable: false },
        { data: 'remitos', orderable: false },
        { data: 'peso_total' },
        { data: '', orderable: false } //BOTONERA
      ]
    };
  }

}
