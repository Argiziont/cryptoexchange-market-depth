export type RawApiOrder = [string, string];

export interface ApiOrder {
  price: number;
  quantity: number;
}

export interface OrderBookData {
  timestamp: string;
  microtimestamp: string;
  bids: RawApiOrder[];
  asks: RawApiOrder[];
}

export interface DepthDataPoint {
  price: number;
  bidsDepth?: number;
  asksDepth?: number;
}

export interface DepthChartProps {
  data: DepthDataPoint[];
  buyReferencePrice?: number;
  sellReferencePrice?: number;
}
