import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import DefaultLayout from "./core/layouts/sidabar-layout";
import EvaluationRuns from "./runs/pages/evaluation-runs";
import CreateRun from "./runs/pages/create-run";

const Dashboard = () => <div>Dashboard</div>;
const App: React.FC = () => {
  return (
    <Router>
      <DefaultLayout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/runs" element={<EvaluationRuns />} />
          <Route path="/create-run" element={<CreateRun />} />
        </Routes>
      </DefaultLayout>
    </Router>
  );
};

export default App;
