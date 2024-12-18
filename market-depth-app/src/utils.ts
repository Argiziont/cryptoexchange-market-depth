import { RawApiOrder, DepthDataPoint } from "./types";

export function trimData(
  data: DepthDataPoint[],
  rangePercent: number
): DepthDataPoint[] {
  if (data.length === 0) {
    console.warn("trimData: Received empty data array.");
    return data;
  }

  const bids = data.filter((d) => d.bidsDepth !== undefined && d.bidsDepth);
  const asks = data.filter((d) => d.asksDepth !== undefined && d.asksDepth);

  if (bids.length === 0 || asks.length === 0) {
    console.warn("trimData: No valid bids or asks. Returning data as is.");
    return data;
  }

  const bestBid = Math.max(...bids.map((b) => b.price));
  const bestAsk = Math.min(...asks.map((a) => a.price));

  const midPrice = (bestBid + bestAsk) / 2;
  const lowerBound = midPrice * (1 - rangePercent);
  const upperBound = midPrice * (1 + rangePercent);

  const trimmed = data.filter(
    (point) => point.price >= lowerBound && point.price <= upperBound
  );

  return trimmed;
}

/**
 * Calculate the cost of buying or selling a certain amount of BTC given the order book.
 * For buying: use asks (lowest ask first).
 * For selling: use bids (highest bid first).
 */
export function calculateOrderCost(
  amount: number | undefined,
  orders: RawApiOrder[] | undefined,
  isBuy: boolean
): number | null {
  if (!amount || isNaN(amount) || amount <= 0 || !orders) return null;

  let remaining = amount;
  let total = 0;

  // For buy: asks are sorted low->high
  // For sell: bids are sorted high->low
  const sorted = [...orders];
  if (isBuy) {
    sorted.sort((a, b) => a.price - b.price);
  } else {
    sorted.sort((a, b) => b.price - a.price);
  }

  for (const order of sorted) {
    if (remaining <= 0) break;
    const filled = Math.min(order.amount, remaining);
    total += filled * order.price;
    remaining -= filled;
  }

  return remaining > 0 ? null : total;
}
