import { ChangeDetectionStrategy, Component, computed, inject } from '@angular/core';
import { NavigationEnd, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { filter, map, startWith } from 'rxjs';

@Component({
  selector: 'app-breadcrumb',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './breadcrumb.component.html',
  styleUrl: './breadcrumb.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BreadcrumbComponent {
  private readonly router = inject(Router);

  private readonly url = toSignal(
    this.router.events.pipe(
      filter((event): event is NavigationEnd => event instanceof NavigationEnd),
      map((event) => event.urlAfterRedirects),
      startWith(this.router.url),
    ),
    { initialValue: this.router.url },
  );

  protected readonly segments = computed(() =>
    this.url()
      .split('?')[0]
      .split('/')
      .filter(Boolean)
      .map((segment, index, items) => ({
        label: segment
          .split('-')
          .map((part) => part.charAt(0).toUpperCase() + part.slice(1))
          .join(' '),
        route: '/' + items.slice(0, index + 1).join('/'),
      })),
  );
}
