import { HttpClient } from "@angular/common/http";
import { Component, Input, OnInit, TemplateRef, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { NgbModal, NgbModalRef } from "@ng-bootstrap/ng-bootstrap";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { ListaDePreciosDTO } from "src/app/classes/dto/precios/lista-de-precios.dto";
import { PrecioListDTO } from "src/app/classes/dto/precios/precio-list.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { DataTableDTO } from '../../classes/data-table-dto';
import { ConfirmDialogService } from "../../components/confirm-dialog/confirm-dialog.service";

@Component({
  selector: 'app-precios',
  styleUrls: ['./precios.component.css'],
  templateUrl: './precios.component.html'
})
export class PreciosComponent implements OnInit {
  @ViewChild('imprimirModal', { static: false }) imprimirModal : TemplateRef<any>; // Note: TemplateRef
  imprimir_modal_instance: NgbModalRef;
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtIndex: DataTables.Settings = {};
  Precios: PrecioListDTO[];
  ListasDePrecios: Array<ListaDePreciosDTO>;
  Noty: any;
  filterListaValue: string;
  listasDePrecios: ListaDePreciosDTO[];
  imprimir_precio_lista_encrypted_id: string;

  constructor(private modalService: NgbModal,
              private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
    this.filterListaValue = "";
    this.imprimir_precio_lista_encrypted_id = "";
  }

  onFiltroListaDePreciosChange(newValue: string) {
    this.filterListaValue = newValue;
    this.dtElement.dtInstance.then((dtInstance: DataTables.Api) => {
      dtInstance.ajax.reload()
    });
  }

  onRenewClick(id: string) {
    this.routerService.navigate(['/precios/renew/' + encodeURIComponent(id)]);
  }

  onImprimirClick() {
    this.imprimir_modal_instance = this.modalService.open(this.imprimirModal);
  }

  imprimir() {
    if (this.imprimir_precio_lista_encrypted_id === undefined || this.imprimir_precio_lista_encrypted_id === null || this.imprimir_precio_lista_encrypted_id.length === 0) {
      this.confirmDialogService.showError("Debes seleccionar una Lista de precios.");
      return;
    }

    this.apiService.OpenNewTab("reportes/precios/listas/imprimir?encryptedId=" + encodeURIComponent(this.imprimir_precio_lista_encrypted_id));
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
        this.apiService.DoPOST<ApiResult<DataTableDTO<PrecioListDTO>>>("precios/list?lista=" + encodeURIComponent(this.filterListaValue), dataTablesParameters, /*headers*/ null,
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
                          this.Precios = response.data.records;
                          this.ListasDePrecios = <Array<ListaDePreciosDTO>>response.data.extraData.listasDePrecios;
                          if (this.Precios.length > 0) {
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
        { data: 'articulo' },
        { data: 'listaDePrecios' },
        { data: 'aplicaDesde' },
        { data: 'precio' },
        { data: '', orderable: false } //BOTONERA
      ]
    };

    this.apiService.DoGET<ApiResult<any>>("ventas/basics/data", /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
            this.listasDePrecios = <Array<ListaDePreciosDTO>>response.data.listasDePrecios;

            setTimeout(function() {
              (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
            }, 300);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

}
