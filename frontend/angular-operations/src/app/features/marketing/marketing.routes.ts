import type { Routes } from '@angular/router';
import { permissionGuard } from '../../core/auth/permission.guard';

export const MARKETING_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'campaigns' },
  {
    path: 'campaigns',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.campaigns.view'] },
    loadComponent: () =>
      import('./pages/campaigns-list-page/campaigns-list-page.component').then(
        (m) => m.CampaignsListPageComponent,
      ),
    title: 'Campaigns',
  },
  {
    path: 'campaigns/new',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.campaigns.manage'] },
    loadComponent: () =>
      import('./pages/campaign-form-page/campaign-form-page.component').then(
        (m) => m.CampaignFormPageComponent,
      ),
    title: 'Create Campaign',
  },
  {
    path: 'campaigns/:campaignID/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.campaigns.manage'] },
    loadComponent: () =>
      import('./pages/campaign-form-page/campaign-form-page.component').then(
        (m) => m.CampaignFormPageComponent,
      ),
    title: 'Edit Campaign',
  },
  {
    path: 'campaigns/:campaignID',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.campaigns.view'] },
    loadComponent: () =>
      import('./pages/campaign-details-page/campaign-details-page.component').then(
        (m) => m.CampaignDetailsPageComponent,
      ),
    title: 'Campaign Details',
  },
  {
    path: 'visits',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.visits.view'] },
    loadComponent: () =>
      import('./pages/visits-list-page/visits-list-page.component').then(
        (m) => m.VisitsListPageComponent,
      ),
    title: 'Visits',
  },
  {
    path: 'visits/:visitID',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.visits.view'] },
    loadComponent: () =>
      import('./pages/visit-details-page/visit-details-page.component').then(
        (m) => m.VisitDetailsPageComponent,
      ),
    title: 'Visit Details',
  },
  {
    path: 'sampling',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.sampling.view'] },
    loadComponent: () =>
      import('./pages/sampling-page/sampling-page.component').then((m) => m.SamplingPageComponent),
    title: 'Sampling',
  },
  {
    path: 'feedback',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.feedback.view'] },
    loadComponent: () =>
      import('./pages/feedback-page/feedback-page.component').then((m) => m.FeedbackPageComponent),
    title: 'Feedback',
  },
  {
    path: 'market-observations',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.market_observations.view'] },
    loadComponent: () =>
      import('./pages/market-observations-page/market-observations-page.component').then(
        (m) => m.MarketObservationsPageComponent,
      ),
    title: 'Market Observations',
  },
  {
    path: 'bp-sell-out',
    canActivate: [permissionGuard],
    data: { permissions: ['marketing.bp_sell_out.view'] },
    loadComponent: () =>
      import('./pages/bp-sell-out-page/bp-sell-out-page.component').then(
        (m) => m.BpSellOutPageComponent,
      ),
    title: 'BP Sell-Out',
  },
];
