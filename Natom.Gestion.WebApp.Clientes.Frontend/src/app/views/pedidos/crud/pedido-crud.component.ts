import { HttpClient } from "@angular/common/http";
import { Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { DataTableDirective } from "angular-datatables/src/angular-datatables.directive";
import { NotifierService } from "angular-notifier";
import { fromEvent } from "rxjs";
import { debounceTime, distinctUntilChanged, map, mergeMap } from "rxjs/operators";
import { ClienteDTO } from "src/app/classes/dto/clientes/cliente.dto";
import { ClienteCtaCteResumeDTO } from "src/app/classes/dto/clientes/cta-cte/cliente-cta-cte-resume.dto";
import { PedidoDetalleDTO } from "src/app/classes/dto/pedidos/pedido-detalle.dto";
import { PedidoDTO } from "src/app/classes/dto/pedidos/pedido.dto";
import { RangoHorarioDTO } from "src/app/classes/dto/pedidos/rango-horario.dto";
import { ListaDePreciosDTO } from "src/app/classes/dto/precios/lista-de-precios.dto";
import { ProductoListDTO } from "src/app/classes/dto/productos/producto-list.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { AutocompleteResultDTO } from "src/app/classes/dto/shared/autocomplete-result.dto";
import { DepositoDTO } from "src/app/classes/dto/stock/deposito.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";
import { AuthService } from "src/app/services/auth.service";
import { FeatureFlagsService } from "src/app/services/feature-flags.service";
import { DataTableDTO } from "../../../classes/data-table-dto";

@Component({
  selector: 'app-pedido-crud',
  styleUrls: ['./pedido-crud.component.css'],
  templateUrl: './pedido-crud.component.html'
})

export class PedidoCrudComponent implements OnInit {
  @ViewChild(DataTableDirective, { static: false })
  dtElement: DataTableDirective;
  dtInstance: Promise<DataTables.Api>;
  dtDetalle: DataTables.Settings = {};
  crud: CRUDView<PedidoDTO>;
  productosSearch: ProductoListDTO[];
  clientesSearch: ClienteDTO[];
  depositos: DepositoDTO[];
  cliente_cta_cte: ClienteCtaCteResumeDTO;
  listasDePrecios: Array<ListaDePreciosDTO>;
  rangosHorarios: RangoHorarioDTO[];
  entrega_fecha_estimada: string;
  detalle_listaDePrecios_encrypted_id: string;
  detalle_listaDePrecios: string;
  detalle_precio: number;
  detalle_deposito_encrypted_id: string;
  detalle_deposito: string;
  detalle_producto_encrypted_id: string;
  detalle_producto: string;
  detalle_cantidad: number;
  detalle_stock_actual: number;
  detalle_peso_unitario_gramos: number;
  general_cliente: string;
  totalizador_monto: number;
  totalizador_peso_gramos: number;


  constructor(private apiService: ApiService,
              private authService: AuthService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService,
              private featureFlagsService: FeatureFlagsService) {
    this.cliente_cta_cte = null;
    this.crud = new CRUDView<PedidoDTO>(routeService);
    this.crud.model = new PedidoDTO();
    this.detalle_deposito_encrypted_id = "";
    this.detalle_listaDePrecios_encrypted_id = "";
    this.detalle_producto_encrypted_id = "";
    this.crud.model.retira_personalmente = false;
    this.detalle_cantidad = 0;
    this.detalle_precio = null;
    this.totalizador_monto = 0;
    this.totalizador_peso_gramos = 0;
    this.crud.model.entrega_estimada_rango_horario_encrypted_id = "";
    this.crud.model.detalle = new Array<PedidoDetalleDTO>();
    this.crud.model.fechaHora = new Date();
    this.crud.model.medio_de_pago = "";
    this.crud.model.usuario = authService.getCurrentUser().first_name;
  }

  decideClosure(event, datepicker) { const path = event.path.map(p => p.localName); if (!path.includes('ngb-datepicker')) { datepicker.close(); } }

  onFechaEntregaEstimadaChange(newValue) {
    this.crud.model.entrega_estimada_fecha = new Date(newValue.year, newValue.month - 1, newValue.day, 0, 0, 0, 0);
  }

  onClienteSearchSelectItem (cliente: ClienteDTO) {
    this.general_cliente = cliente.tipoDocumento + " " + (cliente.numeroDocumento || "") + " /// " + (cliente.esEmpresa ? (cliente.razonSocial || "") : (cliente.nombre || "") + " " + (cliente.apellido || "")) + " /// " + cliente.domicilio + ", " + (cliente.localidad || "");
      
    this.crud.model.cliente_encrypted_id = cliente.encrypted_id;
    this.crud.model.entrega_domicilio = cliente.domicilio;
    this.crud.model.entrega_entre_calles = cliente.entreCalles;
    this.crud.model.entrega_localidad = cliente.localidad;
    this.crud.model.entrega_telefono1 = cliente.contactoTelefono1;
    this.crud.model.entrega_telefono2 = cliente.contactoTelefono2;
    this.detalle_listaDePrecios_encrypted_id = cliente.lista_de_precios_encrypted_id;
    this.clientesSearch = undefined;

    if (this.crud.model.numero_remito === undefined || this.crud.model.numero_remito === null || this.crud.model.numero_remito.length === 0)
      this.getNextNumeroRemito();

    this.getCuentaCorrienteResume();
  }

  getCuentaCorrienteResume () {
    this.apiService.DoGET<ApiResult<ClienteCtaCteResumeDTO>>("clientes/cta_cte/resume?encryptedClienteId=" + encodeURIComponent(this.crud.model.cliente_encrypted_id), /*headers*/ null,
        (response) => {
          if (!response.success) {
            this.confirmDialogService.showError(response.message);
          }
          else {
            this.cliente_cta_cte = response.data;
          }
        },
        (errorMessage) => {
          this.confirmDialogService.showError(errorMessage);
        });
  }

  getNextNumeroRemito () {
    this.apiService.DoGET<ApiResult<string>>("pedidos/comprobantes/next", /*headers*/ null,
        (response) => {
          if (!response.success) {
            this.confirmDialogService.showError(response.message);
          }
          else {
            if (response.data !== null) {
              let _siguienteNumero = response.data;
              let _model = this.crud.model;
              this.confirmDialogService.showConfirm("¿Utilizar '" + _siguienteNumero + "' como Numero de remito?", function() {
                _model.numero_remito = _siguienteNumero;
              });
            }
          }
        },
        (errorMessage) => {
          this.confirmDialogService.showError(errorMessage);
        });
  }

  onAgregarDetalleClick() {
    if (this.detalle_listaDePrecios_encrypted_id === undefined || this.detalle_listaDePrecios_encrypted_id === null || this.detalle_listaDePrecios_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar una Lista de precios.");
      return;
    }

    if (this.detalle_producto_encrypted_id === undefined || this.detalle_producto_encrypted_id === null || this.detalle_producto_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes buscar un Producto.");
      return;
    }

    if (this.detalle_precio === null && this.detalle_listaDePrecios_encrypted_id !== "-1")
    {
      this.confirmDialogService.showError("El precio es inválido.");
      return;
    }

    if (this.detalle_deposito_encrypted_id === undefined || this.detalle_deposito_encrypted_id === null || this.detalle_deposito_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar un Depósito.");
      return;
    }

    if (this.detalle_cantidad === undefined || this.detalle_cantidad <= 0)
    {
      this.confirmDialogService.showError("Debes ingresar una Cantidad válida.");
      return;
    }

    if (this.detalle_cantidad > this.detalle_stock_actual && !this.featureFlagsService.current.stock.permitir_venta_con_stock_negativo)
    {
      this.confirmDialogService.showError("No hay stock disponible para esa cantidad.");
      return;
    }

    this.crud.model.detalle.push({
      encrypted_id: "",
      pedido_encrypted_id: "",
      producto_encrypted_id: this.detalle_producto_encrypted_id,
      producto_descripcion: this.detalle_producto,
      producto_peso_gramos: this.detalle_peso_unitario_gramos,
      cantidad: this.detalle_cantidad,
      entregado: null,
      deposito_encrypted_id: this.detalle_deposito_encrypted_id,
      deposito_descripcion: this.detalle_deposito,
      precio_lista_encrypted_id: this.detalle_listaDePrecios_encrypted_id,
      precio_descripcion: this.detalle_listaDePrecios,
      precio: this.detalle_precio
    });

    this.recalcularTotales();

    this.dataTableAdjustColumnSizing();

    this.detalle_producto_encrypted_id = "";
    this.detalle_producto = "";
    this.detalle_peso_unitario_gramos = 0;
    this.detalle_cantidad = 0;
    this.detalle_deposito_encrypted_id = "";
    this.detalle_deposito = "";
    this.detalle_precio = null;
    this.detalle_stock_actual = null;
  }

  onProductoSearchSelectItem (producto: ProductoListDTO) {
    this.detalle_producto_encrypted_id = producto.encrypted_id;
    this.detalle_producto = "(" + producto.codigo + ") " + producto.marca + " " + producto.descripcion;
    this.detalle_peso_unitario_gramos = producto.peso_unitario_gramos;
    this.productosSearch = undefined;
    this.consultarStock();
    this.consultarPrecio();
  }

  consultarPrecio() {
    if (this.detalle_producto_encrypted_id !== undefined && this.detalle_producto_encrypted_id.length !== 0 
      && this.detalle_listaDePrecios_encrypted_id !== undefined && this.detalle_listaDePrecios_encrypted_id.length !== 0 && this.detalle_listaDePrecios_encrypted_id !== "-1")
    {
      if (this.detalle_listaDePrecios_encrypted_id === "-1")
      {
        this.detalle_listaDePrecios = "< a definir en Venta >";
      }
      else
      {
        for (var i = 0; i < this.listasDePrecios.length; i++) {
          if (this.listasDePrecios[i].encrypted_id === this.detalle_listaDePrecios_encrypted_id) {
            this.detalle_listaDePrecios = this.listasDePrecios[i].descripcion;
          }
        }
      }
      

      this.apiService.DoGET<ApiResult<any>>("precios/get?producto=" + encodeURIComponent(this.detalle_producto_encrypted_id) + "&lista=" + encodeURIComponent(this.detalle_listaDePrecios_encrypted_id), /*headers*/ null,
          (response) => {
            if (!response.success) {
              this.confirmDialogService.showError(response.message);
            }
            else {
              this.detalle_precio = <number>response.data;
    
              setTimeout(function() {
                (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
              }, 300);
            }
          },
          (errorMessage) => {
            this.confirmDialogService.showError(errorMessage);
          });

    }
    else {
      this.detalle_precio = null;
    }
  }
  
  consultarStock() {
    if (this.detalle_producto_encrypted_id !== undefined && this.detalle_producto_encrypted_id.length !== 0 
      && this.detalle_deposito_encrypted_id !== undefined && this.detalle_deposito_encrypted_id.length !== 0)
    {
      for (var i = 0; i < this.depositos.length; i++) {
        if (this.depositos[i].encrypted_id === this.detalle_deposito_encrypted_id) {
          this.detalle_deposito = this.depositos[i].descripcion;
        }
      }

      this.apiService.DoGET<ApiResult<any>>("stock/quantity?producto=" + encodeURIComponent(this.detalle_producto_encrypted_id) + "&deposito=" + encodeURIComponent(this.detalle_deposito_encrypted_id), /*headers*/ null,
          (response) => {
            if (!response.success) {
              this.confirmDialogService.showError(response.message);
            }
            else {
              this.detalle_stock_actual = <number>response.data;
    
              setTimeout(function() {
                (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
              }, 300);
            }
          },
          (errorMessage) => {
            this.confirmDialogService.showError(errorMessage);
          });

    }
    else {
      this.detalle_stock_actual = null;
    }
  }

  onDetalleTabClick() {
    this.dataTableAdjustColumnSizing();
  }

  dataTableAdjustColumnSizing() {
    let _dtInstance = this.dtElement.dtInstance;
    setTimeout(function() {
      _dtInstance.then((dtInstance: DataTables.Api) => {
        dtInstance.columns.adjust();
      });
    }, 300);
  }

  onQuitarDetalleClick(index) {
    let detalleIndex = index;
    let detalleCollection = this.crud.model.detalle;
    let _this = this;

    this.confirmDialogService.showConfirm("¿Quitar?", function() {
      detalleCollection.splice(detalleIndex, 1);
      _this.recalcularTotales();
    });
  }

  recalcularTotales() {
    this.totalizador_monto = 0;
    this.totalizador_peso_gramos = 0;

    if (this.crud.model.detalle.length > 0) {
      for (let i = 0; i < this.crud.model.detalle.length; i ++) {
        this.totalizador_monto = this.totalizador_monto + (this.crud.model.detalle[i].precio * this.crud.model.detalle[i].cantidad);
        this.totalizador_peso_gramos = this.totalizador_peso_gramos + (this.crud.model.detalle[i].producto_peso_gramos * this.crud.model.detalle[i].cantidad);
      }
    }
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.cliente_encrypted_id === undefined || this.crud.model.cliente_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes buscar un Cliente.");
      return;
    }

    if (this.crud.model.detalle === undefined || this.crud.model.detalle.length === 0)
    {
      this.confirmDialogService.showError("Debes agregar al menos un producto en 'Detalle'.");
      return;
    }

    if (this.featureFlagsService.current.pedidos.fecha_y_hora_entrega_obligatorio) {
      if (this.crud.model.entrega_estimada_fecha === undefined || this.crud.model.entrega_estimada_fecha === null)
      {
        this.confirmDialogService.showError("Debes seleccionar una Fecha estimada de entrega.");
        return;
      }

      if (this.crud.model.entrega_estimada_rango_horario_encrypted_id === undefined || this.crud.model.entrega_estimada_rango_horario_encrypted_id.length === 0)
      {
        this.confirmDialogService.showError("Debes seleccionar el Rango horario de entrega.");
        return;
      }
    }

    if (this.crud.model.retira_personalmente === false) {
      if (this.crud.model.entrega_domicilio === undefined || this.crud.model.entrega_domicilio === null || this.crud.model.entrega_domicilio.length === 0)
      {
        this.confirmDialogService.showError("Debes indicar el Domicilio de entrega.");
        return;
      }

      if (this.crud.model.entrega_entre_calles === undefined || this.crud.model.entrega_entre_calles === null || this.crud.model.entrega_entre_calles.length === 0)
      {
        this.confirmDialogService.showError("Debes indicar las Entre Calles de entrega.");
        return;
      }

      if (this.crud.model.entrega_localidad === undefined || this.crud.model.entrega_localidad === null || this.crud.model.entrega_localidad.length === 0)
      {
        this.confirmDialogService.showError("Debes indicar la Localidad de entrega.");
        return;
      }

      if (this.crud.model.entrega_telefono1 === undefined || this.crud.model.entrega_telefono1 === null || this.crud.model.entrega_telefono1.length === 0)
      {
        this.confirmDialogService.showError("Debes indicar el Telefono 1 de entrega.");
        return;
      }
    }

    this.confirmDialogService.showConfirm("¿Ya está todo? Una vez guardado no se podrá modificar el pedido.", () => {
      this.apiService.DoPOST<ApiResult<PedidoDTO>>("pedidos/save", this.crud.model, /*headers*/ null,
            (response) => {
              if (!response.success) {
                this.confirmDialogService.showError(response.message);
              }
              else {
                this.notifierService.notify('success', 'Pedido generado correctamente.');
                this.routerService.navigate(['/pedidos']);
              }
            },
            (errorMessage) => {
              this.confirmDialogService.showError(errorMessage);
            });
    });
  }

  ngOnInit(): void {

    this.bindAutocompleteEvents<ProductoListDTO>("productoSearch", "productos/search?filter=", (data) => { this.productosSearch = data; }, () => { this.productosSearch = undefined; });
    this.bindAutocompleteEvents<ClienteDTO>("clienteSearch", "clientes/search?filter=", (data) => { this.clientesSearch = data; }, () => { this.clientesSearch = undefined; });

    this.dtDetalle = {
      pagingType: 'simple_numbers',
      pageLength: 100,
      serverSide: false,
      processing: false,
      info: false,
      paging: false,
      searching: false,
      scrollY: "270px",
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
          recordsTotal: this.crud.model.detalle.length,
          recordsFiltered: this.crud.model.detalle.length,
          data: [] //Siempre vacío para delegarle el render a Angular
        });
        
        $('.dataTables_empty').hide();
        
        setTimeout(function() {
          (<any>$("tbody tr").find('[data-toggle="tooltip"]')).tooltip();
        }, 300);
      },
      columns: [
        { data: 'producto', orderable: false },
        { data: 'deposito', orderable: false },
        { data: 'cantidad', orderable: false },
        { data: 'peso', orderable: false },
        { data: 'lista', orderable: false },
        { data: 'precio_unitario', orderable: false },
        { data: 'monto_total', orderable: false },
        { data: '', orderable: false } //BOTONERA
      ]
    };

    this.apiService.DoGET<ApiResult<any>>("pedidos/basics/data" + (this.crud.isEditMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          if (response.data.entity !== null)
            this.crud.model = response.data.entity;
        
            this.depositos = <DepositoDTO[]>response.data.depositos;
            this.rangosHorarios = <RangoHorarioDTO[]>response.data.rangos_horarios;
            this.listasDePrecios = <Array<ListaDePreciosDTO>>response.data.listasDePrecios;
            this.crud.model.numero = response.data.numero_pedido;
            this.crud.model.numero_remito = response.data.numero_remito;

            setTimeout(function() {
              (<any>$("#title-crud").find('[data-toggle="tooltip"]')).tooltip();
            }, 300);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
    
  }

  bindAutocompleteEvents<TDTO>(id: string, url: string, onResult: (data: TDTO[]) => void, onBlur: () => void) {
    let observableKeyUp = fromEvent(document.getElementById(id), 'keyup')
      .pipe (
        map(value => (<any>document.getElementById(id)).value),
        debounceTime(500),
        distinctUntilChanged(),
        mergeMap((search) => {
          return this.apiService.DoGETWithObservable(url + encodeURIComponent(search), /* headers */ null);
        })
      )
      observableKeyUp.subscribe((data) => {
        var result = <ApiResult<AutocompleteResultDTO<TDTO>>>data;
        if (!result.success) {
          this.confirmDialogService.showError("Se ha producido un error interno.");
        }
        else {
          onResult(result.data.results);
        }
    });

    let observableBlur = fromEvent(document.getElementById(id), 'blur');
    observableBlur.subscribe(() => {
      setTimeout(onBlur, 200);
    });
  }

}
