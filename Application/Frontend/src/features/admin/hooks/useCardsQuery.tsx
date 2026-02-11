import { useEffect, useState } from "react";
import type { CardDto, CardsQuery } from "../types/card.types";
import { cardsApi } from "../api/cards.api";

const useCardsQuery = ({ page, pageSize, search } : CardsQuery) => {
  const [data, setData] = useState<CardDto[]>([]);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<unknown>(null);

  useEffect(() => {
    let cancelled = false;

    const fetchCards = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const res = await cardsApi.getCards({page, pageSize, search});
        console.log(res);
        if (!cancelled) {
          setData(res.data.items);
          setTotalPages(Math.ceil(res.data.totalCount / pageSize));
        }
      } catch (err) {
        if (!cancelled) setError(err);
      } finally {
        if (!cancelled) setIsLoading(false);
      }
    };

    fetchCards();

    return () => {
      cancelled = true;
    };
  }, [page, pageSize, search]);

  return { data, totalPages, isLoading, error };
};

export default useCardsQuery;
