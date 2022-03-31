import { Injectable } from '@angular/core'; 
import { Observable } from 'rxjs';  
import { Subject } from 'rxjs';  
  
@Injectable() export class SpinnerLoadingService {  
    private subject = new Subject<any>();  
  
    show(): any {  
        const that = this;
        (<any>$('[data-toggle="tooltip"]')).tooltip('hide');
        this.subject.next(true);
    }

    hide(): any {  
        const that = this;
        (<any>$('[data-toggle="tooltip"]')).tooltip('hide');
        this.subject.next(false);
    }
  
    getMessage(): Observable<any> {  
        return this.subject.asObservable();  
    }  
}  