import type { Routes } from '@angular/router';

import { permissionGuard } from '../../core/auth/permission.guard';

export const CRM_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'clients' },
  {
    path: 'clients',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.clients.view'] },
    loadComponent: () =>
      import('./pages/clients-list-page/clients-list-page.component').then(
        (m) => m.ClientsListPageComponent,
      ),
    title: 'Clients',
  },
  {
    path: 'clients/new',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.clients.manage'] },
    loadComponent: () =>
      import('./pages/client-form-page/client-form-page.component').then(
        (m) => m.ClientFormPageComponent,
      ),
    title: 'Create Client',
  },
  {
    path: 'clients/:clientID/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.clients.manage'] },
    loadComponent: () =>
      import('./pages/client-form-page/client-form-page.component').then(
        (m) => m.ClientFormPageComponent,
      ),
    title: 'Edit Client',
  },
  {
    path: 'clients/:clientID',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.clients.view'] },
    loadComponent: () =>
      import('./pages/client-details-page/client-details-page.component').then(
        (m) => m.ClientDetailsPageComponent,
      ),
    title: 'Client Details',
  },
  {
    path: 'leads',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.leads.view'] },
    loadComponent: () =>
      import('./pages/leads-list-page/leads-list-page.component').then(
        (m) => m.LeadsListPageComponent,
      ),
    title: 'Leads',
  },
  {
    path: 'leads/new',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.leads.manage'] },
    loadComponent: () =>
      import('./pages/lead-form-page/lead-form-page.component').then(
        (m) => m.LeadFormPageComponent,
      ),
    title: 'Create Lead',
  },
  {
    path: 'leads/:leadID/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.leads.manage'] },
    loadComponent: () =>
      import('./pages/lead-form-page/lead-form-page.component').then(
        (m) => m.LeadFormPageComponent,
      ),
    title: 'Edit Lead',
  },
  {
    path: 'leads/:leadID',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.leads.view'] },
    loadComponent: () =>
      import('./pages/lead-details-page/lead-details-page.component').then(
        (m) => m.LeadDetailsPageComponent,
      ),
    title: 'Lead Details',
  },
  {
    path: 'tasks',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.tasks.view'] },
    loadComponent: () =>
      import('./pages/tasks-page/tasks-page.component').then((m) => m.TasksPageComponent),
    title: 'CRM Tasks',
  },
  {
    path: 'opportunities',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.opportunities.view'] },
    loadComponent: () =>
      import('./pages/opportunities-list-page/opportunities-list-page.component').then(
        (m) => m.OpportunitiesListPageComponent,
      ),
    title: 'Opportunities',
  },
  {
    path: 'opportunities/new',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.opportunities.manage'] },
    loadComponent: () =>
      import('./pages/opportunity-details-page/opportunity-details-page.component').then(
        (m) => m.OpportunityDetailsPageComponent,
      ),
    title: 'Create Opportunity',
  },
  {
    path: 'opportunities/:opportunityID',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.opportunities.view'] },
    loadComponent: () =>
      import('./pages/opportunity-details-page/opportunity-details-page.component').then(
        (m) => m.OpportunityDetailsPageComponent,
      ),
    title: 'Opportunity Details',
  },
  {
    path: 'quotations',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.quotations.view'] },
    loadComponent: () =>
      import('./pages/quotations-list-page/quotations-list-page.component').then(
        (m) => m.QuotationsListPageComponent,
      ),
    title: 'Quotations',
  },
  {
    path: 'quotations/new',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.quotations.manage'] },
    loadComponent: () =>
      import('./pages/quotation-form-page/quotation-form-page.component').then(
        (m) => m.QuotationFormPageComponent,
      ),
    title: 'Create Quotation',
  },
  {
    path: 'quotations/:quotationID/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.quotations.manage'] },
    loadComponent: () =>
      import('./pages/quotation-form-page/quotation-form-page.component').then(
        (m) => m.QuotationFormPageComponent,
      ),
    title: 'Edit Quotation',
  },
  {
    path: 'quotations/:quotationID',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.quotations.view'] },
    loadComponent: () =>
      import('./pages/quotation-details-page/quotation-details-page.component').then(
        (m) => m.QuotationDetailsPageComponent,
      ),
    title: 'Quotation Details',
  },
  {
    path: 'complaints',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.complaints.view'] },
    loadComponent: () =>
      import('./pages/complaints-list-page/complaints-list-page.component').then(
        (m) => m.ComplaintsListPageComponent,
      ),
    title: 'Complaints',
  },
  {
    path: 'complaints/new',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.complaints.manage'] },
    loadComponent: () =>
      import('./pages/complaint-details-page/complaint-details-page.component').then(
        (m) => m.ComplaintDetailsPageComponent,
      ),
    title: 'Create Complaint',
  },
  {
    path: 'complaints/:complaintID',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.complaints.view'] },
    loadComponent: () =>
      import('./pages/complaint-details-page/complaint-details-page.component').then(
        (m) => m.ComplaintDetailsPageComponent,
      ),
    title: 'Complaint Details',
  },
  {
    path: 'reactivation',
    canActivate: [permissionGuard],
    data: { permissions: ['crm.reactivation.view'] },
    loadComponent: () =>
      import('./pages/reactivation-page/reactivation-page.component').then(
        (m) => m.ReactivationPageComponent,
      ),
    title: 'Reactivation',
  },
];
