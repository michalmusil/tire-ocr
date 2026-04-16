import React from "react";
import { Outlet } from "react-router-dom";
import { SidebarProvider, SidebarTrigger } from "../components/ui/sidebar";
import AppSidebar from "../components/app-sidebar";
import { Separator } from "../components/ui/separator";

const SidebarLayout: React.FC = () => {
  return (
    <SidebarProvider>
      <AppSidebar />
      <main className="flex flex-col flex-1 overflow-x-hidden overflow-y-auto mx-6">
        <header className="flex items-center sticky top-0 m-5">
          <SidebarTrigger className="w-10 h-10" />
          <Separator
            orientation="vertical"
            className="mr-3 ml-1 data-[orientation=vertical]:h-5"
          />
          <h1 className="text-xl font-semibold">Tire OCR Evaluation Tool</h1>
        </header>

        <Outlet />
      </main>
    </SidebarProvider>
  );
};

export default SidebarLayout;
