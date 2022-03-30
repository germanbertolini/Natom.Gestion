import { Component } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
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
              private authService: AuthService) {

    router.events.subscribe((val) => {

        this.isLoggedIn = authService.getCurrentUser() !== null;

        //SI HAY CAMBIO DE URL
        (<any>$('[data-toggle="tooltip"]')).tooltip('dispose');

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
