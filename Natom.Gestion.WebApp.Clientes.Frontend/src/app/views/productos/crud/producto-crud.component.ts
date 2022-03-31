import { HttpClient } from "@angular/common/http";
import { Component, OnInit } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { NotifierService } from "angular-notifier";
import { MarcaDTO } from "src/app/classes/dto/marca.dto";
import { CategoriaProductoDTO } from "src/app/classes/dto/productos/producto-categoria.dto";
import { ProductoDTO } from "src/app/classes/dto/productos/producto.dto";
import { UnidadPesoDTO } from "src/app/classes/dto/productos/unidad-peso.dto";
import { ApiResult } from "src/app/classes/dto/shared/api-result.dto";
import { CRUDView } from "src/app/classes/views/crud-view.classes";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { ApiService } from "src/app/services/api.service";

@Component({
  selector: 'app-producto-crud',
  styleUrls: ['./producto-crud.component.css'],
  templateUrl: './producto-crud.component.html'
})

export class ProductoCrudComponent implements OnInit {

  crud: CRUDView<ProductoDTO>;
  marcas: Array<MarcaDTO>;
  unidades: Array<UnidadPesoDTO>;
  categorias: Array<CategoriaProductoDTO>;

  constructor(private apiService: ApiService,
              private routerService: Router,
              private routeService: ActivatedRoute,
              private notifierService: NotifierService,
              private confirmDialogService: ConfirmDialogService) {
                
    this.crud = new CRUDView<ProductoDTO>(routeService);
    this.crud.model = new ProductoDTO();
    this.crud.model.marca_encrypted_id = "";
    this.crud.model.unidadPeso_encrypted_id = "";
    this.crud.model.categoria_encrypted_id = "";
    this.crud.model.mueveStock = true;
  }

  onCancelClick() {
    this.confirmDialogService.showConfirm("¿Descartar cambios?", function() {
      window.history.back();
    });
  }

  onSaveClick() {
    if (this.crud.model.marca_encrypted_id === undefined || this.crud.model.marca_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar una Marca.");
      return;
    }
  
    // if (this.crud.model.codigo === undefined || this.crud.model.codigo.length === 0)
    // {
    //   this.confirmDialogService.showError("Debes ingresar un Código.");
    //   return;
    // }

    if (this.crud.model.descripcionCorta === undefined || this.crud.model.descripcionCorta.length === 0)
    {
      this.confirmDialogService.showError("Debes ingresar la Descripción Corta.");
      return;
    }

    if (this.crud.model.categoria_encrypted_id === undefined || this.crud.model.categoria_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar una Categoría.");
      return;
    }

    if (this.crud.model.pesoUnitario === undefined || this.crud.model.pesoUnitario === 0)
    {
      this.confirmDialogService.showError("Debes ingresar el Peso Unitario.");
      return;
    }

    if (this.crud.model.unidadPeso_encrypted_id === undefined || this.crud.model.unidadPeso_encrypted_id.length === 0)
    {
      this.confirmDialogService.showError("Debes seleccionar la Unidad de medida para el peso unitario.");
      return;
    }

    this.apiService.DoPOST<ApiResult<ProductoDTO>>("productos/save", this.crud.model, /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
          this.notifierService.notify('success', 'Producto guardado correctamente.');
          this.routerService.navigate(['/productos']);
        }
      },
      (errorMessage) => {
        this.confirmDialogService.showError(errorMessage);
      });
  }

  ngOnInit(): void {

    this.apiService.DoGET<ApiResult<any>>("productos/basics/data" + (this.crud.isEditMode ? "?encryptedId=" + encodeURIComponent(this.crud.id) : ""), /*headers*/ null,
      (response) => {
        if (!response.success) {
          this.confirmDialogService.showError(response.message);
        }
        else {
            if (response.data.entity !== null)
              this.crud.model = response.data.entity;
            this.marcas = response.data.marcas;
            this.unidades = response.data.unidades;
            this.categorias = response.data.categorias;

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
