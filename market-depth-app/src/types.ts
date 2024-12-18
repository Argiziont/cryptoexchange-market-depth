export interface RawApiOrder {
  price: number;
  amount: number;
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
export interface RawData {
  acquiredAt: Date;
  timestamp: Date;

  bids: RawApiOrder[] | undefined;
  asks: RawApiOrder[] | undefined;
}

export interface DepthChartProps {
  data: DepthDataPoint[];
  buyReferencePrice?: number;
  sellReferencePrice?: number;
}

export interface ComputedMarketDepthResult {
  data: DepthDataPoint[];
}
