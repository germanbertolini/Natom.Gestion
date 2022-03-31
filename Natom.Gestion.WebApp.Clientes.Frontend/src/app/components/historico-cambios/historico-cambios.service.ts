import { Injectable } from '@angular/core';  
import { Router, NavigationStart } from '@angular/router';  
import { Observable } from 'rxjs';  
import { Subject } from 'rxjs';  
  
@Injectable() export class HistoricoCambiosService {  
    private subject = new Subject<any>();  
  
    show(entity: string, encrypted_id: string): any {  
        const that = this;
        (<any>$('[data-toggle="tooltip"]')).tooltip('hide');
        this.subject.next({  
            entity: entity,  
            encrypted_id: encrypted_id,  
            onCloseClick(): any {
                that.subject.next(); // This will close the modal   
            }  
        });  
    }
  
    getMessage(): Observable<any> {  
        return this.subject.asObservable();  
    }  
}  