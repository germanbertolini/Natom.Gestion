import { Component } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { NavigationEnd, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  styleUrls: ['./app.component.css'],
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';
  sidebarOpened = false;
  isLoggedIn = false;
  
  constructor(private router: Router,
              private titleService: Title,
              private authService: AuthService) {

    router.events.subscribe((val) => {

        this.isLoggedIn = authService.getCurrentUser() !== null;

        //SETEAMOS EL TITULO DEL TAB
        this.titleService.setTitle("MundoMascota .:. Gestión");

        //SI HAY CAMBIO DE URL
        (<any>$('[data-toggle="tooltip"]')).tooltip('dispose');
        
        if (!((<any>$(".navbar-toggler-button")).is(".collapsed"))) {
          (<any>$(".navbar-collapse")).slideUp();
          (<any>$(".navbar-toggler-button")).removeClass("collapsed");
        }

        //LUEGO DEL CAMBIO
        if(val instanceof NavigationEnd) {
          //CERRAMOS EL SIDEBAR
          if (this.sidebarOpened) {
            (<any>$(".nav-menu-button")).click();
          }
        }
    });
    
  }

  toggleSidebar(expanded) {
    if (expanded) {
      $(".nav-menu-button").addClass("active");
      this.sidebarOpened = true;
    }
    else {
      $(".nav-menu-button").removeClass("active");
      this.sidebarOpened = false;
    }
  }
  
}
