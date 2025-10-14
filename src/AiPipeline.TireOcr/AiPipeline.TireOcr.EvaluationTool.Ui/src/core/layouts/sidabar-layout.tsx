import React from "react";
import AppSidebar from "@/core/components/app-sidebar";
import { ThemeProvider } from "../components/theme-provider";

const DefaultLayout: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  return (
    <ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
      <div className="flex flex-col md:flex-row h-screen">
        <AppSidebar />
        <div className="flex-1 flex flex-col overflow-hidden">
          <main className="flex-1 overflow-x-hidden overflow-y-auto">
            <div className="container mx-auto px-6 py-8">{children}</div>
          </main>
        </div>
      </div>
    </ThemeProvider>
  );
};

export default DefaultLayout;
