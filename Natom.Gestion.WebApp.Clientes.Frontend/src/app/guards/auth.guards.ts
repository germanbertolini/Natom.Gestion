import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate {

    constructor(private _authService: AuthService,
                private _router: Router) {
        
    }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot): Observable<boolean | UrlTree> |
        Promise<boolean | UrlTree> | boolean | UrlTree {

        if (this.getCurrentUrl(route) === "/users/confirm/:data")
        {
            return true;
        }

        if (this._authService.getCurrentUser() === null)
        {
            this._router.navigate(['/login']);
            return false;
        }

        //GERMAN 24/09/2021: LO DEJO COMENTADO ASI LA GENTE LO PUEDE IR PROBANDO!
        //if (this._authService.getCurrentPermissions().indexOf(state.url.toLowerCase()) < 0)
        //{
        //    this._router.navigate(['/forbidden']);
        //    return false;
        //}

        return true;

    }

    getCurrentUrl(route: ActivatedRouteSnapshot): string {
        return '/' + route.pathFromRoot
            .filter(v => v.routeConfig)
            .map(v => v.routeConfig!.path)
            .join('/');
    }
}