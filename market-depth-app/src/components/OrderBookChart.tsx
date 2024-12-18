import React from "react";
import { DepthDataPoint, RawData } from "../types";
import { trimData, calculateOrderCost } from "../utils";
import DepthChart from "./DepthChart";
import OrderControls from "./OrderControls";

interface OrderBookChartProps {
  depthData: DepthDataPoint[];
  rawData: RawData | undefined;
}

const OrderBookChart: React.FC<OrderBookChartProps> = ({
  depthData,
  rawData,
}) => {
  const [chartDepth, setChartDepth] = React.useState<number>(0.2);

  const trimmed = trimData(depthData, chartDepth);

  const [buyBtc, setBuyBtc] = React.useState<number>(0);
  const [sellBtc, setSellBtc] = React.useState<number>(0);

  const buyTotalCost = React.useMemo(
    () => calculateOrderCost(buyBtc, rawData?.asks, true),
    [buyBtc, rawData?.asks]
  );
  const sellTotalCost = React.useMemo(
    () => calculateOrderCost(sellBtc, rawData?.bids, false),
    [sellBtc, rawData?.bids]
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
        acquireTime={rawData?.acquiredAt}
        liquidityTime={rawData?.timestamp}
      />
    </div>
  );
};

export default OrderBookChart;
