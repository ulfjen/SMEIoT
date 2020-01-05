export default interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  traceId?: string;
  errors?: any;
  detail?: string;
};
