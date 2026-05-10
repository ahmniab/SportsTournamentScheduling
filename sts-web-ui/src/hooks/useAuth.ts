import { useEffect, useState, useCallback } from 'react';
import api from '../lib/api';

type User = any;

export default function useAuth() {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<any | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get('/User/me');
      setUser(res.data || null);
    } catch (err: any) {
      if (err?.response?.status === 401) {
        setUser(null);
      } else {
        setError(err);
      }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  return {
    user,
    loading,
    error,
    isAuthenticated: !!user,
    reload: load,
  } as const;
}
