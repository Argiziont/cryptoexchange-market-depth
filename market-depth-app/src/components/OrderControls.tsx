import React, { FC } from "react";

interface OrderControlsProps {
  buyBtc: number;
  sellBtc: number;
  onBuyChange: (value: number) => void;
  onSellChange: (value: number) => void;
  buyTotalCost: number | null;
  sellTotalCost: number | null;
  chartDepth: number | null;
  onSetChartDepth: (value: number) => void;
  acquireTime: Date | undefined;
  liquidityTime: Date | undefined;
}

const OrderControls: FC<OrderControlsProps> = ({
  buyBtc,
  sellBtc,
  onBuyChange,
  onSellChange,
  buyTotalCost,
  sellTotalCost,
  chartDepth,
  onSetChartDepth,
  acquireTime,
  liquidityTime,
}) => {
  const buyAverage =
    buyTotalCost !== null && buyBtc > 0 ? buyTotalCost / buyBtc : null;
  const sellAverage =
    sellTotalCost !== null && sellBtc > 0 ? sellTotalCost / sellBtc : null;

  const [localSliderValue, setLocalSliderValue] = React.useState(chartDepth);

  return (
    <div style={{ marginTop: "20px", marginLeft: "60px" }}>
      <div style={{ marginTop: "20px" }}>
        <div style={{ marginTop: "20px" }}>
          <label style={{ display: "flex", alignItems: "center" }}>
            Zoom:
            <input
              type="range"
              step="0.0001"
              min="0.0005"
              max="0.995"
              value={String(localSliderValue)}
              onChange={(e) => setLocalSliderValue(parseFloat(e.target.value))}
              onMouseUp={() => onSetChartDepth(localSliderValue ?? 0.0001)}
              style={{ marginLeft: "10px", width: "400px" }}
            />
          </label>
        </div>
      </div>

      <div style={{ marginTop: "20px" }}>
        <label>
          Buy BTC:
          <input
            type="number"
            step="0.0001"
            value={String(buyBtc)}
            onChange={(e) => onBuyChange(parseFloat(e.target.value))}
            style={{ marginLeft: "10px", width: "100px" }}
          />
        </label>
        {buyTotalCost !== null && buyBtc > 0 ? (
          <div style={{ marginTop: "10px" }}>
            <p>Total Cost (Buy): {buyTotalCost.toFixed(4)} EUR</p>
            <p>Avg Price (Buy): {buyAverage?.toFixed(4)} EUR</p>
          </div>
        ) : buyBtc > 0 ? (
          <p style={{ color: "red", marginTop: "10px" }}>
            Not enough liquidity to fulfill buy order.
          </p>
        ) : null}
      </div>

      <div style={{ marginTop: "20px" }}>
        <label>
          Sell BTC:
          <input
            type="number"
            step="0.0001"
            value={String(sellBtc)}
            onChange={(e) => onSellChange(parseFloat(e.target.value))}
            style={{ marginLeft: "10px", width: "100px" }}
          />
        </label>
        {sellTotalCost !== null && sellBtc > 0 ? (
          <div style={{ marginTop: "10px" }}>
            <p>Total Proceeds (Sell): {sellTotalCost.toFixed(4)} EUR</p>
            <p>Avg Price (Sell): {sellAverage?.toFixed(4)} EUR</p>
          </div>
        ) : sellBtc > 0 ? (
          <p style={{ color: "red", marginTop: "10px" }}>
            Not enough liquidity to fulfill sell order.
          </p>
        ) : null}
        <div style={{ marginTop: "20px" }}>
          {acquireTime && <p>Acquire Time: {acquireTime.toLocaleString()}</p>}
          {liquidityTime && (
            <p>Liquidity Time: {liquidityTime.toLocaleString()}</p>
          )}
        </div>
      </div>
    </div>
  );
};

export default OrderControls;
