import React from "react";
import { Link, useLocation } from "react-router-dom";
import { LayoutDashboard, List, FilePlus } from "lucide-react";
import { Button } from "@/core/components/ui/button";
import { ModeToggle } from "./mode-toggle";

const AppSidebar: React.FC = () => {
  const location = useLocation();

  const menuItems = [
    { to: "/", icon: LayoutDashboard, label: "Dashboard" },
    { to: "/runs", icon: List, label: "Evaluation Runs" },
    { to: "/batches", icon: List, label: "Evaluation Batches" },
    { to: "/create-run", icon: FilePlus, label: "New Run" },
    { to: "/create-batch", icon: FilePlus, label: "New Batch" },
  ];

  return (
    <div className="m-6">
      <div className="flex flex-row justify-around w-full overflow-clip p-3 bg-accent/25 rounded-md md:flex-col md:justify-start md:w-56 lg:w-64 md:h-full">
        <div className="p-4 flex space-x-5">
          <ModeToggle />
          <h2 className="text-2xl font-semibold">Tire OCR</h2>
        </div>
        <nav className="px-2 flex flex-col md:mt-4">
          {menuItems.map((item) => (
            <Button
              key={item.to}
              asChild
              variant={location.pathname === item.to ? "secondary" : "ghost"}
              className="md:w-full justify-start mb-1 overflow-clip"
            >
              <Link to={item.to}>
                <item.icon className="mr-2 h-4 w-4" />
                {item.label}
              </Link>
            </Button>
          ))}
        </nav>
      </div>
    </div>
  );
};

export default AppSidebar;
