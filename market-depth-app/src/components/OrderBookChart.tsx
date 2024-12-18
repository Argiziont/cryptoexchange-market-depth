import React from "react";
import { OrderBookData } from "../types";
import {
  normalizeOrders,
  computeDepthData,
  trimData,
  calculateOrderCost,
} from "../utils";
import DepthChart from "./DepthChart";
import OrderControls from "./OrderControls";

interface OrderBookChartProps {
  data: OrderBookData;
}

const OrderBookChart: React.FC<OrderBookChartProps> = ({ data }) => {
  const { bids, asks } = data;
  const normalizedBids = normalizeOrders(bids);
  const normalizedAsks = normalizeOrders(asks);

  const combinedData = computeDepthData(normalizedBids, normalizedAsks);
  const bidsDepth = combinedData.filter((d) => d.bidsDepth !== undefined);
  const asksDepth = combinedData.filter((d) => d.asksDepth !== undefined);

  const [chartDepth, setChartDepth] = React.useState<number>(0.2);

  const trimmed = trimData(combinedData, bidsDepth, asksDepth, chartDepth);

  const [buyBtc, setBuyBtc] = React.useState<number>(0);
  const [sellBtc, setSellBtc] = React.useState<number>(0);

  const buyTotalCost = React.useMemo(
    () => calculateOrderCost(buyBtc, normalizedAsks, true),
    [buyBtc, normalizedAsks]
  );
  const sellTotalCost = React.useMemo(
    () => calculateOrderCost(sellBtc, normalizedBids, false),
    [sellBtc, normalizedBids]
  );

  const buyReferencePrice =
    buyTotalCost && buyBtc > 0 ? buyTotalCost / buyBtc : undefined;
  const sellReferencePrice =
    sellTotalCost && sellBtc > 0 ? sellTotalCost / sellBtc : undefined;

  if (trimmed.length === 0) {
    return <div>No data available</div>;
  }

  return (
    <div>
      <DepthChart
        data={trimmed}
        buyReferencePrice={buyReferencePrice}
        sellReferencePrice={sellReferencePrice}
      />
      <OrderControls
        buyBtc={buyBtc}
        sellBtc={sellBtc}
        onBuyChange={setBuyBtc}
        onSellChange={setSellBtc}
        buyTotalCost={buyTotalCost}
        sellTotalCost={sellTotalCost}
        chartDepth={chartDepth}
        onSetChartDepth={setChartDepth}
      />
    </div>
  );
};

export default OrderBookChart;
