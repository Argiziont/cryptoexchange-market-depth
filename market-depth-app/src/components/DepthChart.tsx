import React from "react";
import {
  AreaChart,
  Area,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  ReferenceLine,
} from "recharts";
import { DepthChartProps, DepthDataPoint } from "../types";
import { CustomTooltip } from "./CustomTooltip";

const DepthChart: React.FC<DepthChartProps> = ({
  data,
  buyReferencePrice,
  sellReferencePrice,
}) => {
  return (
    <ResponsiveContainer width="100%" height={600}>
      <AreaChart data={data as DepthDataPoint[]}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis
          dataKey="price"
          type="number"
          domain={["dataMin", "dataMax"]}
          tickFormatter={(val: number) => val.toFixed(2)}
        />
        <YAxis tickFormatter={(val: number) => val.toFixed(2)} />
        <Tooltip content={<CustomTooltip />} />
        <Area
          type="step"
          dataKey="bidsDepth"
          stroke="#00C087"
          fill="#00C087"
          fillOpacity={0.2}
          activeDot={{ r: 4 }}
        />
        <Area
          type="step"
          dataKey="asksDepth"
          stroke="#F84960"
          fill="#F84960"
          fillOpacity={0.2}
          activeDot={{ r: 4 }}
        />
        {buyReferencePrice !== undefined && (
          <ReferenceLine
            x={buyReferencePrice}
            stroke="green"
            strokeWidth={1.5}
            strokeDasharray="3 3"
            label={{
              value: `Buy @ ${buyReferencePrice.toFixed(4)} EUR`,
              position: "insideLeft",
              style: {
                fontWeight: "bold",
                fill: "white",
              },
            }}
          />
        )}
        {sellReferencePrice !== undefined && (
          <ReferenceLine
            x={sellReferencePrice}
            stroke="red"
            strokeWidth={1.5}
            strokeDasharray="3 3"
            label={{
              value: `Sell @ ${sellReferencePrice.toFixed(4)} EUR`,
              position: "insideRight",
              style: {
                fontWeight: "bold",
                fill: "white",
              },
            }}
          />
        )}
      </AreaChart>
    </ResponsiveContainer>
  );
};

export default DepthChart;
