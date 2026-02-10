export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface InformativeResult{
  message?:string,
  error?:string
}