import { useEffect, useState } from "react";
import type { ChangeEvent, FormEvent } from "react";
import { lookupService } from "../../services/lookupService";
import { profileService } from "../../services/profileService";
import type { LookupDto } from "../../types/lookups";
import type { UserProfileReadDto, UserProfileUpdateDto } from "../../types/profile";

interface ProfileFormState {
  fitnessGoalId: string;
  dateOfBirth: string;
  gender: string;
  heightCm: string;
  currentWeightKg: string;
}

function toDateInputValue(dateValue: string | null): string {
  if (!dateValue) {
    return "";
  }

  return dateValue.split("T")[0];
}

function formatNumber(value: number | null | undefined): string {
  return value === null || value === undefined ? "-" : value.toString();
}

export default function ProfilePage() {
  const [profile, setProfile] = useState<UserProfileReadDto | null>(null);
  const [fitnessGoals, setFitnessGoals] = useState<LookupDto[]>([]);
  const [form, setForm] = useState<ProfileFormState>({
    fitnessGoalId: "",
    dateOfBirth: "",
    gender: "",
    heightCm: "",
    currentWeightKg: "",
  });

  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  useEffect(() => {
    const load = async () => {
      try {
        setLoading(true);

        const [profileData, goalsData] = await Promise.all([
          profileService.getMyProfile(),
          lookupService.getFitnessGoals(),
        ]);

        setProfile(profileData);
        setFitnessGoals(goalsData);

        setForm({
          fitnessGoalId: profileData.fitnessGoalId ? String(profileData.fitnessGoalId) : "",
          dateOfBirth: toDateInputValue(profileData.dateOfBirth),
          gender: profileData.gender ?? "",
          heightCm: profileData.heightCm !== null ? String(profileData.heightCm) : "",
          currentWeightKg:
            profileData.currentWeightKg !== null ? String(profileData.currentWeightKg) : "",
        });
      } catch (err) {
        setError(err instanceof Error ? err.message : "Greška pri dohvaćanju profila.");
      } finally {
        setLoading(false);
      }
    };

    void load();
  }, []);

  const handleChange = (
    event: ChangeEvent<HTMLInputElement | HTMLSelectElement>
  ) => {
    const { name, value } = event.target;

    setForm((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSaving(true);
    setError(null);
    setSuccess(null);

    try {
      const dto: UserProfileUpdateDto = {
        fitnessGoalId: form.fitnessGoalId ? Number(form.fitnessGoalId) : null,
        dateOfBirth: form.dateOfBirth || null,
        gender: form.gender || null,
        heightCm: form.heightCm ? Number(form.heightCm) : null,
        currentWeightKg: form.currentWeightKg ? Number(form.currentWeightKg) : null,
      };

      await profileService.updateMyProfile(dto);
      setSuccess("Profil je uspješno ažuriran.");
      const refreshed = await profileService.getMyProfile();
      setProfile(refreshed);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Greška pri spremanju profila.");
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <div className="page-card">
        <p className="subtitle">Učitavanje profila...</p>
      </div>
    );
  }

  return (
    <div className="profile-page">
      <section className="hero-card">
        <div>
          <p className="eyebrow">Korisnički profil</p>
          <h1 className="title">Moj profil</h1>
          <p className="subtitle">
            Ovdje uređuješ osnovne podatke, fitness cilj i tjelesne vrijednosti.
          </p>
        </div>

        <div className="hero-badge">
          <span>Trenutni cilj</span>
          <strong>{profile?.fitnessGoalName ?? "Nije odabrano"}</strong>
        </div>
      </section>

      {error && <div className="error-box">{error}</div>}
      {success && <div className="success-box">{success}</div>}

      <div className="profile-grid">
        <section className="page-card">
          <h2 className="section-title">Osnovni podaci</h2>

          <div className="readonly-grid">
            <div className="readonly-item">
              <span>Ime</span>
              <strong>{profile?.firstName ?? "-"}</strong>
            </div>

            <div className="readonly-item">
              <span>Prezime</span>
              <strong>{profile?.lastName ?? "-"}</strong>
            </div>

            <div className="readonly-item">
              <span>Email</span>
              <strong>{profile?.email ?? "-"}</strong>
            </div>

            <div className="readonly-item">
              <span>Korisničko ime</span>
              <strong>{profile?.userName ?? "-"}</strong>
            </div>
          </div>
        </section>

        <section className="page-card">
          <h2 className="section-title">Uređivanje profila</h2>

          <form className="form" onSubmit={handleSubmit}>
            <div className="field">
              <label className="label" htmlFor="fitnessGoalId">
                Fitness cilj
              </label>
              <select
                id="fitnessGoalId"
                name="fitnessGoalId"
                className="input"
                value={form.fitnessGoalId}
                onChange={handleChange}
              >
                <option value="">-- Odaberi cilj --</option>
                {fitnessGoals.map((goal) => (
                  <option key={goal.id} value={goal.id}>
                    {goal.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="field-grid">
              <div className="field">
                <label className="label" htmlFor="dateOfBirth">
                  Datum rođenja
                </label>
                <input
                  id="dateOfBirth"
                  name="dateOfBirth"
                  type="date"
                  className="input"
                  value={form.dateOfBirth}
                  onChange={handleChange}
                />
              </div>

              <div className="field">
                <label className="label" htmlFor="gender">
                  Spol
                </label>
                <input
                  id="gender"
                  name="gender"
                  className="input"
                  value={form.gender}
                  onChange={handleChange}
                  placeholder="npr. Muško / Žensko"
                />
              </div>
            </div>

            <div className="field-grid">
              <div className="field">
                <label className="label" htmlFor="heightCm">
                  Visina (cm)
                </label>
                <input
                  id="heightCm"
                  name="heightCm"
                  type="number"
                  step="0.1"
                  className="input"
                  value={form.heightCm}
                  onChange={handleChange}
                />
              </div>

              <div className="field">
                <label className="label" htmlFor="currentWeightKg">
                  Trenutna težina (kg)
                </label>
                <input
                  id="currentWeightKg"
                  name="currentWeightKg"
                  type="number"
                  step="0.1"
                  className="input"
                  value={form.currentWeightKg}
                  onChange={handleChange}
                />
              </div>
            </div>

            <button className="button button-primary" type="submit" disabled={saving}>
              {saving ? "Spremanje..." : "Spremi promjene"}
            </button>
          </form>
        </section>

        <section className="page-card">
          <h2 className="section-title">Sažetak</h2>

          <div className="readonly-grid">
            <div className="readonly-item">
              <span>Fitness cilj</span>
              <strong>{profile?.fitnessGoalName ?? "Nije odabrano"}</strong>
            </div>

            <div className="readonly-item">
              <span>Visina</span>
              <strong>{formatNumber(profile?.heightCm)} cm</strong>
            </div>

            <div className="readonly-item">
              <span>Težina</span>
              <strong>{formatNumber(profile?.currentWeightKg)} kg</strong>
            </div>

            <div className="readonly-item">
              <span>Datum rođenja</span>
              <strong>{profile?.dateOfBirth ? profile.dateOfBirth.split("T")[0] : "-"}</strong>
            </div>
          </div>
        </section>
      </div>
    </div>
  );
}