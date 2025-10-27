/// <reference types="vite-plugin-svgr/client" />

import { Link } from "react-router-dom";
import {
  Sidebar,
  SidebarContent,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton,
  SidebarHeader,
} from "./ui/sidebar";
import { Package, Workflow, Play } from "lucide-react";
import { ModeToggle } from "./mode-toggle";
import Logo from "../assets/tire-icon.svg?react";

const singleRunItems = [
  { to: "/runs", icon: Workflow, label: "Results" },
  { to: "/create-run", icon: Play, label: "New Run" },
];

const runBatchItems = [
  { to: "/batches", icon: Package, label: "Batch results" },
  { to: "/create-batch", icon: Play, label: "New Batch" },
];

const AppSidebar = () => {
  return (
    <Sidebar>
      <SidebarHeader className="flex flex-row justify-between items-center">
        <Link to="/">
          <div className="flex items-center">
            <Logo className="block w-12 h-12" />
            <h1 className="text-xl font-semibold">Tire OCR</h1>
          </div>
        </Link>
        <ModeToggle />
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Single run</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {singleRunItems.map((item) => (
                <SidebarMenuItem key={item.to}>
                  <SidebarMenuButton asChild>
                    <Link to={item.to}>
                      <item.icon />
                      <span>{item.label}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
        <SidebarGroup>
          <SidebarGroupLabel>Run batch</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {runBatchItems.map((item) => (
                <SidebarMenuItem key={item.to}>
                  <SidebarMenuButton asChild>
                    <Link to={item.to}>
                      <item.icon />
                      <span>{item.label}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>
    </Sidebar>
  );
};

export default AppSidebar;
