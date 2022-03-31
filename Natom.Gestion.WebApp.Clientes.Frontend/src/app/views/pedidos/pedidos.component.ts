import { HttpClient } from "@angular/common/http";
import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { MarcaDTO } from "src/app/classes/dto/marca.dto";
import { PedidoListDetalleDTO } from "src/app/classes/dto/pedidos/pedido-list-detalle.dto";
import { PedidosListDTO } from "src/app/classes/dto/pedidos/pedidos-list.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { TransporteDTO } from "src/app/classes/dto/transportes/transporte.dto";
import { ZonaDTO } from "src/app/classes/dto/zonas/zona.dto";
import { HistoricoCambiosService } from "src/app/components/historico-cambios/historico-cambios.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { DataTableDTO } from '../../classes/data-table-dto';
import { ConfirmDialogService } from "../../components/confirm-dialog/confirm-dialog.service";

@Component({
  selector: 'app-pedidos',
  styleUrls: ['./pedidos.component.css'],
  templateUrl: './pedidos.component.html'
})
export class PedidosComponent implements OnInit {
  @ViewChild('despacharPedidoModal', { static: false }) despacharPedidoModal : TemplateRef<any>; // Note: TemplateRef
  @ViewChild('confirmarPedidoModal', { static: false }) confirmarPedidoModal : TemplateRef<any>; // Note: TemplateRef
  @ViewChild('modificarPedidoModal', { static: false }) modificarPedidoModal : TemplateRef<any>; // Note: TemplateRef
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  Pedidos: PedidosListDTO[];
  transportes: TransporteDTO[];
  Noty: any;
  zonas: ZonaDTO[];
  filterStatusValue: string;
  filterZonaValue: string;
  filterTransporteValue: string;
  despachar_pedido_modal: NgbModalRef;
  despachar_pedido_encrypted_id: string;
  despachar_pedido_transporte_encrypted_id: string;
  confirmar_pedido_modal: NgbModalRef;
  confirmar_pedido_encrypted_id: string;
  dtConfirmarPedido: DataTables.Settings = {};
  PedidoDetail: PedidoListDetalleDTO[];
  modificar_pedido_modal: NgbModalRef;
  modificar_pedido_encrypted_id: string;
  dtModificarPedido: DataTables.Settings = {};
  PedidoDetailModificar: PedidoListDetalleDTO[];

  constructor(private modalService: NgbModal,
              private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService,
              private historicoCambiosService: HistoricoCambiosService) {
    this.filterStatusValue = "TODOS";
    this.filterZonaValue = "";
    this.filterTransporteValue = "";
  }

  onModificarCantidadesClick(id: string) {
    this.modificar_pedido_encrypted_id = id;
    this.modificar_pedido_modal = this.modalService.open(this.modificarPedidoModal, { windowClass : "modalConfirmarPedido"});
    this.dtModificarPedido = {
      pagingType: 'simple_numbers',
      pageLength: 100,
      serverSide: true,
      processing: true,
      info: false,
      paging: false,
      search: false,
      searching: false,
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
          this.apiService.DoPOST<ApiResult<DataTableDTO<PedidoListDetalleDTO>>>("pedidos/detail?id=" + encodeURIComponent(this.modificar_pedido_encrypted_id), dataTablesParameters, /*headers*/ null,
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
                            this.PedidoDetailModificar = response.data.records;
                            if (this.PedidoDetailModificar.length > 0) {
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
        { data: 'producto', orderable: false },
        { data: 'deposito', orderable: false },
        { data: 'pedido', orderable: false }
      ]
    };
  }

  onMarcarEntregaPedidoClick(id: string) {
    this.confirmar_pedido_encrypted_id = id;
    this.confirmar_pedido_modal = this.modalService.open(this.confirmarPedidoModal, { windowClass : "modalConfirmarPedido"});
    this.dtConfirmarPedido = {
      pagingType: 'simple_numbers',
      pageLength: 100,
      serverSide: true,
      processing: true,
      info: false,
      paging: false,
      search: false,
      searching: false,
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
          this.apiService.DoPOST<ApiResult<DataTableDTO<PedidoListDetalleDTO>>>("pedidos/detail?id=" + encodeURIComponent(this.confirmar_pedido_encrypted_id), dataTablesParameters, /*headers*/ null,
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
                            this.PedidoDetail = response.data.records;
                            if (this.PedidoDetail.length > 0) {
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
        { data: 'producto', orderable: false },
        { data: 'deposito', orderable: false },
        { data: 'pedido', orderable: false },
        { data: 'entregado', orderable: false }
      ]
    };
  }

  onDespacharPedidoClick(id: string) {
    this.despachar_pedido_encrypted_id = id;
    this.despachar_pedido_transporte_encrypted_id = "";
    this.despachar_pedido_modal = this.modalService.open(this.despacharPedidoModal);
  }

  onFiltroZonaChange(newValue: string) {
    this.filterZonaValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onFiltroTransporteChange(newValue: string) {
    this.filterTransporteValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onFiltroEstadoChange(newValue: string) {
    this.filterStatusValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onPrintOrdenClick(id: string) {
    this.apiService.OpenNewTab("pedidos/imprimir/orden?encryptedId=" + encodeURIComponent(id));
  }

  onPrintRemitoClick(id: string) {
    this.apiService.OpenNewTab("pedidos/imprimir/remito?encryptedId=" + encodeURIComponent(id));
  }

  onVerHistoricoCambiosClick(id: string) {
    this.historicoCambiosService.show("OrdenDePedido", id);
  }

  onIniciarOrdenClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("Desea marcar el pedido como 'En preparación'?", function () {  
      apiService.DoPOST<ApiResult<any>>("pedidos/preparacion/iniciar?encryptedId=" + encodeURIComponent(id), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Pedido iniciado.');
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

  onCompletarOrdenClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("Desea marcar el pedido como 'Preparación completada'?", function () {  
      apiService.DoPOST<ApiResult<any>>("pedidos/preparacion/finalizacion?encryptedId=" + encodeURIComponent(id), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Preparación de pedido completada.');
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

  onCancelarPreparacionOrdenClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("Desea volver a marcar el pedido como 'Pendiente de preparación'?", function () {  
      apiService.DoPOST<ApiResult<any>>("pedidos/preparacion/cancelar?encryptedId=" + encodeURIComponent(id), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Preparación de pedido cancelada.');
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

  onDeleteClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("IMPORTANTE: Una vez anulado se revertirá el Stock y no se podrá deshacer la acción. ¿Desea continuar?", function () {  
      apiService.DoPOST<ApiResult<any>>("pedidos/anular?encryptedId=" + encodeURIComponent(id), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Pedido anulado con éxito.');
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

  onDespacharOrdenConfirmadoClick() {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;
    let pedidoId = this.despachar_pedido_encrypted_id;
    let transporteId = this.despachar_pedido_transporte_encrypted_id;
    let modalInstance = this.despachar_pedido_modal;

    if (transporteId === undefined || transporteId.length === 0) {
      this.confirmDialogService.showError("Debes seleccionar un Transporte.");
      return;
    }

    this.confirmDialogService.showConfirm("Desea marcar el pedido como 'Despachado'?", function () {  
      apiService.DoPOST<ApiResult<any>>("pedidos/despachar?encryptedId=" + encodeURIComponent(pedidoId) + "&transporteId=" + encodeURIComponent(transporteId), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                modalInstance.close();
                                                notifier.notify('success', 'Orden despachada correctamente.');
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

  onCambiarCantidadesOrdenClick() {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;
    let pedidoId = this.modificar_pedido_encrypted_id;
    let detalle = this.PedidoDetailModificar;
    let modalInstance = this.modificar_pedido_modal;

    for (let i = 0; i < this.PedidoDetailModificar.length; i ++) {
      if (this.PedidoDetailModificar[i].cantidad < 0) {
        this.confirmDialogService.showError("La cantidad no puede ser menor a cero.");
        return;
      }
    }

    apiService.DoPOST<ApiResult<any>>("pedidos/save_detail?encryptedId=" + encodeURIComponent(pedidoId), detalle, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                modalInstance.close();
                                                notifier.notify('success', 'Orden modificada correctamente.');
                                                dataTableInstance.then((dtInstance: DataTables.Api) => {
                                                  dtInstance.ajax.reload()
                                                });
                                              }
                                            },
                                            (errorMessage) => {
                                              confirmDialogService.showError(errorMessage);
                                            });
  }

  onEntregadoOrdenClick() {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;
    let pedidoId = this.confirmar_pedido_encrypted_id;
    let detalle = this.PedidoDetail;
    let modalInstance = this.confirmar_pedido_modal;

    for (let i = 0; i < this.PedidoDetail.length; i ++) {
      if (this.PedidoDetail[i].entregado < 0) {
        this.confirmDialogService.showError("La cantidad entregada no puede ser menor a cero.");
        return;
      }

      if (this.PedidoDetail[i].entregado > this.PedidoDetail[i].cantidad) {
        this.confirmDialogService.showError("La cantidad entregada no puede ser mayor a la solicitada.");
        return;
      }
    }

    this.confirmDialogService.showConfirm("Desea marcar el pedido como 'Entregado'? La mercaderia no entregada reingresará a su depósito origen.", function () {  
      apiService.DoPOST<ApiResult<any>>("pedidos/entregado?encryptedId=" + encodeURIComponent(pedidoId), detalle, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                modalInstance.close();
                                                notifier.notify('success', 'Orden entregada correctamente.');
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

  onNoEntregadoOrdenClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;

    this.confirmDialogService.showConfirm("Desea marcar el pedido nuevamente como 'Pendiente de despacho'?", function () {  
      apiService.DoPOST<ApiResult<any>>("pedidos/no_entrega?encryptedId=" + encodeURIComponent(id), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Orden nuevamente en Pendiente de despacho.');
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

    this.apiService.DoGET<ApiResult<any>>("pedidos/basics/data_list", /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
            this.zonas = <Array<ZonaDTO>>response.data.zonas;
            this.transportes = <Array<TransporteDTO>>response.data.transportes;

            setTimeout(function() {
              (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
            }, 300);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });

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
        }
      },
      order: [[0, "desc"]],
      ajax: (dataTablesParameters: any, callback) => {
          this.apiService.DoPOST<ApiResult<DataTableDTO<PedidosListDTO>>>("pedidos/list?status=" + this.filterStatusValue + "&zona=" + encodeURIComponent(this.filterZonaValue) + "&transporte=" + encodeURIComponent(this.filterTransporteValue), dataTablesParameters, /*headers*/ null,
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
                            this.Pedidos = response.data.records;
                            if (this.Pedidos.length > 0) {
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
        { data: 'pedido' },
        { data: 'documento' },
        { data: 'cliente' },
        { data: 'fechaHora' },
        { data: 'fechaHoraPreparado' },
        { data: 'estado', orderable: false },
        { data: 'peso_total' },
        { data: '', orderable: false } //BOTONERA
      ]
    };
  }

}
