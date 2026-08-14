export enum ProductCategoryType {
  Standard = 1,
  Promotional = 2,
  Service = 3,
}

export enum ProductType {
  FinishedGood = 1,
  Sample = 2,
  PromotionalItem = 3,
  Service = 4,
}

export enum SalesChannel {
  GeneralTrade = 1,
  ModernTrade = 2,
  BusinessPartner = 3,
  Institutional = 4,
  Online = 5,
}

export enum PriceListStatus {
  Draft = 1,
  Active = 2,
  Inactive = 3,
  Expired = 4,
}

export interface SelectOption<T extends number = number> {
  value: T;
  label: string;
}

export const PRODUCT_CATEGORY_TYPE_OPTIONS: readonly SelectOption<ProductCategoryType>[] = [
  { value: ProductCategoryType.Standard, label: 'Standard' },
  { value: ProductCategoryType.Promotional, label: 'Promotional' },
  { value: ProductCategoryType.Service, label: 'Service' },
];

export const PRODUCT_TYPE_OPTIONS: readonly SelectOption<ProductType>[] = [
  { value: ProductType.FinishedGood, label: 'Finished Good' },
  { value: ProductType.Sample, label: 'Sample' },
  { value: ProductType.PromotionalItem, label: 'Promotional Item' },
  { value: ProductType.Service, label: 'Service' },
];

export const SALES_CHANNEL_OPTIONS: readonly SelectOption<SalesChannel>[] = [
  { value: SalesChannel.GeneralTrade, label: 'General Trade' },
  { value: SalesChannel.ModernTrade, label: 'Modern Trade' },
  { value: SalesChannel.BusinessPartner, label: 'Business Partner' },
  { value: SalesChannel.Institutional, label: 'Institutional' },
  { value: SalesChannel.Online, label: 'Online' },
];

export const PRICE_LIST_STATUS_OPTIONS: readonly SelectOption<PriceListStatus>[] = [
  { value: PriceListStatus.Draft, label: 'Draft' },
  { value: PriceListStatus.Active, label: 'Active' },
  { value: PriceListStatus.Inactive, label: 'Inactive' },
  { value: PriceListStatus.Expired, label: 'Expired' },
];

export function optionLabel<T extends number>(
  options: readonly SelectOption<T>[],
  value: T,
): string {
  return options.find((option) => option.value === value)?.label ?? `Value ${value}`;
}

export interface ProductCategoryListItem {
  productCategoryID: number;
  parentProductCategoryID: number | null;
  categoryCode: string;
  categoryName: string;
  categoryType: ProductCategoryType;
  isActive: boolean;
}

export interface ProductCategoryDetails extends ProductCategoryListItem {
  children: readonly ProductCategoryListItem[];
}

export interface SaveProductCategoryRequest {
  parentProductCategoryID: number | null;
  categoryCode: string;
  categoryName: string;
  categoryType: ProductCategoryType;
  isActive: boolean;
}

export interface Brand {
  brandID: number;
  brandCode: string;
  brandName: string;
  ownerCompanyName: string | null;
  isCustomerFacing: boolean;
  isActive: boolean;
}

export interface SaveBrandRequest {
  brandCode: string;
  brandName: string;
  ownerCompanyName: string | null;
  isCustomerFacing: boolean;
  isActive: boolean;
}

export interface ProductListItem {
  productID: number;
  productCode: string;
  productName: string;
  categoryName: string;
  brandName: string;
  productType: ProductType;
  requiresBatch: boolean;
  requiresExpiryDate: boolean;
  isActive: boolean;
}

export interface ProductDetails {
  productID: number;
  productCode: string;
  productCategoryID: number;
  categoryName: string;
  brandID: number;
  brandName: string;
  productName: string;
  productType: ProductType;
  description: string | null;
  requiresBatch: boolean;
  requiresExpiryDate: boolean;
  isActive: boolean;
  skus: readonly SKUListItem[];
}

export interface SaveProductRequest {
  productCode: string;
  productCategoryID: number;
  brandID: number;
  productName: string;
  productType: ProductType;
  description: string | null;
  requiresBatch: boolean;
  requiresExpiryDate: boolean;
  isActive: boolean;
}

export interface SKUListItem {
  skuID: number;
  productID: number;
  productName: string;
  skuCode: string;
  skuName: string;
  size: string | null;
  unit: string;
  barcode: string | null;
  mrp: number;
  standardTradePrice: number;
  isActive: boolean;
}

export interface SKUDetails extends SKUListItem {
  productCode: string;
}

export interface SaveSKURequest {
  productID: number;
  skuCode: string;
  skuName: string;
  size: string | null;
  unit: string;
  barcode: string | null;
  mrp: number;
  standardTradePrice: number;
  isActive: boolean;
}

export interface PriceListItem {
  priceListItemID: number;
  skuID: number;
  skuCode: string;
  skuName: string;
  unitPrice: number;
  maximumDiscountPercent: number;
  minimumOrderQuantity: number | null;
}

export interface PriceListListItem {
  priceListID: number;
  priceListCode: string;
  priceListName: string;
  channel: SalesChannel;
  clientSegmentID: number | null;
  clientSegmentName: string | null;
  effectiveFrom: string;
  effectiveTo: string | null;
  currencyCode: string;
  status: PriceListStatus;
}

export interface PriceListDetails extends PriceListListItem {
  items: readonly PriceListItem[];
}

export interface SavePriceListItemRequest {
  skuID: number;
  unitPrice: number;
  maximumDiscountPercent: number;
  minimumOrderQuantity: number | null;
}

export interface SavePriceListRequest {
  priceListCode: string;
  priceListName: string;
  channel: SalesChannel;
  clientSegmentID: number | null;
  effectiveFrom: string;
  effectiveTo: string | null;
  currencyCode: string;
  items: readonly SavePriceListItemRequest[];
}

export interface StandardDiscountRule {
  standardDiscountRuleID: number;
  ruleName: string;
  channel: SalesChannel;
  clientSegmentID: number | null;
  skuID: number | null;
  productCategoryID: number | null;
  minQuantity: number | null;
  maxDiscountPercent: number;
  requiresApproval: boolean;
  effectiveFrom: string;
  effectiveTo: string | null;
  isActive: boolean;
}

export interface SaveStandardDiscountRuleRequest {
  ruleName: string;
  channel: SalesChannel;
  clientSegmentID: number | null;
  skuID: number | null;
  productCategoryID: number | null;
  minQuantity: number | null;
  maxDiscountPercent: number;
  requiresApproval: boolean;
  effectiveFrom: string;
  effectiveTo: string | null;
  isActive: boolean;
}

export interface PriceResolutionRequest {
  skuID: number;
  channel: SalesChannel;
  clientSegmentID: number | null;
  quantity: number;
  priceDate: string;
}

export interface PriceResolution {
  skuID: number;
  priceListID: number;
  priceListItemID: number;
  unitPrice: number;
  maximumPriceListDiscountPercent: number;
  maximumStandardDiscountPercent: number;
  effectiveMaximumDiscountPercent: number;
  requiresApproval: boolean;
  standardDiscountRuleID: number | null;
  currencyCode: string;
}
