import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { PlaceDTO } from "src/app/classes/dto/place.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { DataTableDTO } from '../../../classes/data-table-dto';
import { ConfirmDialogService } from "../../../components/confirm-dialog/confirm-dialog.service";


@Component({
  selector: 'app-places',
  templateUrl: './places.component.html'
})
export class PlacesComponent implements OnInit {
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  filterStatusValue: string;
  Places: PlaceDTO[];
  Noty: any;
  clienteId: string;

  constructor(private apiService: ApiService,
              private authService: AuthService,
              private route: ActivatedRoute,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.filterStatusValue = "ACTIVOS";
    this.clienteId = decodeURIComponent(this.route.snapshot.paramMap.get('id_cliente'));
  }

  onFiltroEstadoChange(newValue: string) {
    this.filterStatusValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onNewClick() {
    this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/places/new']);
  }

  onEditClick(id: string) {
    this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/places/edit/' + encodeURIComponent(id)]);
  }

  onGoalsClick(id: string) {
    this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/goals/' + encodeURIComponent(id)]);
  }

  onToleranciasClick(id: string) {
    this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/horarios/' + encodeURIComponent(id)]);
  }

  onEnableClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;
    let clienteId = this.clienteId;

    this.confirmDialogService.showConfirm("Desea dar de alta a la Planta / Oficina?", function () {  
      apiService.DoPOST<ApiResult<any>>("places/enable?clienteEncryptedId=" + encodeURIComponent(clienteId) + "&encryptedId=" + encodeURIComponent(id), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Planta / Oficina dada de alta con éxito.');
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

  onDisableClick(id: string) {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;
    let clienteId = this.clienteId;

    this.confirmDialogService.showConfirm("Desea dar de baja a la Planta / Oficina?", function () {  
      apiService.DoDELETE<ApiResult<any>>("places/disable?clienteEncryptedId=" + encodeURIComponent(clienteId) + "&encryptedId=" + encodeURIComponent(id), /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                notifier.notify('success', 'Planta / Oficina dada de baja con éxito.');
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
        this.apiService.DoPOST<ApiResult<DataTableDTO<PlaceDTO>>>("places/list?encryptedId=" + encodeURIComponent(this.clienteId) + "&status=" + this.filterStatusValue, dataTablesParameters, /*headers*/ null,
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
                          this.Places = response.data.records;
                          if (this.Places.length > 0) {
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
        { data: 'name' },
        { data: 'address' },
        { data: '', orderable: false } //BOTONERA
      ]
    };
  }

}
