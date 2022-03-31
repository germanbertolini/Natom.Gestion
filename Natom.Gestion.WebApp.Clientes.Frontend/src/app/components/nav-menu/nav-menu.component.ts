import { Component } from '@angular/core';
import { Output, EventEmitter } from '@angular/core'; 
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent {
  
  constructor(private authService: AuthService) {
    
  }

  toggleMenu() {
    if (!((<any>$(".navbar-toggler-button")).is(".collapsed"))) {
      (<any>$(".navbar-collapse")).slideUp();
      (<any>$(".navbar-toggler-button")).removeClass("collapsed");
    }
    else {
      (<any>$(".navbar-collapse")).slideDown();
      (<any>$(".navbar-toggler-button")).addClass("collapsed");
    }
  }

}
