import httpClient from "./httpClient";
import type { ApiResponse } from "../types/common.types";
import type {
  AuthSession,
  ChangePasswordRequest,
  CurrentUser,
  LoginRequest,
} from "../types/auth.types";

function requireData<T>(response: ApiResponse<T>): T {
  if (!response.succeeded || response.data === null) {
    throw new Error(
      response.message || "The API response did not contain data.",
    );
  }

  return response.data;
}

export async function login(request: LoginRequest): Promise<AuthSession> {
  const response = await httpClient.post<ApiResponse<AuthSession>>(
    "/auth/login",
    request,
  );

  return requireData(response.data);
}

export async function getCurrentUser(): Promise<CurrentUser> {
  const response = await httpClient.get<ApiResponse<CurrentUser>>("/auth/me");
  return requireData(response.data);
}

export async function changePassword(
  request: ChangePasswordRequest,
): Promise<boolean> {
  const response = await httpClient.post<ApiResponse<boolean>>(
    "/auth/change-password",
    request,
  );

  return requireData(response.data);
}
