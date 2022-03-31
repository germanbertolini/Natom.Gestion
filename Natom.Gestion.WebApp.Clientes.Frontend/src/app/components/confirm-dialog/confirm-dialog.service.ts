import { Injectable } from '@angular/core';  
import { Router, NavigationStart } from '@angular/router';  
import { Observable } from 'rxjs';  
import { Subject } from 'rxjs';  
  
@Injectable() export class ConfirmDialogService {  
    private subject = new Subject<any>();  
  
    showConfirm(message: string, yesClickFn: () => void, noClickFn: () => void = undefined): any {  
        const that = this;
        (<any>$('[data-toggle="tooltip"]')).tooltip('hide');
        this.subject.next({  
            type: 'confirm',  
            text: message,  
            onYesClick(): any {  
                    that.subject.next(); // This will close the modal  
                    yesClickFn();  
                },  
            onNoClick(): any {  
                that.subject.next();  
                if (noClickFn !== undefined)
                    noClickFn();  
            }  
        });  
    }

    showOK(message: string, okClickFn: () => void = undefined): any {  
        const that = this;  
        (<any>$('[data-toggle="tooltip"]')).tooltip('hide');
        this.subject.next({  
            type: 'ok',  
            text: message,
            onOkClick(): any {  
                that.subject.next();
                if (okClickFn !== undefined)  
                    okClickFn();  
            }  
        });  
    }

    showError(message: string, okClickFn: () => void = undefined): any {  
        const that = this;  
        (<any>$('[data-toggle="tooltip"]')).tooltip('hide');
        this.subject.next({  
            type: 'error',  
            text: message,
            onOkClick(): any {  
                that.subject.next();
                if (okClickFn !== undefined)  
                    okClickFn();  
            }  
        });  
    }
  
    getMessage(): Observable<any> {  
        return this.subject.asObservable();  
    }  
}  