import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ApiResult } from 'src/app/classes/dto/shared/api-result.dto';
import { ConfirmDialogService } from 'src/app/components/confirm-dialog/confirm-dialog.service';
import { ApiService } from 'src/app/services/api.service';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-login',
  styleUrls: ['./login.component.css'],
  templateUrl: './login.component.html'
})
export class LoginComponent {
    email : string = "";
    password : string = "";

    constructor(private _authService: AuthService,
                        private _apiService: ApiService,
                        private _router: Router,
                        private _confirmDialogService: ConfirmDialogService) {

    }

    onRecoverPasswordClick() {
        if (this.email === undefined || this.isValidEmail(this.email))
        {
            this._confirmDialogService.showError("Debes ingresar un Email válido.");
            return;
        }

        let confirmDialogService = this._confirmDialogService;
        let apiService = this._apiService;
        let email = this.email;

        confirmDialogService.showConfirm("Desea recuperar la clave del usuario?", function () {  
        apiService.DoPOST<ApiResult<any>>("users/recover?email=" + encodeURIComponent(email), {}, /*headers*/ null,
                                                (response) => {
                                                    if (!response.success) {
                                                        confirmDialogService.showError(response.message);
                                                    }
                                                    else {
                                                        confirmDialogService.showError("Email de recuperación enviado con éxito.");
                                                    }
                                                },
                                                (errorMessage) => {
                                                confirmDialogService.showError(errorMessage);
                                                });
        
        });
    }

    onLoginClick() {
        this._authService.Login(this.email, this.password,
            /* onSuccess */
            () => {
                this._router.navigate(['/']);
            },
            /* onError */
            (errorMessage) => {
                this._confirmDialogService.showError(errorMessage);
                this.password = "";
            });
    }

    isValidEmail(search:string):boolean
  {
      var regexp = new RegExp(/^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/);
      return !regexp.test(search);
  }
    
}