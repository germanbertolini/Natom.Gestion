import { HttpClient } from "@angular/common/http";
import { Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables";
import { NotifierService } from "angular-notifier";
import { ClienteDTO } from "src/app/classes/dto/clientes/cliente.dto";
import { ClienteMontoDTO } from "src/app/classes/dto/clientes/cliente.monto.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { ZonaDTO } from "src/app/classes/dto/zonas/zona.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";

@Component({
  selector: 'app-cliente-crud',
  styleUrls: ['./cliente-crud.component.css'],
  templateUrl: './cliente-crud.component.html'
})

export class ClienteCrudComponent implements OnInit {
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtMensualidad: DataTables.Settings = {};
  
  crud: CRUDView<ClienteDTO>;
  zonas: ZonaDTO[];
  mensualidad_monto: number;
  mensualidad_desde: string;
  mensualidad_desde_date: Date;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<ClienteDTO>(routeService);
    this.crud.model = new ClienteDTO();
    this.crud.model.montos = new Array<ClienteMontoDTO>();
    this.crud.model.zona_encrypted_id = "";
    this.mensualidad_desde = "";
  }

  decideClosure(event, datepicker) { const path = event.path.map(p => p.localName); if (!path.includes('ngb-datepicker')) { datepicker.close(); } }

  onFechaDesdeChange(newValue) {
    this.mensualidad_desde = newValue.day + "/" + newValue.month + "/" + newValue.year;
    this.mensualidad_desde_date = new Date(newValue.year, newValue.month - 1, newValue.day);
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onEliminarMontoClick(monto: ClienteMontoDTO) {
    let me = this;
    this.confirmDialogService.showConfirm("¿Eliminar monto? Esto hará que quede como vigente el monto anterior", function() {
      for (let i = 0; i < me.crud.model.montos.length; i ++) {
        if (me.crud.model.montos[i] === monto)
        {
          me.crud.model.montos.splice(i, 1);
          break;
        }
      }
    });

    if (this.crud.model.montos.length > 0) {
      $('.dataTables_empty').hide();
    }
    else {
      $('.dataTables_empty').show();
    }
  }

  onAgregarMensualidadClick() {
    if (this.mensualidad_monto === undefined || this.mensualidad_monto === null || this.mensualidad_monto <= 0) {
      this.confirmDialogService.showError("Debes ingresar un monto válido.");
      return;
    }

    if (this.mensualidad_desde === undefined || this.mensualidad_desde === null || this.mensualidad_desde.length === 0) {
      this.confirmDialogService.showError("Debes seleccionar una fecha 'Desde'.");
      return;
    }

    this.crud.model.montos.push({
      encrypted_id: "",
      monto: this.mensualidad_monto,
      desde: this.mensualidad_desde_date
    });

    this.mensualidad_monto = null;
    this.mensualidad_desde = "";

    if (this.crud.model.montos.length > 0) {
      $('.dataTables_empty').hide();
    }
    else {
      $('.dataTables_empty').show();
    }
  }

  onSaveClick() {
    //TAB GENERAL
    if (this.crud.model.esEmpresa) {
      if (this.crud.model.razonSocial === undefined || this.crud.model.razonSocial.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar la Razón social.");
        return;
      }

      if (this.crud.model.nombreFantasia === undefined || this.crud.model.nombreFantasia.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el Nombre fantasía.");
        return;
      }

      if (this.crud.model.numeroDocumento === undefined || this.crud.model.numeroDocumento.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el CUIT.");
        return;
      }

      if (!(/^[0-9]{2}-[0-9]{8}-[0-9]{1}$/.test(this.crud.model.numeroDocumento)))
      {
        this.confirmDialogService.showError("Debes ingresar un CUIT válido.");
        return;
      }
    }
    else
    {
      if (this.crud.model.nombre === undefined || this.crud.model.nombre.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el Nombre.");
        return;
      }

      if (this.crud.model.apellido === undefined || this.crud.model.apellido.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el Apellido.");
        return;
      }

      if (this.crud.model.numeroDocumento === undefined || this.crud.model.numeroDocumento.length === 0)
      {
        this.confirmDialogService.showError("Debes ingresar el DNI.");
        return;
      }

      if (!(/^[0-9]{8}$/.test(this.crud.model.numeroDocumento)))
      {
        this.confirmDialogService.showError("Debes ingresar un DNI válido.");
        return;
      }
    }

    if (this.crud.model.domicilio === undefined || this.crud.model.domicilio.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar el Domicilio.");
      return;
    }

    if (this.crud.model.entreCalles === undefined || this.crud.model.entreCalles.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar las Entre calles.");
      return;
    }

    if (this.crud.model.localidad === undefined || this.crud.model.localidad.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar la Localidad.");
      return;
    }

    if (this.crud.model.zona_encrypted_id === undefined || this.crud.model.zona_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar la Zona.");
      return;
    }


    //TAB CONTACTO
    //Todo opcional: No controlamos nada!
    

    //TAB MENSUALIDAD
    if (this.crud.model.montos.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar una mensualidad.");
      return;
    }


    this.apiService.DoPOST<ApiResult<ClienteDTO>>("clientes/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Cliente guardado correctamente.');
          this.routerService.navigate(['/clientes']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("clientes/basics/data" + (this.crud.isEditMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
    (response) => {
      if (!response.success) {
        this.confirmDialogService.showError(response.message);
      }
      else {
        if (response.data.entity !== null)
          this.crud.model = response.data.entity;

        this.zonas = response.data.zonas;


        if (this.crud.model.montos.length > 0) {
          $('.dataTables_empty').hide();
        }
        else {
          $('.dataTables_empty').show();
        }

        setTimeout(function() {
          (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
        }, 300);
      }
    },
    (errorMessage) => {
      this.confirmDialogService.showError(errorMessage);
    });
    
    
    this.dtMensualidad = {
      pagingType: 'simple_numbers',
      pageLength: 10,
      serverSide: false,
      processing: false,
      search: false,
      searching: false,
      info: false,
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
        callback({
          recordsTotal: this.crud.model.montos.length,
          recordsFiltered: this.crud.model.montos.length,
          data: [] //Siempre vacío para delegarle el render a Angular
        });
        
        if (this.crud.model.montos.length > 0) {
          $('.dataTables_empty').hide();
        }
        else {
          $('.dataTables_empty').show();
        }
      },
      columns: [
        { data: 'monto', orderable: false },
        { data: 'desde', orderable: true },
        { data: '', orderable: false } //BOTONERA
      ]
    };
  }

}
