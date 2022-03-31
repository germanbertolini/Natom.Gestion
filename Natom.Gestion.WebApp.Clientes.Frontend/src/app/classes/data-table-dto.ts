export class DataTableDTO<TData> {
  recordsTotal: number;
  recordsFiltered: number;
  records: TData[];
  extraData: any;
}
