import type { FormEvent } from "react";
import { useState } from "react";
import { Link, Navigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import type { RegisterDto } from "../../types/auth";

export default function RegisterPage() {
  const { register, isAuthenticated } = useAuth();

  const [form, setForm] = useState<RegisterDto>({
    firstName: "",
    lastName: "",
    userName: "",
    email: "",
    password: "",
  });

  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
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
    setSuccess(null);

    try {
      await register(form);
      setSuccess("Korisnik je uspješno registriran. Sada se možeš prijaviti.");
      setForm({
        firstName: "",
        lastName: "",
        userName: "",
        email: "",
        password: "",
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Greška pri registraciji.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page-shell">
      <div className="card auth-card">
        <h1 className="title">Registracija</h1>
        <p className="subtitle">Kreiraj svoj FitnessApp račun.</p>

        {error && <div className="error-box">{error}</div>}
        {success && <div className="success-box">{success}</div>}

        <form className="form" onSubmit={handleSubmit}>
          <div className="field-grid">
            <div className="field">
              <label className="label" htmlFor="firstName">
                Ime
              </label>
              <input
                id="firstName"
                name="firstName"
                className="input"
                value={form.firstName}
                onChange={handleChange}
                autoComplete="given-name"
                required
              />
            </div>

            <div className="field">
              <label className="label" htmlFor="lastName">
                Prezime
              </label>
              <input
                id="lastName"
                name="lastName"
                className="input"
                value={form.lastName}
                onChange={handleChange}
                autoComplete="family-name"
                required
              />
            </div>
          </div>

          <div className="field">
            <label className="label" htmlFor="userName">
              Korisničko ime
            </label>
            <input
              id="userName"
              name="userName"
              className="input"
              value={form.userName}
              onChange={handleChange}
              autoComplete="username"
              required
            />
          </div>

          <div className="field">
            <label className="label" htmlFor="email">
              Email
            </label>
            <input
              id="email"
              name="email"
              type="email"
              className="input"
              value={form.email}
              onChange={handleChange}
              autoComplete="email"
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
              autoComplete="new-password"
              required
            />
          </div>

          <button className="button button-primary" type="submit" disabled={loading}>
            {loading ? "Registracija..." : "Registriraj se"}
          </button>
        </form>

        <p className="helper-text">
          Već imaš račun? <Link to="/login">Prijavi se</Link>
        </p>
      </div>
    </div>
  );
}