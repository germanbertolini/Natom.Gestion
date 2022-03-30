import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { DataTableDTO } from '../../../classes/data-table-dto';
import { ConfirmDialogService } from "../../../components/confirm-dialog/confirm-dialog.service";
import { GoalDTO } from "src/app/classes/dto/goal.dto";
import { HorarioDTO } from "src/app/classes/dto/horario.dto";

@Component({
  selector: 'app-horarios',
  templateUrl: './horarios.component.html'
})
export class HorariosComponent implements OnInit {
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  Horarios: HorarioDTO[];
  Noty: any;
  place_id: string;
  clienteId: string;

  constructor(private apiService: ApiService,
              private route: ActivatedRoute,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.place_id = decodeURIComponent(this.route.snapshot.paramMap.get('place_id'));
    this.clienteId = decodeURIComponent(this.route.snapshot.paramMap.get('id_cliente'));
  }

  onFiltroEstadoChange(newValue: string) {
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onNewClick() {
    this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/horarios/' + encodeURIComponent(this.place_id) + '/new']);
  }

  onVerClick(id: string) {
    this.routerService.navigate(['/clientes/' + encodeURIComponent(this.clienteId) + '/horarios/' + encodeURIComponent(this.place_id) + '/ver/' + encodeURIComponent(id)]);
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
      order: [[1, "desc"]],
      ajax: (dataTablesParameters: any, callback) => {
        this.apiService.DoPOST<ApiResult<DataTableDTO<HorarioDTO>>>("horarios/list?encryptedId=" + encodeURIComponent(this.place_id), dataTablesParameters, /*headers*/ null,
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
                          this.Horarios = response.data.records;
                          if (this.Horarios.length > 0) {
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
        { data: 'aplica_desde' },
        { data: 'aplica_hasta' },
        { data: 'configurado' },
        { data: 'status', orderable: false },
        { data: '', orderable: false } //BOTONERA
      ]
    };
  }

}
