import { Injectable } from '@angular/core';

import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root',
})
export class PreventUnsavedChanges {
  canDeactivate(
    component: MemberEditComponent,
    currentRoute: import('@angular/router').ActivatedRouteSnapshot,
    currentState: import('@angular/router').RouterStateSnapshot,
    nextState?: import('@angular/router').RouterStateSnapshot
  ):
    | boolean
    | import('@angular/router').UrlTree
    | import('rxjs').Observable<boolean | import('@angular/router').UrlTree>
    | Promise<boolean | import('@angular/router').UrlTree> {
    if (component.editFrom.dirty) {
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    }
    return true;
  }
}
