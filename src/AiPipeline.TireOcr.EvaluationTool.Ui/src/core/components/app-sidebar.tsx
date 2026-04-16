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
  SidebarFooter,
} from "./ui/sidebar";
import { Package, Workflow, Play, LogOut, User, Home } from "lucide-react";
import { ModeToggle } from "./mode-toggle";
import { useAuth } from "../providers/auth-provider";
import { Button } from "./ui/button";
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
      <Header />
      <SidebarContent>
        <SidebarGroup>
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarMenuButton asChild>
                <Link to={"/"}>
                  <Home />
                  <span>Home</span>
                </Link>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarGroup>
        <SingleRunGroup />
        <RunBatchGroup />
      </SidebarContent>
      <Footer />
    </Sidebar>
  );
};

const Header = () => {
  return (
    <SidebarHeader className="flex flex-row justify-between items-center">
      <Link to="/">
        <div className="flex items-center">
          <Logo className="block w-12 h-12" />
          <h1 className="text-xl font-semibold">Tire OCR</h1>
        </div>
      </Link>
      <ModeToggle />
    </SidebarHeader>
  );
};

const SingleRunGroup = () => {
  return (
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
  );
};

const RunBatchGroup = () => {
  return (
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
  );
};

const Footer = () => {
  const { authState, logout } = useAuth();

  return (
    <SidebarFooter>
      <SidebarGroup>
        <SidebarGroupContent>
          <div className="flex justify-between items-center gap-2 p-2">
            <div className="flex items-center gap-2 text-sm text-muted-foreground">
              <User className="w-4 h-4" />
              <span>{authState?.user.username ?? "ERROR: Not logged in"}</span>
            </div>
            <Button variant="outline" size="sm" onClick={logout}>
              <LogOut className="w-4 h-4" />
              Logout
            </Button>
          </div>
        </SidebarGroupContent>
      </SidebarGroup>
    </SidebarFooter>
  );
};

export default AppSidebar;
