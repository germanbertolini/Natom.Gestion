import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "./guards/auth.guards";
import { HomeComponent } from "./views/home/home.component";
import { LoginComponent } from "./views/login/login.component";
import { ABMUsuariosGuard } from "./guards/usuarios/abm.usuarios.guards";
import { UsersComponent } from "./views/users/users.component";
import { UserCrudComponent } from "./views/users/crud/user-crud.component";
import { ErrorPageComponent } from "./views/error-page/error-page.component";
import { UserConfirmComponent } from "./views/users/confirm/user-confirm.component";
import { ClientesComponent } from "./views/clientes/clientes.component";
import { ClienteCrudComponent } from "./views/clientes/crud/cliente-crud.component";
import { ABMClientesGuard } from "./guards/clientes/abm.clientes.guards";
import { UsuarioClientesCrudComponent } from "./views/clientes/usuarios/crud/usuario-clientes-crud.component";
import { UsuariosClientesComponent } from "./views/clientes/usuarios/usuarios-clientes.component";
import { ABMUsuariosClientesGuard } from "./guards/clientes/abm.usuarios.clientes.guards";

const appRoutes: Routes = [
    { path: 'login', component: LoginComponent, pathMatch: 'full' },
    { canActivate: [ AuthGuard ], path: '', component: HomeComponent, pathMatch: 'full' },
    { path: 'error-page', component: ErrorPageComponent, data: { message: "Se ha producido un error" } },
    { path: 'forbidden', component: ErrorPageComponent, data: { message: "No tienes permisos" } },
    { path: 'not-found', component: ErrorPageComponent, data: { message: "No se encontró la ruta solicitada" } },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: 'users', component: UsersComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: "users/new", component: UserCrudComponent },
    { canActivate: [ AuthGuard, ABMUsuariosGuard ], path: "users/edit/:id", component: UserCrudComponent },
    { canActivate: [ AuthGuard ], path: "users/confirm/:data", component: UserConfirmComponent },
    { canActivate: [ AuthGuard, ABMClientesGuard ], path: 'clientes', component: ClientesComponent },
    { canActivate: [ AuthGuard, ABMClientesGuard ], path: "clientes/new", component: ClienteCrudComponent },
    { canActivate: [ AuthGuard, ABMClientesGuard ], path: "clientes/edit/:id", component: ClienteCrudComponent },
    { canActivate: [ AuthGuard, ABMUsuariosClientesGuard ], path: 'clientes/:id_cliente/users', component: UsuariosClientesComponent },
    { canActivate: [ AuthGuard, ABMUsuariosClientesGuard ], path: "clientes/:id_cliente/users/new", component: UsuarioClientesCrudComponent },
    { canActivate: [ AuthGuard, ABMUsuariosClientesGuard ], path: "clientes/:id_cliente/users/edit/:id", component: UsuarioClientesCrudComponent },
]

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [ RouterModule ]
})
export class AppRoutingModule {

}