import React from "react";
import { TooltipProps } from "recharts";

export const CustomTooltip: React.FC<TooltipProps<number, string>> = ({
  active,
  payload,
  label,
}) => {
  if (!active || !payload || payload.length === 0) return null;
  return (
    <div
      style={{
        background: "#333",
        padding: "5px",
        borderRadius: "4px",
        color: "#fff",
      }}
    >
      <p>Price: {label}</p>
      {payload.map((item, idx) => (
        <p key={idx} style={{ color: item.color }}>
          {item.name === "bidsDepth" ? "Bids Depth" : "Asks Depth"}:{" "}
          {item.value?.toFixed(4)}
        </p>
      ))}
    </div>
  );
};
