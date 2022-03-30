import { HttpHeaders } from "@angular/common/http";
import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { DataTableDTO } from "src/app/classes/data-table-dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DeviceDTO } from "src/app/classes/dto/device.dto";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { GoalDTO } from "src/app/classes/dto/goal.dto";

@Component({
  selector: 'app-devices-syncs-clientes',
  templateUrl: './devices-syncs-clientes.component.html'
})
export class DevicesSyncsClientesComponent implements OnInit {
  @ViewChild('asignarModal', { static: false }) asignarModal : TemplateRef<any>; // Note: TemplateRef
  asignar_modal: NgbModalRef;
  asignar_encrypted_id: string;
  asignar_goal_encrypted_id: string;
  
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtDevices: DataTables.Settings = {};
  Devices: DeviceDTO[];
  Goals: GoalDTO[];
  Noty: any;
  clienteId: string;
  syncId: string;

  constructor(private modalService: NgbModal,
              private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.clienteId = decodeURIComponent(this.routeService.snapshot.paramMap.get('id_cliente'));
    this.syncId = decodeURIComponent(this.routeService.snapshot.paramMap.get('id'));
  }

  onChangeGoalClick(encrypted_id: string) {
    this.asignar_encrypted_id = encrypted_id;
    this.asignar_goal_encrypted_id = "";
    this.asignar_modal = this.modalService.open(this.asignarModal);

    this.apiService.DoPOST<ApiResult<Array<GoalDTO>>>("goals/list_actives?encryptedId=" + encodeURIComponent(this.clienteId), {}, /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          this.confirmDialogService.showError(response.message);
                        }
                        else {                          
                          this.Goals = response.data;
                        }
                      },
                      (errorMessage) => {
                        this.confirmDialogService.showError(errorMessage);
                      });
  }

  onAsignarConfirmadoClick() {
    let notifier = this.notifierService;
    let confirmDialogService = this.confirmDialogService;
    let apiService = this.apiService;
    let dataTableInstance = this.dtElement.dtInstance;
    let encryptedId = this.asignar_encrypted_id;
    let goalId = this.asignar_goal_encrypted_id;
    let modalInstance = this.asignar_modal;

    if (goalId === undefined || goalId.length === 0) {
      this.confirmDialogService.showError("Debes seleccionar una Portería / Acceso al cual asignar el dispositivo.");
      return;
    }

    this.confirmDialogService.showConfirm("Desea asignar el dispositivo?", function () {  
      apiService.DoPOST<ApiResult<any>>("clientes/syncs/devices/assign_device?encryptedId=" + encodeURIComponent(encryptedId) + "&goalId=" + encodeURIComponent(goalId), {}, /*headers*/ null,
                                            (response) => {
                                              if (!response.success) {
                                                confirmDialogService.showError(response.message);
                                              }
                                              else {
                                                modalInstance.close();
                                                notifier.notify('success', 'Dispositivo asignado correctamente.');
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

    this.dtDevices = {
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
        this.apiService.DoPOST<ApiResult<DataTableDTO<DeviceDTO>>>("clientes/syncs/devices/list?encryptedId=" + encodeURIComponent(this.syncId), dataTablesParameters, /*headers*/ null,
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
                          this.Devices = response.data.records;
                          if (this.Devices.length > 0) {
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
        { data: 'device_name', orderable: false },
        { data: 'device_last_configuration_at', orderable: false },
        { data: 'device_id', orderable: false },
        { data: 'marca', orderable: false },
        { data: 'modelo', orderable: false },
        { data: 'firmware_version', orderable: false },
        { data: 'location', orderable: false },
        { data: '', orderable: false } /* BOTONERA */
      ]
    };
  }

}
