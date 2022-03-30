import { HttpHeaders } from '@angular/common/http';
import { toBase64String } from '@angular/compiler/src/output/source_map';
import { Injectable } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { HandledError } from '../classes/errors/handled.error';
import { LoginResult } from '../classes/dto/auth/login-result.dto';
import { ApiResult } from '../classes/dto/shared/api-result.dto';
import { UserDTO } from "../classes/dto/user.dto";
import { ApiService } from './api.service';
import { Router } from '@angular/router';
import { ConfirmDialogService } from '../components/confirm-dialog/confirm-dialog.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _current_user: UserDTO;
  private _current_token: string;
  private _current_permissions: Array<string>;
  
  constructor(private cookieService: CookieService,
              private apiService: ApiService,
              private routerService: Router,
              private confirmDialogService: ConfirmDialogService) {
    this._current_user = null;
    this._current_token = null;
    this._current_permissions = null;

    let userCookieData = this.cookieService.get('Admin.Auth.Current.User');
    if (userCookieData.length > 0) this._current_user = JSON.parse(atob(userCookieData));
    
    let tokenCookieData = this.cookieService.get('Admin.Auth.Current.Token');
    if (tokenCookieData.length > 0) this._current_token = JSON.parse(atob(tokenCookieData));

    let permissionsCookieData = this.cookieService.get('Admin.Auth.Current.Permissions');
    if (permissionsCookieData.length > 0) this._current_permissions = JSON.parse(atob(permissionsCookieData));

    apiService.SetOnForbiddenAction(() => {
      this.confirmDialogService.showError("La sesión expiró.", () => { location.href = this.getBaseURL(); });
      this.logout(true);
    });
  }

  public logout(cancelRedirect: boolean = false) {
    let _cancelRedirect = cancelRedirect;
    let _cookieService = this.cookieService;
    let _confirmDialogService = this.confirmDialogService;
    let _baseURL = this.getBaseURL();
    
    this.apiService.DoPOST<ApiResult<LoginResult>>("auth/logout", {}, /*headers*/ null,
                      (response) => {
                        if (!response.success) {
                          _confirmDialogService.showError(response.message);
                        }
                        else {
                          _cookieService.delete('Admin.Auth.Current.User');
                          _cookieService.delete('Admin.Auth.Current.Token');
                          _cookieService.delete('Admin.Auth.Current.Permissions');
                          _cookieService.delete('Admin.Authorization', "/");

                          setTimeout(function() {
                            if (!_cancelRedirect)
                              location.href = _baseURL;
                          }, 300);
                        }
                      },
                      (errorMessage) => {
                        _confirmDialogService.showError(errorMessage);
                      });
  }

  private getBaseURL() {
    var getUrl = window.location;
    var baseUrl = getUrl.protocol + "//" + getUrl.host + "/" + getUrl.pathname.split('/')[1];
    return baseUrl;
  }

  public getCurrentUser() {
    return this._current_user;
  }

  public getCurrentToken() {
    return this._current_token;
  }

  public getCurrentPermissions() {
    return this._current_permissions;
  }

  public can(permission: string) {
    return this.getCurrentPermissions().indexOf(permission.toLowerCase()) >= 0;
  }
  
  public Login(email: string, password: string, onSuccess: () => void, onError: (errorMessage: string) => void) {
    let response = new ApiResult<LoginResult>();
    let headers = new HttpHeaders();
    headers = headers.append('Authorization', 'Basic ' + btoa(email + ":" + password));

    this.apiService.DoPOST<ApiResult<LoginResult>>("auth/login", {}, headers,
                      (response) => {
                        if (!response.success) {
                          onError(response.message);
                        }
                        else {
                          this._current_user = response.data.user;
                          this._current_token = response.data.token;
                          this._current_permissions = response.data.permissions.map(function(permission) {
                            return permission.toLowerCase();
                          });

                          this.cookieService.set('Admin.Auth.Current.User', btoa(JSON.stringify(this._current_user)));
                          this.cookieService.set('Admin.Auth.Current.Token', btoa(JSON.stringify(this._current_token)));
                          this.cookieService.set('Admin.Auth.Current.Permissions', btoa(JSON.stringify(this._current_permissions)));
                          this.cookieService.set('Admin.Authorization', 'Bearer ' + this._current_token, null, "/");

                          onSuccess();
                        }
                      },
                      (errorMessage) => {
                        onError(errorMessage);
                      });
  }
}
