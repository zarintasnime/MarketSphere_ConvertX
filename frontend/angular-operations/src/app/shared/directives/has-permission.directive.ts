import { Directive, Input, TemplateRef, ViewContainerRef, effect, inject } from '@angular/core';

import { AuthService } from '../../core/auth/auth.service';

@Directive({
  selector: '[appHasPermission]',
  standalone: true,
})
export class HasPermissionDirective {
  private readonly authService = inject(AuthService);
  private readonly templateRef = inject(TemplateRef<unknown>);
  private readonly viewContainer = inject(ViewContainerRef);

  private permissionCodes: readonly string[] = [];
  private match: 'all' | 'any' = 'all';
  private isRendered = false;

  @Input()
  set appHasPermission(value: string | readonly string[]) {
    this.permissionCodes = typeof value === 'string' ? [value] : value;
    this.updateView();
  }

  @Input()
  set appHasPermissionMatch(value: 'all' | 'any') {
    this.match = value;
    this.updateView();
  }

  constructor() {
    effect(() => {
      this.authService.currentUser();
      this.updateView();
    });
  }

  private updateView(): void {
    const isAllowed =
      this.permissionCodes.length === 0 ||
      (this.match === 'any'
        ? this.authService.hasAnyPermission(this.permissionCodes)
        : this.authService.hasEveryPermission(this.permissionCodes));

    if (isAllowed && !this.isRendered) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.isRendered = true;
      return;
    }

    if (!isAllowed && this.isRendered) {
      this.viewContainer.clear();
      this.isRendered = false;
    }
  }
}
