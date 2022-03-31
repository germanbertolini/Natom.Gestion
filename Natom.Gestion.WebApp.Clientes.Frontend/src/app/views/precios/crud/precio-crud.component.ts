import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { fromEvent } from 'rxjs';
import { map, distinctUntilChanged, debounceTime, mergeMap } from 'rxjs/operators';
import { PrecioDTO } from "src/app/classes/dto/precios/precio.dto";
import { ProductoListDTO } from "src/app/classes/dto/productos/producto-list.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { DataTableDTO } from "../../../classes/data-table-dto";
import { ApiService } from "src/app/services/api.service";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { AutocompleteResultDTO } from "src/app/classes/dto/shared/autocomplete-result.dto";
import { ListaDePreciosDTO } from "src/app/classes/dto/precios/lista-de-precios.dto";

@Component({
  selector: 'app-precio-crud',
  styleUrls: ['./precio-crud.component.css'],
  templateUrl: './precio-crud.component.html'
})

export class PrecioCrudComponent implements OnInit {

  crud: CRUDView<PrecioDTO>;
  productosSearch: ProductoListDTO[];
  ListasDePrecios: Array<ListaDePreciosDTO>;


  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<PrecioDTO>(routeService);
    this.crud.model = new PrecioDTO();
    this.crud.model.listaDePrecios_encrypted_id = "";
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.producto_encrypted_id === undefined || this.crud.model.producto_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar un Producto.");
      return;
    }

    if (this.crud.model.listaDePrecios_encrypted_id === undefined || this.crud.model.listaDePrecios_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar una Lista de precios.");
      return;
    }

    if (this.crud.model.precio === undefined || this.crud.model.precio <= 0)
    {
      this.confirmDialogService.showError("Debes ingresar un Precio válido.");
      return;
    }

    let saveAction = () => {
      this.apiService.DoPOST<ApiResult<PrecioDTO>>("precios/save", this.crud.model, /*headers*/ null,
        (response) => {
          if (!response.success) {
            this.confirmDialogService.showError(response.message);
          }
          else {
            this.notifierService.notify('success', 'Precio guardado correctamente.');
            this.routerService.navigate(['/precios']);
          }
        },
        (errorMessage) => {
          this.confirmDialogService.showError(errorMessage);
        });
    };

    if (this.getSelectedPrecio().esPorcentual === true) {
      this.confirmDialogService.showConfirm("La lista de precios seleccionada es Porcentual por lo que este nuevo precio reemplazará el precio heredado 'porcentual'. ¿Desea continuar?", saveAction);
    }
    else {
      saveAction();
    }
  }

  getPrecioActual() {
    if (this.crud.model.producto_encrypted_id !== undefined && this.crud.model.producto_encrypted_id !== null && this.crud.model.producto_encrypted_id.length > 0
          && this.crud.model.listaDePrecios_encrypted_id !== undefined && this.crud.model.listaDePrecios_encrypted_id !== null && this.crud.model.listaDePrecios_encrypted_id.length > 0)
    {
        this.apiService.DoGET<ApiResult<any>>("precios/get?producto=" + encodeURIComponent(this.crud.model.producto_encrypted_id) + "&lista=" + encodeURIComponent(this.crud.model.listaDePrecios_encrypted_id), /*headers*/ null,
              (response) => {
                if (!response.success) {
                  this.confirmDialogService.showError(response.message);
                }
                else {
                  this.crud.model.precio = <number>response.data;
        
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

  onListaDePreciosChange() {
    this.getPrecioActual();
  }

  onProductoSearchSelectItem (producto: ProductoListDTO) {
    this.crud.model.producto_encrypted_id = producto.encrypted_id;
    this.crud.model.producto = "(" + producto.codigo + ") " + producto.marca + " " + producto.descripcion;
    this.productosSearch = undefined;
    this.getPrecioActual();
  }

  ngOnInit(): void {

    this.bindAutocompleteEvents<ProductoListDTO>("productoSearch", "productos/search?filter=", (data) => { this.productosSearch = data; }, () => { this.productosSearch = undefined; });

    this.apiService.DoGET<ApiResult<any>>("precios/basics/data" + (this.crud.isRenewMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          if (response.data.entity !== null)
            this.crud.model = response.data.entity;
          
          this.ListasDePrecios = <Array<ListaDePreciosDTO>>response.data.listasDePrecios;

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
  
  getSelectedPrecio(): ListaDePreciosDTO {
    let selected : ListaDePreciosDTO = null;
    if (this.crud.model.listaDePrecios_encrypted_id !== undefined && this.crud.model.listaDePrecios_encrypted_id !== null && this.crud.model.listaDePrecios_encrypted_id.length > 0) {
      for (let i = 0; i < this.ListasDePrecios.length; i ++) {
        if (this.ListasDePrecios[i].encrypted_id === this.crud.model.listaDePrecios_encrypted_id) {
          selected = this.ListasDePrecios[i];
          break;
        }
      }
    }
    return selected;
  }
}
