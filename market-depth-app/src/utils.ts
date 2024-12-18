import { ApiOrder, DepthDataPoint } from "./types";

export function normalizeOrders(rawOrders: [string, string][]): ApiOrder[] {
  return rawOrders.map(([price, quantity]) => ({
    price: Number(price),
    quantity: Number(quantity),
  }));
}

export function computeDepthData(
  bids: ApiOrder[],
  asks: ApiOrder[]
): DepthDataPoint[] {
  // Sort bids descending (high to low)
  bids.sort((a, b) => b.price - a.price);
  // Sort asks ascending (low to high)
  asks.sort((a, b) => a.price - b.price);

  const bidsDepth = bids.reduce<DepthDataPoint[]>((acc, item) => {
    const cumulative = (acc[acc.length - 1]?.bidsDepth ?? 0) + item.quantity;
    acc.push({ price: item.price, bidsDepth: cumulative });
    return acc;
  }, []);

  const asksDepth = asks.reduce<DepthDataPoint[]>((acc, item) => {
    const cumulative = (acc[acc.length - 1]?.asksDepth ?? 0) + item.quantity;
    acc.push({ price: item.price, asksDepth: cumulative });
    return acc;
  }, []);

  const priceSet = new Set<number>([
    ...bidsDepth.map((b) => b.price),
    ...asksDepth.map((a) => a.price),
  ]);

  const combinedData = Array.from(priceSet)
    .map((price) => {
      const bidPoint = bidsDepth.find((b) => b.price === price);
      const askPoint = asksDepth.find((a) => a.price === price);
      return {
        price,
        bidsDepth: bidPoint?.bidsDepth,
        asksDepth: askPoint?.asksDepth,
      };
    })
    .sort((a, b) => a.price - b.price);
  return combinedData;
}

export function trimData(
  data: DepthDataPoint[],
  bidsDepth: DepthDataPoint[],
  asksDepth: DepthDataPoint[],
  trimPercentage: number
): DepthDataPoint[] {
  const bestBid =
    bidsDepth.length > 0 ? Math.max(...bidsDepth.map((b) => b.price)) : 0;
  const bestAsk =
    asksDepth.length > 0 ? Math.min(...asksDepth.map((a) => a.price)) : 0;

  if (bestBid === 0 || bestAsk === 0) {
    return [];
  }

  const midPrice = (bestBid + bestAsk) / 2;
  const lowerBound = midPrice * (1 - trimPercentage);
  const upperBound = midPrice * (1 + trimPercentage);

  return data.filter(
    (point) => point.price >= lowerBound && point.price <= upperBound
  );
}

/**
 * Calculate the cost of buying or selling a certain amount of BTC given the order book.
 * For buying: use asks (lowest ask first).
 * For selling: use bids (highest bid first).
 */
export function calculateOrderCost(
  amount: number,
  orders: ApiOrder[],
  isBuy: boolean
): number | null {
  if (isNaN(amount) || amount <= 0) return null;

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
    const filled = Math.min(order.quantity, remaining);
    total += filled * order.price;
    remaining -= filled;
  }

  return remaining > 0 ? null : total;
}
