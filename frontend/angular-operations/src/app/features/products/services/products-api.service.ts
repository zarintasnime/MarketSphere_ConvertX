import { HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, type Observable } from 'rxjs';

import { API_ENDPOINTS } from '../../../core/config/api-endpoints';
import { ApiClientService } from '../../../core/http/api-client.service';
import type { PagedRequest, PagedResult } from '../../../core/models/paged-result.model';
import type {
  Brand,
  PriceListDetails,
  PriceListListItem,
  PriceListStatus,
  PriceResolution,
  PriceResolutionRequest,
  ProductCategoryDetails,
  ProductCategoryListItem,
  ProductDetails,
  ProductListItem,
  SaveBrandRequest,
  SavePriceListRequest,
  SaveProductCategoryRequest,
  SaveProductRequest,
  SaveSKURequest,
  SaveStandardDiscountRuleRequest,
  SKUDetails,
  SKUListItem,
  StandardDiscountRule,
} from '../models/products.model';

interface RawSkuIdentifier {
  skuID?: number | string | null;
  skuId?: number | string | null;
  skuid?: number | string | null;
}

type RawSKUListItem = Omit<SKUListItem, 'skuID'> & RawSkuIdentifier;

type RawSKUDetails = Omit<SKUDetails, 'skuID'> & RawSkuIdentifier;

type RawProductDetails = Omit<ProductDetails, 'skus'> & {
  skus: readonly RawSKUListItem[];
};

@Injectable({
  providedIn: 'root',
})
export class ProductsApiService {
  private readonly api = inject(ApiClientService);

  getCategories(): Observable<readonly ProductCategoryListItem[]> {
    return this.api.get(API_ENDPOINTS.products.categories);
  }

  getCategory(productCategoryID: number): Observable<ProductCategoryDetails> {
    return this.api.get(API_ENDPOINTS.products.category(productCategoryID));
  }

  createCategory(request: SaveProductCategoryRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.products.categories, request);
  }

  updateCategory(
    productCategoryID: number,
    request: SaveProductCategoryRequest,
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.products.category(productCategoryID), request);
  }

  setCategoryStatus(productCategoryID: number, isActive: boolean): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.products.categoryStatus(productCategoryID), {
      isActive,
    });
  }

  getBrands(): Observable<readonly Brand[]> {
    return this.api.get(API_ENDPOINTS.products.brands);
  }

  createBrand(request: SaveBrandRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.products.brands, request);
  }

  updateBrand(brandID: number, request: SaveBrandRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.products.brand(brandID), request);
  }

  setBrandStatus(brandID: number, isActive: boolean): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.products.brandStatus(brandID), {
      isActive,
    });
  }

  getProducts(request: PagedRequest): Observable<PagedResult<ProductListItem>> {
    return this.api.get(API_ENDPOINTS.products.list, this.toPagedParams(request));
  }

  getProduct(productID: number): Observable<ProductDetails> {
    return this.api.get<RawProductDetails>(API_ENDPOINTS.products.byID(productID)).pipe(
      map((details) => ({
        ...details,
        skus: details.skus.map((item) => this.normalizeSKUListItem(item)),
      })),
    );
  }

  createProduct(request: SaveProductRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.products.root, request);
  }

  updateProduct(productID: number, request: SaveProductRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.products.byID(productID), request);
  }

  setProductStatus(productID: number, isActive: boolean): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.products.status(productID), {
      isActive,
    });
  }

  getSKUs(request: PagedRequest): Observable<PagedResult<SKUListItem>> {
    return this.api
      .get<PagedResult<RawSKUListItem>>(API_ENDPOINTS.products.skus, this.toPagedParams(request))
      .pipe(
        map((result) => ({
          ...result,
          items: result.items.map((item) => this.normalizeSKUListItem(item)),
        })),
      );
  }

  getSKU(skuID: number): Observable<SKUDetails> {
    return this.api
      .get<RawSKUDetails>(API_ENDPOINTS.products.sku(skuID))
      .pipe(map((details) => this.normalizeSKUDetails(details)));
  }

  createSKU(request: SaveSKURequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.products.skus, request);
  }

  updateSKU(skuID: number, request: SaveSKURequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.products.sku(skuID), request);
  }

  setSKUStatus(skuID: number, isActive: boolean): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.products.skuStatus(skuID), {
      isActive,
    });
  }

  getPriceLists(request: PagedRequest): Observable<PagedResult<PriceListListItem>> {
    return this.api.get(API_ENDPOINTS.pricing.priceLists, this.toPagedParams(request));
  }

  getPriceList(priceListID: number): Observable<PriceListDetails> {
    return this.api.get(API_ENDPOINTS.pricing.priceList(priceListID));
  }

  createPriceList(request: SavePriceListRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.pricing.priceLists, request);
  }

  updatePriceList(priceListID: number, request: SavePriceListRequest): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.pricing.priceList(priceListID), request);
  }

  changePriceListStatus(priceListID: number, status: PriceListStatus): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.pricing.priceListStatus(priceListID), {
      status,
    });
  }

  getDiscountRules(request: PagedRequest): Observable<PagedResult<StandardDiscountRule>> {
    return this.api.get(API_ENDPOINTS.pricing.discountRules, this.toPagedParams(request));
  }

  createDiscountRule(request: SaveStandardDiscountRuleRequest): Observable<number> {
    return this.api.post(API_ENDPOINTS.pricing.discountRules, request);
  }

  updateDiscountRule(
    standardDiscountRuleID: number,
    request: SaveStandardDiscountRuleRequest,
  ): Observable<boolean> {
    return this.api.put(API_ENDPOINTS.pricing.discountRule(standardDiscountRuleID), request);
  }

  setDiscountRuleStatus(standardDiscountRuleID: number, isActive: boolean): Observable<boolean> {
    return this.api.patch(API_ENDPOINTS.pricing.discountRuleStatus(standardDiscountRuleID), {
      isActive,
    });
  }

  resolvePrice(request: PriceResolutionRequest): Observable<PriceResolution> {
    return this.api.post(API_ENDPOINTS.pricing.resolve, request);
  }

  private normalizeSKUListItem(item: RawSKUListItem): SKUListItem {
    const { skuID, skuId, skuid, ...remainingProperties } = item;

    return {
      ...remainingProperties,
      skuID: this.resolveSkuID({
        skuID,
        skuId,
        skuid,
      }),
    };
  }

  private normalizeSKUDetails(details: RawSKUDetails): SKUDetails {
    const { skuID, skuId, skuid, ...remainingProperties } = details;

    return {
      ...remainingProperties,
      skuID: this.resolveSkuID({
        skuID,
        skuId,
        skuid,
      }),
    };
  }

  private resolveSkuID(item: RawSkuIdentifier): number {
    const rawValue = item.skuID ?? item.skuId ?? item.skuid;

    const normalizedValue = Number(rawValue);

    return Number.isInteger(normalizedValue) && normalizedValue > 0 ? normalizedValue : 0;
  }

  private toPagedParams(request: PagedRequest): HttpParams {
    let params = new HttpParams()
      .set('pageNumber', request.pageNumber)
      .set('pageSize', request.pageSize);

    if (request.search?.trim()) {
      params = params.set('search', request.search.trim());
    }

    if (request.sortBy) {
      params = params.set('sortBy', request.sortBy);
    }

    if (request.sortDirection) {
      params = params.set('sortDescending', request.sortDirection === 'desc');
    }

    return params;
  }
}
