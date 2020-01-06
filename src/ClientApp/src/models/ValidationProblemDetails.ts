import { ProblemDetails } from "smeiot-client";

export default interface ValidationProblemDetails extends ProblemDetails {
  readonly errors?: {
    [key: string]: Array<string>;
  };
} 
