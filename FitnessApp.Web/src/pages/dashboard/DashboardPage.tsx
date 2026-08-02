import { useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

export default function DashboardPage() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <div className="page-shell">
      <div className="card dashboard-card">
        <div className="dashboard-header">
          <div>
            <p className="eyebrow">FitnessApp</p>
            <h1 className="title">Dobro došao, {user?.userName ?? "korisniče"}!</h1>
            <p className="subtitle">
              Ovdje će kasnije biti pregled aktivnosti, treninga, prehrane i napretka.
            </p>
          </div>

          <button className="button button-secondary" onClick={handleLogout}>
            Odjava
          </button>
        </div>

        <div className="dashboard-grid">
          <div className="info-card">
            <h3>Profil</h3>
            <p>{user?.email}</p>
          </div>

          <div className="info-card">
            <h3>Treninzi</h3>
            <p>Uskoro lista planova i sesija.</p>
          </div>

          <div className="info-card">
            <h3>Prehrana</h3>
            <p>Uskoro planovi prehrane i kalorije.</p>
          </div>

          <div className="info-card">
            <h3>Statistika</h3>
            <p>Uskoro grafovi napretka i PR-ovi.</p>
          </div>
        </div>
      </div>
    </div>
  );
}