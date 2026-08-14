import type { Routes } from '@angular/router';

import { permissionGuard } from '../../core/auth/permission.guard';

export const PRODUCTS_ROUTES: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'list' },
  {
    path: 'categories',
    canActivate: [permissionGuard],
    data: { permissions: ['products.categories.view'] },
    loadComponent: () =>
      import('./pages/categories-page/categories-page.component').then(
        (m) => m.CategoriesPageComponent,
      ),
    title: 'Product Categories',
  },
  {
    path: 'brands',
    canActivate: [permissionGuard],
    data: { permissions: ['products.brands.view'] },
    loadComponent: () =>
      import('./pages/brands-page/brands-page.component').then((m) => m.BrandsPageComponent),
    title: 'Brands',
  },
  {
    path: 'list',
    canActivate: [permissionGuard],
    data: { permissions: ['products.products.view'] },
    loadComponent: () =>
      import('./pages/products-list-page/products-list-page.component').then(
        (m) => m.ProductsListPageComponent,
      ),
    title: 'Products',
  },
  {
    path: 'new',
    canActivate: [permissionGuard],
    data: { permissions: ['products.products.manage'] },
    loadComponent: () =>
      import('./pages/product-form-page/product-form-page.component').then(
        (m) => m.ProductFormPageComponent,
      ),
    title: 'Create Product',
  },
  {
    path: ':productID/edit',
    canActivate: [permissionGuard],
    data: { permissions: ['products.products.manage'] },
    loadComponent: () =>
      import('./pages/product-form-page/product-form-page.component').then(
        (m) => m.ProductFormPageComponent,
      ),
    title: 'Edit Product',
  },
  {
    path: 'skus',
    canActivate: [permissionGuard],
    data: { permissions: ['products.skus.view'] },
    loadComponent: () =>
      import('./pages/skus-page/skus-page.component').then((m) => m.SkusPageComponent),
    title: 'SKUs',
  },
  {
    path: 'price-lists',
    canActivate: [permissionGuard],
    data: { permissions: ['pricing.price_lists.view'] },
    loadComponent: () =>
      import('./pages/price-lists-page/price-lists-page.component').then(
        (m) => m.PriceListsPageComponent,
      ),
    title: 'Price Lists',
  },
  {
    path: 'price-lists/new',
    canActivate: [permissionGuard],
    data: { permissions: ['pricing.price_lists.manage'] },
    loadComponent: () =>
      import('./pages/price-list-details-page/price-list-details-page.component').then(
        (m) => m.PriceListDetailsPageComponent,
      ),
    title: 'Create Price List',
  },
  {
    path: 'price-lists/:priceListID',
    canActivate: [permissionGuard],
    data: { permissions: ['pricing.price_lists.view'] },
    loadComponent: () =>
      import('./pages/price-list-details-page/price-list-details-page.component').then(
        (m) => m.PriceListDetailsPageComponent,
      ),
    title: 'Price List Details',
  },
  {
    path: 'discount-rules',
    canActivate: [permissionGuard],
    data: { permissions: ['pricing.discount_rules.view'] },
    loadComponent: () =>
      import('./pages/discount-rules-page/discount-rules-page.component').then(
        (m) => m.DiscountRulesPageComponent,
      ),
    title: 'Discount Rules',
  },
];
