import { useAuth } from "../../context/AuthContext";

export default function DashboardPage() {
  const { user } = useAuth();

  return (
    <div className="dashboard-page">
      <section className="hero-card">
        <div>
          <p className="eyebrow">FitnessApp</p>
          <h1 className="title">Dobro došao, {user?.userName ?? "korisniče"}!</h1>
          <p className="subtitle">
            Ovo je početni pregled aplikacije. Kasnije ćemo ovdje prikazivati stvarne
            podatke iz profila, treninga, prehrane i napretka.
          </p>
        </div>

        <div className="hero-badge">
          <span>Uloga</span>
          <strong>{user?.roles?.join(", ") ?? "User"}</strong>
        </div>
      </section>

      <section className="dashboard-grid">
        <div className="metric-card">
          <span>Korisinik</span>
          <strong>{user?.userName ?? "-"}</strong>
        </div>

        <div className="metric-card">
          <span>Email</span>
          <strong>{user?.email ?? "-"}</strong>
        </div>

        <div className="metric-card">
          <span>Fitness cilj</span>
          <strong>Uskoro</strong>
        </div>

        <div className="metric-card">
          <span>Zadnje mjerenje</span>
          <strong>Uskoro</strong>
        </div>

        <div className="metric-card">
          <span>Zadnji trening</span>
          <strong>Uskoro</strong>
        </div>

        <div className="metric-card">
          <span>Zadnji PR</span>
          <strong>Uskoro</strong>
        </div>
      </section>
    </div>
  );
}