export interface Pagination<T> {
    data: T[];
    page: number;
    availablePages: number;
    pageSize: number;
}