import { HttpClient } from "@angular/common/http";
import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { MarcaDTO } from "src/app/classes/dto/marca.dto";
import { MovimientoCajaDiariaDTO } from "src/app/classes/dto/cajas/movimiento-caja-diaria.dto";
import { DataTableDTO } from "src/app/classes/data-table-dto";
import { ConfirmDialogService } from "../../../components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";

@Component({
  selector: 'app-caja-diaria',
  styleUrls: ['./caja-diaria.component.css'],
  templateUrl: './caja-diaria.component.html'
})
export class CajaDiariaComponent implements OnInit {
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  Movimientos: MovimientoCajaDiariaDTO[];
  saldoActual: number;
  filtroFecha: string;
  filtroFechaFinal: string;
  Noty: any;

  decideClosure(event, datepicker) { const path = event.path.map(p => p.localName); if (!path.includes('ngb-datepicker')) { datepicker.close(); } }

  constructor(private apiService: ApiService,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
  }

  onFiltroFechaChange(newValue) {
    this.filtroFechaFinal = newValue.day + "/" + newValue.month + "/" + newValue.year;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
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
      order: [[0, 'desc']],
      ajax: (dataTablesParameters: any, callback) => {
        this.apiService.DoPOST<ApiResult<DataTableDTO<MovimientoCajaDiariaDTO>>>("cajas/diaria/list" + (this.filtroFechaFinal === undefined ? "" : "?filterDate=" + encodeURIComponent(this.filtroFechaFinal)), dataTablesParameters, /*headers*/ null,
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
                          this.Movimientos = response.data.records;
                          this.saldoActual =response.data.extraData.saldoActual;
                          if (this.Movimientos.length > 0) {
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
        { data: 'fechaHora' },
        { data: 'usuario' },
        { data: 'tipo' },
        { data: 'importe' },
        { data: 'cheque' },
        { data: 'observaciones', orderable: false }
      ]
    };
  }

}
