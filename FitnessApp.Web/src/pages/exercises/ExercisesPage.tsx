import { useEffect, useMemo, useState } from "react";
import type { ChangeEvent } from "react";
import { exerciseService } from "../../services/exerciseService";
import { lookupService } from "../../services/lookupService";
import type { LookupDto } from "../../types/lookups";
import type { ExerciseReadDto } from "../../types/exercise";

interface FilterState {
  search: string;
  exerciseCategoryId: string;
  muscleGroupId: string;
}

export default function ExercisesPage() {
  const [exercises, setExercises] = useState<ExerciseReadDto[]>([]);
  const [categories, setCategories] = useState<LookupDto[]>([]);
  const [muscleGroups, setMuscleGroups] = useState<LookupDto[]>([]);
  const [selectedExercise, setSelectedExercise] = useState<ExerciseReadDto | null>(null);

  const [filters, setFilters] = useState<FilterState>({
    search: "",
    exerciseCategoryId: "",
    muscleGroupId: "",
  });

  const [loading, setLoading] = useState(true);
  const [loadingFilters, setLoadingFilters] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const queryParams = useMemo(
    () => ({
      search: filters.search,
      exerciseCategoryId: filters.exerciseCategoryId ? Number(filters.exerciseCategoryId) : null,
      muscleGroupId: filters.muscleGroupId ? Number(filters.muscleGroupId) : null,
    }),
    [filters]
  );

  useEffect(() => {
    const loadFilters = async () => {
      try {
        setLoadingFilters(true);

        const [categoriesData, muscleGroupsData] = await Promise.all([
          lookupService.getExerciseCategories(),
          lookupService.getMuscleGroups(),
        ]);

        setCategories(categoriesData);
        setMuscleGroups(muscleGroupsData);
      } catch (err) {
        setError(err instanceof Error ? err.message : "Greška pri dohvaćanju filtera.");
      } finally {
        setLoadingFilters(false);
      }
    };

    void loadFilters();
  }, []);

  useEffect(() => {
    const loadExercises = async () => {
      try {
        setLoading(true);
        setError(null);

        const data = await exerciseService.getAll(queryParams);
        setExercises(data);

        if (data.length > 0 && !selectedExercise) {
          setSelectedExercise(data[0]);
        }

        if (selectedExercise) {
          const refreshedSelected = data.find((x) => x.id === selectedExercise.id) ?? data[0] ?? null;
          setSelectedExercise(refreshedSelected);
        }
      } catch (err) {
        setError(err instanceof Error ? err.message : "Greška pri dohvaćanju vježbi.");
      } finally {
        setLoading(false);
      }
    };

    void loadExercises();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [queryParams.search, queryParams.exerciseCategoryId, queryParams.muscleGroupId]);

  const handleChange = (event: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = event.target;

    setFilters((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleReset = () => {
    setFilters({
      search: "",
      exerciseCategoryId: "",
      muscleGroupId: "",
    });
  };

  return (
    <div className="exercises-page">
      <section className="hero-card">
        <div>
          <p className="eyebrow">Baza vježbi</p>
          <h1 className="title">Vježbe</h1>
          <p className="subtitle">
            Pretražuj, filtriraj i otvaraj detalje vježbi s video uputama.
          </p>
        </div>

        <div className="hero-badge">
          <span>Ukupno vježbi</span>
          <strong>{loading ? "..." : exercises.length}</strong>
        </div>
      </section>

      {error && <div className="error-box">{error}</div>}

      <section className="page-card">
        <div className="exercises-toolbar">
          <div className="field">
            <label className="label" htmlFor="search">
              Pretraživanje
            </label>
            <input
              id="search"
              name="search"
              className="input"
              value={filters.search}
              onChange={handleChange}
              placeholder="npr. bench, squat, row..."
            />
          </div>

          <div className="field">
            <label className="label" htmlFor="exerciseCategoryId">
              Kategorija
            </label>
            <select
              id="exerciseCategoryId"
              name="exerciseCategoryId"
              className="input"
              value={filters.exerciseCategoryId}
              onChange={handleChange}
              disabled={loadingFilters}
            >
              <option value="">Sve kategorije</option>
              {categories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.name}
                </option>
              ))}
            </select>
          </div>

          <div className="field">
            <label className="label" htmlFor="muscleGroupId">
              Mišićna skupina
            </label>
            <select
              id="muscleGroupId"
              name="muscleGroupId"
              className="input"
              value={filters.muscleGroupId}
              onChange={handleChange}
              disabled={loadingFilters}
            >
              <option value="">Sve skupine</option>
              {muscleGroups.map((group) => (
                <option key={group.id} value={group.id}>
                  {group.name}
                </option>
              ))}
            </select>
          </div>

          <div className="exercise-toolbar-actions">
            <button className="button button-secondary" type="button" onClick={handleReset}>
              Reset filtera
            </button>
          </div>
        </div>
      </section>

      <div className="exercises-grid">
        <section className="page-card exercises-list-card">
          <div className="section-header">
            <h2 className="section-title">Popis vježbi</h2>
            <span className="section-muted">{loading ? "Učitavanje..." : `${exercises.length} rezultata`}</span>
          </div>

          <div className="exercises-list">
            {exercises.map((exercise) => (
              <button
                key={exercise.id}
                type="button"
                className={`exercise-list-item ${selectedExercise?.id === exercise.id ? "active" : ""}`}
                onClick={() => setSelectedExercise(exercise)}
              >
                <div className="exercise-list-item-head">
                  <strong>{exercise.name}</strong>
                  <span>{exercise.exerciseCategoryName}</span>
                </div>

                <p>{exercise.muscleGroupName}</p>
              </button>
            ))}

            {!loading && exercises.length === 0 && (
              <div className="empty-state">
                Nema rezultata za odabrane filtere.
              </div>
            )}
          </div>
        </section>

        <section className="page-card exercise-detail-card">
          <h2 className="section-title">Detalji vježbe</h2>

          {selectedExercise ? (
            <div className="exercise-detail">
              <div className="detail-block">
                <span>Naziv</span>
                <strong>{selectedExercise.name}</strong>
              </div>

              <div className="detail-block">
                <span>Kategorija</span>
                <strong>{selectedExercise.exerciseCategoryName}</strong>
              </div>

              <div className="detail-block">
                <span>Mišićna skupina</span>
                <strong>{selectedExercise.muscleGroupName}</strong>
              </div>

              <div className="detail-block">
                <span>Opis</span>
                <p>{selectedExercise.description}</p>
              </div>

              {selectedExercise.youtubeUrl && (
                <a
                  className="button button-primary exercise-video-link"
                  href={selectedExercise.youtubeUrl}
                  target="_blank"
                  rel="noreferrer"
                >
                  Otvori YouTube video
                </a>
              )}
            </div>
          ) : (
            <div className="empty-state">Odaberi vježbu iz popisa.</div>
          )}
        </section>
      </div>
    </div>
  );
}