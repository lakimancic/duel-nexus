import { useEffect, useState } from "react";
import type { CardDto } from "../types/card.types";
import { cardsApi } from "../api/cards.api";

const useCardById = (id: string) => {
  const [data, setData] = useState<CardDto | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<unknown>(null);

  useEffect(() => {
    if (!id) return;

    let cancelled = false;

    const fetchCard = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const res = await cardsApi.getCard(id);

        if (!cancelled) {
          setData(res.data);
        }
      } catch (err) {
        if (!cancelled) {
          setError(err);
        }
      } finally {
        if (!cancelled) {
          setIsLoading(false);
        }
      }
    };

    fetchCard();

    return () => {
      cancelled = true;
    };
  }, [id]);

  return {
    data,
    isLoading,
    error,
  };
};

export default useCardById;
