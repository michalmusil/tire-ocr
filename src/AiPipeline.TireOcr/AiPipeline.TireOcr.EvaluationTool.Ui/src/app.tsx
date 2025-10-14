import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import DefaultLayout from "./core/layouts/sidabar-layout";
import EvaluationRunsPage from "./runs/pages/evaluation-runs-page";
import EvaluationBatchesPage from "./run-batches/pages/evaluation-batches-page";
import CreateRun from "./runs/pages/create-run-page";
import CreateBatch from "./run-batches/pages/create-batch-page";

const Dashboard = () => <div>Dashboard</div>;
const App: React.FC = () => {
  return (
    <Router>
      <DefaultLayout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/runs" element={<EvaluationRunsPage />} />
          <Route path="/batches" element={<EvaluationBatchesPage />} />
          <Route path="/create-run" element={<CreateRun />} />
          <Route path="/create-batch" element={<CreateBatch />} />
        </Routes>
      </DefaultLayout>
    </Router>
  );
};

export default App;
