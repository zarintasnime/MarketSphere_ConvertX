import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

import { BreadcrumbComponent } from './breadcrumb.component';
import { SideNavigationComponent } from './side-navigation.component';
import { TopHeaderComponent } from './top-header.component';

@Component({
  selector: 'app-operations-shell',
  standalone: true,
  imports: [RouterOutlet, BreadcrumbComponent, SideNavigationComponent, TopHeaderComponent],
  templateUrl: './operations-shell.component.html',
  styleUrl: './operations-shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OperationsShellComponent {}
