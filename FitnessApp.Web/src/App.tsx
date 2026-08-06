import { Navigate, Route, Routes } from "react-router-dom";
import { useAuth } from "./context/AuthContext";
import ProtectedRoute from "./routes/ProtectedRoute";
import AppLayout from "./layouts/AppLayout";
import DashboardPage from "./pages/dashboard/DashboardPage";
import LoginPage from "./pages/auth/LoginPage";
import RegisterPage from "./pages/auth/RegisterPage";
import ComingSoonPage from "./pages/common/ComingSoonPage";

export default function App() {
  const { isAuthenticated } = useAuth();

  return (
    <Routes>
      <Route
        path="/"
        element={<Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />}
      />

      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      <Route element={<ProtectedRoute />}>
        <Route element={<AppLayout />}>
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route
            path="/profile"
            element={
              <ComingSoonPage
                title="Profil"
                description="Ovdje ćemo uređivati korisničke podatke i fitness cilj."
              />
            }
          />
          <Route
            path="/exercises"
            element={
              <ComingSoonPage
                title="Vježbe"
                description="Ovdje dolazi baza vježbi, search, filteri i detalji vježbi."
              />
            }
          />
          <Route
            path="/workout-plans"
            element={
              <ComingSoonPage
                title="Planovi treninga"
                description="Ovdje ćemo kasnije prikazivati i uređivati planove treninga."
              />
            }
          />
          <Route
            path="/workout-sessions"
            element={
              <ComingSoonPage
                title="Treninzi"
                description="Ovdje dolaze zakazani trening termini i evidencija izvedbe."
              />
            }
          />
          <Route
            path="/meal-plans"
            element={
              <ComingSoonPage
                title="Prehrana"
                description="Ovdje ćemo imati planove prehrane i povezane obroke."
              />
            }
          />
          <Route
            path="/body-measurements"
            element={
              <ComingSoonPage
                title="Mjerenja"
                description="Ovdje ćemo pratiti težinu, BMI i ostala tjelesna mjerenja."
              />
            }
          />
          <Route
            path="/calorie-entries"
            element={
              <ComingSoonPage
                title="Kalorije"
                description="Ovdje dolazi dnevni unos kalorija i pregled kroz vrijeme."
              />
            }
          />
          <Route
            path="/personal-records"
            element={
              <ComingSoonPage
                title="Osobni rekordi"
                description="Ovdje ćemo prikazivati PR-ove po vježbama."
              />
            }
          />
          <Route
            path="/statistics"
            element={
              <ComingSoonPage
                title="Statistika"
                description="Ovdje dolaze grafovi napretka, treninzi i sažeci."
              />
            }
          />
        </Route>
      </Route>

      <Route
        path="*"
        element={<Navigate to={isAuthenticated ? "/dashboard" : "/login"} replace />}
      />
    </Routes>
  );
}