export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface InformativeResult {
  message?: string;
  error?: string;
}

export interface Query {
  page: number;
  pageSize: number;
}

export interface SearchQuery extends Query {
  search?: string;
}
