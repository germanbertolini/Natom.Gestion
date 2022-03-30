import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from "@angular/router";
import { Observable } from "rxjs";
import { ConfirmDialogService } from "src/app/components/confirm-dialog/confirm-dialog.service";
import { AuthService } from "src/app/services/auth.service";

@Injectable({
    providedIn: 'root'
})
export class ABMPlacesAndGoalsGuard implements CanActivate {

    constructor(private _authService: AuthService,
                private confirmDialogService: ConfirmDialogService) { }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Observable<boolean | UrlTree> |
        Promise<boolean | UrlTree> | boolean | UrlTree {

        let permissions = this._authService.getCurrentPermissions();
        let containsPermission = permissions.indexOf("abm_clientes_places_goals") >= 0 || permissions.indexOf("*") >= 0;

        if (!containsPermission)
            this.confirmDialogService.showError("¡Ups! No tienes permisos");

        return containsPermission;
    }
}