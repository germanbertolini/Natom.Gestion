import { Component, Input, OnInit } from '@angular/core';
import { SpinnerLoadingService } from './spinner-loading.service';
  
@Component({  
    selector: 'app-spinner-loading',  
    templateUrl: 'spinner-loading.component.html',  
    styleUrls: ['spinner-loading.component.css']  
})  
  
export class SpinnerLoadingComponent implements OnInit {  
    show: boolean = false;

    constructor(  
        private spinnerLoadingService: SpinnerLoadingService  
    ) { }  
  
    ngOnInit(): any {  
       /** 
        *   This function waits for a message from alert service, it gets 
        *   triggered when we call this from any other component 
        */  
        this.spinnerLoadingService.getMessage().subscribe(show => {  
            this.show = show;  
        });  
    }  
}  