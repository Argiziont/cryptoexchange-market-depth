import React from "react";
import * as signalR from "@microsoft/signalr";
import OrderBookChart from "./components/OrderBookChart";
import SnapshotList from "./components/SnapshotList";
import {
  ComputedMarketDepthResult,
  DepthDataPoint,
  RawData,
  Snapshot,
} from "./types";

const App: React.FC = () => {
  const [depthData, setDepthData] = React.useState<DepthDataPoint[]>([]);
  const [rawData, setRawData] = React.useState<RawData>();
  const [lastSnapshots, setLastSnapshots] = React.useState<Snapshot[]>([]);

  React.useEffect(() => {
    // Connect to the SignalR hub
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7275/marketdepthhub")
      .withAutomaticReconnect()
      .build();

    connection
      .start()
      .then(() => {
        // Receive real-time updates
        connection.on("UpdateDepthData", (data: ComputedMarketDepthResult) => {
          setDepthData(data.data);
        });

        connection.on("UpdateRawData", (data: RawData) => {
          setRawData(data);
        });

        // Receive last 10 snapshots
        connection.on("ReceiveLastSnapshots", (snapshots: Snapshot[]) => {
          const sortedSnapshots = snapshots.sort(
            (a, b) =>
              new Date(b.acquiredAt).getTime() -
              new Date(a.acquiredAt).getTime()
          );
          setLastSnapshots(sortedSnapshots);
        });
      })
      .catch((err) => console.error("Connection failed: ", err));

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        padding: "20px",
        fontFamily: "Inter, system-ui, Avenir, Helvetica, Arial, sans-serif",
        color: "rgba(255, 255, 255, 0.87)",
      }}
    >
      <h1 style={{ marginBottom: "20px" }}>Market Depth Chart</h1>
      <div
        style={{
          display: "flex",
          flexDirection: "row",
          width: "2000px",
          height: "890px",
          border: "1px solid #ddd",
          borderRadius: "8px",
          overflow: "hidden",
          boxShadow: "0 2px 8px rgba(0, 0, 0, 0.1)",
          backgroundColor: "transparent",
        }}
      >
        {/* Chart Section */}
        <div
          style={{
            flex: 1,
            padding: "20px",
            overflow: "auto",
          }}
        >
          <OrderBookChart rawData={rawData} depthData={depthData} />
        </div>

        {/* Snapshots Section */}
        <div
          style={{
            width: "350px",
            borderLeft: "1px solid #ddd",
            padding: "20px",
            overflowY: "auto",
            backgroundColor: "transparent",
          }}
        >
          <SnapshotList snapshots={lastSnapshots} />
        </div>
      </div>
    </div>
  );
};

export default App;
