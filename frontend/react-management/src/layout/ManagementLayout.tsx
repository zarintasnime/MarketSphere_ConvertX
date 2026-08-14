import { useState } from "react";
import { Outlet } from "react-router-dom";

import Breadcrumbs from "./Breadcrumbs";
import ManagementSidebar from "./ManagementSidebar";
import ManagementTopbar from "./ManagementTopbar";

export default function ManagementLayout() {
  const [sidebarOpen, setSidebarOpen] = useState(false);

  return (
    <div className="management-layout">
      <ManagementSidebar
        open={sidebarOpen}
        onClose={() => setSidebarOpen(false)}
      />

      <div className="management-layout__workspace">
        <ManagementTopbar onMenuClick={() => setSidebarOpen(true)} />
        <div className="management-layout__content">
          <Breadcrumbs />
          <Outlet />
        </div>
      </div>
    </div>
  );
}
