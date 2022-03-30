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
import { SyncsClientesComponent } from "./views/clientes/syncs/syncs-clientes.component";
import { ABMSyncsClientesGuard } from "./guards/clientes/abm.syncs.clientes.guards";
import { DevicesSyncsClientesComponent } from "./views/clientes/syncs/devices/devices-syncs-clientes.component";
import { SyncsClientesNewComponent } from "./views/clientes/syncs/crud/syncs-clientes-new.component";
import { ABMPlacesAndGoalsGuard } from "./guards/placesgoals.guards";
import { PlacesComponent } from "./views/clientes/places/places.component";
import { PlaceCrudComponent } from "./views/clientes/places/crud/place-crud.component";
import { GoalsComponent } from "./views/clientes/goals/goals.component";
import { GoalCrudComponent } from "./views/clientes/goals/crud/goal-crud.component";
import { HorariosComponent } from "./views/clientes/horarios/horarios.component";
import { HorarioCrudComponent } from "./views/clientes/horarios/crud/horario-crud.component";
import { ABMHorariosGuard } from "./guards/horarios.guards";

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
    { canActivate: [ AuthGuard, ABMSyncsClientesGuard ], path: 'clientes/:id_cliente/syncs', component: SyncsClientesComponent },
    { canActivate: [ AuthGuard, ABMSyncsClientesGuard ], path: "clientes/:id_cliente/syncs/new", component: SyncsClientesNewComponent },
    { canActivate: [ AuthGuard, ABMSyncsClientesGuard ], path: "clientes/:id_cliente/syncs/devices/:id", component: DevicesSyncsClientesComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: 'clientes/:id_cliente/places', component: PlacesComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: "clientes/:id_cliente/places/new", component: PlaceCrudComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: "clientes/:id_cliente/places/edit/:id", component: PlaceCrudComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: 'clientes/:id_cliente/goals/:place_id', component: GoalsComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: "clientes/:id_cliente/goals/:place_id/new", component: GoalCrudComponent },
    { canActivate: [ AuthGuard, ABMPlacesAndGoalsGuard ], path: "clientes/:id_cliente/goals/:place_id/edit/:id", component: GoalCrudComponent },
    { canActivate: [ AuthGuard, ABMHorariosGuard ], path: 'clientes/:id_cliente/horarios/:place_id', component: HorariosComponent },
    { canActivate: [ AuthGuard, ABMHorariosGuard ], path: "clientes/:id_cliente/horarios/:place_id/new", component: HorarioCrudComponent },
    { canActivate: [ AuthGuard, ABMHorariosGuard ], path: "clientes/:id_cliente/horarios/:place_id/ver/:id", component: HorarioCrudComponent }
]

@NgModule({
    imports: [
        RouterModule.forRoot(appRoutes)
    ],
    exports: [ RouterModule ]
})
export class AppRoutingModule {

}