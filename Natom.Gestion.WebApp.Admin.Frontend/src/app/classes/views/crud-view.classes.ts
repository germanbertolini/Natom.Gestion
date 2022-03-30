import { ActivatedRoute, NavigationEnd } from "@angular/router";

export class CRUDView<T> {
  id: string;
  mode: string;
  isNewMode: boolean;
  isEditMode: boolean;
  model: T;

  constructor(private route: ActivatedRoute) {
    this.id = this.route.snapshot.paramMap.get('id');
    if (this.route.snapshot.url.toString().endsWith("new")) {
      this.isNewMode = true;
      this.mode = "Nuevo";
    }
    else if (this.route.snapshot.url.toString().indexOf("edit") >= 0) {
      this.isEditMode = true;
      this.mode = "Editar";
    }
  }

}