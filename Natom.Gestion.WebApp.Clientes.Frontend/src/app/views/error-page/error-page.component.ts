import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-error-page',
  styleUrls: ['./error-page.component.css'],
  templateUrl: './error-page.component.html'
})
export class ErrorPageComponent {
  error_message: string;
  
  constructor(private activatedroute: ActivatedRoute) {
    this.error_message = "";
  }
 
     ngOnInit() {
      this.activatedroute.data.subscribe(data => {
            this.error_message = data.message;
        });
     }

}