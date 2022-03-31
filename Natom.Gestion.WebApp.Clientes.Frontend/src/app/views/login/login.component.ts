import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmDialogService } from 'src/app/components/confirm-dialog/confirm-dialog.service';
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
                        private _router: Router,
                        private _confirmDialogService: ConfirmDialogService) {

    }

    onRecoverPasswordClick() {
        this._confirmDialogService.showOK("Para recuperar su cuenta debe contactarse con el Administrador de sistema para que le envíe el email de recuperación de clave.");
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
}