import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import { API_ENDPOINTS } from '../config/api-endpoints';
import type { ApiResponse } from '../models/api-response.model';
import { requireApiData } from '../models/api-response.model';

export interface FileAttachment {
  fileAttachmentID: number;
  referenceType: string;
  referenceID: number;
  attachmentCategory: string;
  fileName: string;
  storedFileName: string;
  fileUrl: string;
  mimeType: string;
  fileSizeBytes: number;
  fileHash: string;
  isEvidence: boolean;
  capturedAt: string | null;
  gps: string | null;
  verificationStatus: number;
  verifiedByUserID: number | null;
  uploadedByUserID: number;
  uploadedAt: string;
}

export interface UploadFileRequest {
  file: File;
  referenceType: string;
  referenceID: number;
  attachmentCategory: string;
  isEvidence?: boolean;
  capturedAt?: string | null;
  gps?: string | null;
}

@Injectable({ providedIn: 'root' })
export class FileTransferService {
  private readonly http = inject(HttpClient);

  getAttachments(
    referenceType: string,
    referenceID: number,
  ): Observable<readonly FileAttachment[]> {
    const params = new HttpParams()
      .set('referenceType', referenceType.trim())
      .set('referenceID', referenceID);

    return this.http
      .get<ApiResponse<readonly FileAttachment[]>>(API_ENDPOINTS.files.root, { params })
      .pipe(map(requireApiData));
  }

  upload(request: UploadFileRequest): Observable<number> {
    if (!request.file || request.file.size <= 0) {
      throw new Error('Select a non-empty file before uploading.');
    }

    if (!request.referenceType.trim()) {
      throw new Error('A file reference type is required.');
    }

    if (!request.attachmentCategory.trim()) {
      throw new Error('A file attachment category is required.');
    }

    const formData = new FormData();
    formData.append('file', request.file, request.file.name);
    formData.append('referenceType', request.referenceType.trim());
    formData.append('referenceID', String(request.referenceID));
    formData.append('attachmentCategory', request.attachmentCategory.trim());
    formData.append('isEvidence', String(request.isEvidence ?? false));

    if (request.capturedAt) {
      formData.append('capturedAt', request.capturedAt);
    }

    if (request.gps) {
      formData.append('gps', request.gps);
    }

    return this.http
      .post<ApiResponse<number>>(API_ENDPOINTS.files.root, formData)
      .pipe(map(requireApiData));
  }

  download(fileAttachmentID: number, fileName: string): Observable<void> {
    return this.http
      .get(API_ENDPOINTS.files.download(fileAttachmentID), {
        responseType: 'blob',
      })
      .pipe(
        map((blob) => {
          const url = URL.createObjectURL(blob);
          const anchor = document.createElement('a');
          anchor.href = url;
          anchor.download = this.sanitizeFileName(fileName);
          anchor.rel = 'noopener';
          anchor.style.display = 'none';
          document.body.appendChild(anchor);
          anchor.click();
          anchor.remove();
          window.setTimeout(() => URL.revokeObjectURL(url), 1000);
        }),
      );
  }

  private sanitizeFileName(fileName: string): string {
    const safeName = fileName
      .replace(/[\\/:*?"<>|]+/g, '_')
      .replace(/\s+/g, ' ')
      .trim();

    return safeName || 'download';
  }
}
