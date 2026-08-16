import React, { createContext, useContext, useEffect, useMemo, useState } from "react";

type CurrentUser = {
  id: number;
  name: string;
  lastName: string;
  username: string;
  email: string;
};

type AuthContextValue = {
  accessToken: string | null;
  refreshToken: string | null;
  currentUser: CurrentUser | null;
  isAuthLoading: boolean;
  setTokens: (accessToken: string, refreshToken: string) => void;
  clearAuth: () => void;
  refetchMe: () => Promise<void>;
};

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode; meEndpoint: string }> = ({
  children,
  meEndpoint,
}) => {
        const [accessToken, setAccessToken] = useState<string | null>(() => sessionStorage.getItem("accessToken"));
        const [refreshToken, setRefreshToken] = useState<string | null>(() => sessionStorage.getItem("refreshToken"));
        const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
        const [isAuthLoading, setIsAuthLoading] = useState(true);

        const clearAuth = () => {
            sessionStorage.removeItem("accessToken");
            sessionStorage.removeItem("refreshToken");
            sessionStorage.removeItem("loginTime");
            sessionStorage.removeItem("userId");

            setAccessToken(null);
            setRefreshToken(null);
            setCurrentUser(null);
        };

        const setTokens = (newAccessToken: string, newRefreshToken: string) => {
            sessionStorage.setItem("accessToken", newAccessToken);
            sessionStorage.setItem("refreshToken", newRefreshToken);
            sessionStorage.setItem("loginTime", Date.now().toString());

            setAccessToken(newAccessToken);
            setRefreshToken(newRefreshToken);
        };

        const refetchMe = async () => {
            if (!accessToken) {
            setCurrentUser(null);
            return;
            }

            const res = await fetch(meEndpoint, {
            headers: { Authorization: `Bearer ${accessToken}` },
            });

            if (res.status === 401) {
            // token nije validan/istekao
            clearAuth();
            return;
            }

            if (!res.ok) {
            // možeš ovde logovati, ali nemoj crash
            setCurrentUser(null);
            return;
            }

            const data: CurrentUser = await res.json();
            setCurrentUser(data);
        };

        useEffect(() => {
            (async () => {
            try {
                await refetchMe();
            } finally {
                setIsAuthLoading(false);
            }
            })();
        }, [accessToken]);

        const value = useMemo<AuthContextValue>(
            () => ({
            accessToken,
            refreshToken,
            currentUser,
            isAuthLoading,
            setTokens,
            clearAuth,
            refetchMe,
            }),
            [accessToken, refreshToken, currentUser, isAuthLoading]
        );

        return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
    };

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
};
