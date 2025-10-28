import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import AuthProvider from "./core/providers/auth-provider";
import { AuthenticatedRoute } from "./core/components/authenticated-route";
import SidebarLayout from "./core/layouts/sidabar-layout";
import LoginPage from "./core/pages/login-page";
import DashboardPage from "./core/pages/dashboard-page";
import EvaluationRunsPage from "./runs/pages/evaluation-runs-page";
import EvaluationBatchesPage from "./run-batches/pages/evaluation-batches-page";
import EvaluationBatchDetailPage from "./run-batches/pages/evaluation-batch-detail-page";
import CreateRun from "./runs/pages/create-run-page";
import CreateBatch from "./run-batches/pages/create-batch-page";
import EvaluationRunDetailPage from "./runs/pages/evaluation-run-detail-page";
import { ThemeProvider } from "./core/components/theme-provider";

const App: React.FC = () => {
  return (
    <AuthProvider>
      <ThemeProvider defaultTheme="system" storageKey="vite-ui-theme">
        <Router>
          <Routes>
            <Route path="/login" element={<LoginPage />} />

            <Route element={<AuthenticatedRoute />}>
              <Route element={<SidebarLayout />}>
                <Route path="/" element={<DashboardPage />} />
                <Route path="/runs" element={<EvaluationRunsPage />} />
                <Route
                  path="/runs/:runId"
                  element={<EvaluationRunDetailPage />}
                />
                <Route path="/batches" element={<EvaluationBatchesPage />} />
                <Route
                  path="/batches/:batchId"
                  element={<EvaluationBatchDetailPage />}
                />
                <Route path="/create-run" element={<CreateRun />} />
                <Route path="/create-batch" element={<CreateBatch />} />
              </Route>
            </Route>
          </Routes>
        </Router>
      </ThemeProvider>
    </AuthProvider>
  );
};

export default App;
