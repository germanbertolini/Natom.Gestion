export class ApiResult<TData> {
    public success: boolean;
    public message: string;
    public data: TData;
}