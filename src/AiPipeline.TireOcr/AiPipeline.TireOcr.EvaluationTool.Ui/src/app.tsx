import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import DefaultLayout from "./core/layouts/sidabar-layout";

const Dashboard = () => <div>Dashboard</div>;
const App: React.FC = () => {
  return (
    <Router>
      <DefaultLayout>
        <Routes>
          <Route path="/" element={<Dashboard />} />
        </Routes>
      </DefaultLayout>
    </Router>
  );
};

export default App;
