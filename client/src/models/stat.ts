export interface Stat {
  name: string;
  apiKey: string;
  rateLimit: number;
  requests: number;
  blocked: number;
}
