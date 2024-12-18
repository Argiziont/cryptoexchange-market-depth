import React from "react";
import * as signalR from "@microsoft/signalr";

import OrderBookChart from "./components/OrderBookChart";
import { ComputedMarketDepthResult, DepthDataPoint, RawData } from "./types";

const App: React.FC = () => {
  const [depthData, setDepthData] = React.useState<DepthDataPoint[]>([]);
  const [rawData, setRawData] = React.useState<RawData>();

  React.useEffect(() => {
    // Connect to the SignalR hub
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7275/marketdepthhub") // Adjust to BE URL, By default - https://localhost:7275
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        console.log("Connected to MarketDepthHub");
        connection.on("UpdateDepthData", (data: ComputedMarketDepthResult) => {
          setDepthData(data.data);
        });
        connection.on("UpdateRawData", (data: RawData) => {
          setRawData(data);
        });
      })
      .catch((err) => console.error("Connection failed: ", err));

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <div style={{ width: "1250px", height: "600px" }}>
      <h1 style={{ marginLeft: "60px" }}>Market Depth Chart</h1>
      <OrderBookChart rawData={rawData} depthData={depthData} />
    </div>
  );
};

export default App;
