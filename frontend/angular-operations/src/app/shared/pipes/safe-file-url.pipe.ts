import { Pipe, PipeTransform, inject } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Pipe({
  name: 'safeFileUrl',
  standalone: true,
})
export class SafeFileUrlPipe implements PipeTransform {
  private readonly sanitizer = inject(DomSanitizer);

  transform(value: string | null | undefined): SafeUrl | null {
    if (!value) return null;
    return this.sanitizer.bypassSecurityTrustUrl(value);
  }
}
