import type { AuthResponse, LoginDto, RegisterDto } from "../types/auth";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "https://localhost:7032";

async function readErrorMessage(response: Response): Promise<string> {
  const raw = await response.text();

  if (!raw) {
    return response.statusText || "Dogodila se greška.";
  }

  try {
    const parsed = JSON.parse(raw) as { title?: string; message?: string; error?: string };
    return parsed.title || parsed.message || parsed.error || raw;
  } catch {
    return raw;
  }
}

async function postJson<T>(url: string, body: unknown): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${url}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return (await response.json()) as T;
}

async function postText(url: string, body: unknown): Promise<string> {
  const response = await fetch(`${API_BASE_URL}${url}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(body),
  });

  if (!response.ok) {
    throw new Error(await readErrorMessage(response));
  }

  return await response.text();
}

export const authService = {
  login: async (dto: LoginDto): Promise<AuthResponse> => {
    return await postJson<AuthResponse>("/api/Auth/login", dto);
  },

  register: async (dto: RegisterDto): Promise<string> => {
    return await postText("/api/Auth/register", dto);
  },
};