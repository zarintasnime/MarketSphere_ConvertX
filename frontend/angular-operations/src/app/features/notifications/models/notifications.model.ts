export enum NotificationType {
  Information = 1,
  Warning = 2,
  ActionRequired = 3,
  Approval = 4,
  Expiry = 5,
  Sla = 6,
  System = 7,
}

export enum NotificationPriority {
  Low = 1,
  Normal = 2,
  High = 3,
  Critical = 4,
}

export interface NotificationItem {
  notificationID: number;
  notificationType: NotificationType;
  title: string;
  message: string;
  priority: NotificationPriority;
  referenceType: string | null;
  referenceID: number | null;
  isRead: boolean;
  createdAt: string;
  expiresAt: string | null;
  readAt: string | null;
}

export interface CreateNotificationRequest {
  userID: number;
  notificationType: NotificationType;
  title: string;
  message: string;
  priority: NotificationPriority;
  referenceType: string | null;
  referenceID: number | null;
  expiresAt: string | null;
}

export interface SystemCheckItem {
  checkCode: string;
  title: string;
  matchCount: number;
  message: string;
  referenceType: string | null;
  referenceID: number | null;
}

export interface SystemCheckRun {
  ranAt: string;
  notificationsCreated: number;
  results: readonly SystemCheckItem[];
}

export type NotificationViewFilter = 'all' | 'unread' | 'read';
