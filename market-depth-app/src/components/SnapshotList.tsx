// src/components/SnapshotList.tsx
import React from "react";
import { Snapshot } from "../types";

interface SnapshotListProps {
  snapshots: Snapshot[];
}

const SnapshotList: React.FC<SnapshotListProps> = ({ snapshots }) => {
  return (
    <div>
      <h2 style={{ marginBottom: "10px" }}>Last 10 Snapshots</h2>
      <ul
        style={{
          listStyleType: "none",
          padding: 0,
          margin: 0,
          maxHeight: "100%",
        }}
      >
        {snapshots.map((snapshot, index) => (
          <li
            key={snapshot.id}
            style={{
              padding: "10px",
              marginBottom: "10px",
              border: index === 0 ? "2px solid #646cff" : "1px solid #ccc",
              borderRadius: "4px",
              fontWeight: index === 0 ? "bold" : "normal",
              backgroundColor: "transparent",
            }}
          >
            <div>
              <strong>Snapshot ID:</strong> {snapshot.id}
            </div>
            <div>
              <strong>Acquired At:</strong>{" "}
              {new Date(snapshot.acquiredAt).toLocaleString()}
            </div>
            <div>
              <strong>Timestamp:</strong>{" "}
              {new Date(snapshot.timestamp).toLocaleString()}
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default SnapshotList;
