import { NavLink, Outlet, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

const navigationItems = [
  { to: "/dashboard", label: "Dashboard" },
  { to: "/profile", label: "Profil" },
  { to: "/exercises", label: "Vježbe" },
  { to: "/workout-plans", label: "Planovi treninga" },
  { to: "/workout-sessions", label: "Treninzi" },
  { to: "/meal-plans", label: "Prehrana" },
  { to: "/body-measurements", label: "Mjerenja" },
  { to: "/calorie-entries", label: "Kalorije" },
  { to: "/personal-records", label: "PR" },
  { to: "/statistics", label: "Statistika" },
];

export default function AppLayout() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="sidebar-brand">
          <div className="brand-mark">FA</div>
          <div>
            <h1>FitnessApp</h1>
            <p>Control panel</p>
          </div>
        </div>

        <nav className="sidebar-nav">
          {navigationItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              className={({ isActive }) => `nav-link ${isActive ? "active" : ""}`}
            >
              {item.label}
            </NavLink>
          ))}
        </nav>

        <div className="sidebar-footer">
          <div className="sidebar-user">
            <span>{user?.userName ?? "Korisnik"}</span>
            <small>{user?.roles?.join(", ") ?? "User"}</small>
          </div>

          <button className="button button-secondary sidebar-logout" onClick={handleLogout}>
            Odjava
          </button>
        </div>
      </aside>

      <div className="app-main">
        <header className="topbar">
          <div>
            <p className="eyebrow">Trenutni pregled</p>
            <h2>{user?.userName ?? "Korisnik"}</h2>
          </div>

          <div className="topbar-actions">
            <div className="user-chip">
              <span>{user?.email ?? ""}</span>
              <small>{user?.roles?.join(", ") ?? "User"}</small>
            </div>

            <button className="button button-secondary" onClick={handleLogout}>
              Odjava
            </button>
          </div>
        </header>

        <main className="content-area">
          <Outlet />
        </main>
      </div>
    </div>
  );
}