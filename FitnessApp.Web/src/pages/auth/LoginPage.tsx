import type { FormEvent } from "react";
import { useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import type { LoginDto } from "../../types/auth";

export default function LoginPage() {
  const navigate = useNavigate();
  const { login, isAuthenticated } = useAuth();

  const [form, setForm] = useState<LoginDto>({
    userNameOrEmail: "",
    password: "",
  });

  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;

    setForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setLoading(true);
    setError(null);

    try {
      await login(form);
      navigate("/dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Greška pri prijavi.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page-shell">
      <div className="card auth-card">
        <h1 className="title">Prijava</h1>
        <p className="subtitle">Prijavi se u svoj FitnessApp račun.</p>

        {error && <div className="error-box">{error}</div>}

        <form className="form" onSubmit={handleSubmit}>
          <div className="field">
            <label className="label" htmlFor="userNameOrEmail">
              Korisničko ime ili email
            </label>
            <input
              id="userNameOrEmail"
              name="userNameOrEmail"
              className="input"
              value={form.userNameOrEmail}
              onChange={handleChange}
              autoComplete="username"
              required
            />
          </div>

          <div className="field">
            <label className="label" htmlFor="password">
              Lozinka
            </label>
            <input
              id="password"
              name="password"
              type="password"
              className="input"
              value={form.password}
              onChange={handleChange}
              autoComplete="current-password"
              required
            />
          </div>

          <button className="button button-primary" type="submit" disabled={loading}>
            {loading ? "Prijava..." : "Prijavi se"}
          </button>
        </form>

        <p className="helper-text">
          Nemaš račun? <Link to="/register">Registriraj se</Link>
        </p>
      </div>
    </div>
  );
}