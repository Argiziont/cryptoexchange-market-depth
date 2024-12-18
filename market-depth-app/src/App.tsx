import React from "react";
import OrderBookChart from "./components/OrderBookChart";
import { OrderBookData } from "./types";

import testData from "./testData.json";

const App: React.FC = () => {
  return (
    <div style={{ width: "1250px", height: "600px" }}>
      <h1 style={{ marginLeft: "60px" }}>Market Depth Chart</h1>
      <OrderBookChart data={testData as OrderBookData} />
    </div>
  );
};

export default App;
