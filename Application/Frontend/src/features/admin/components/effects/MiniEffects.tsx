
import React, { useEffect, useState, useCallback } from "react";
import { effectsApi } from "../../api/effects.api";
import type { EffectDto } from "../../types/effect.types";
import type { Query } from "@/shared/types/result.types";
import MiniPagination from "../MiniPagination";
import Spinner from "@/shared/components/Spinner";
import { useEffectTypesStore } from "@/shared/enums/effectTypes.store";

interface MiniEffectsProps {
  value?: string | null;
  onChange: (effectId?: string) => void;
  className?: string;
}

const PAGE_SIZE = 5;

const MiniEffects: React.FC<MiniEffectsProps> = ({ value, onChange, className }) => {
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [effects, setEffects] = useState<EffectDto[]>([]);
  const [loading, setLoading] = useState(false);

  const effectTypesStore = useEffectTypesStore();
  useEffect(() => { effectTypesStore.load(); }, [effectTypesStore]);

  const fetchEffects = useCallback(async () => {
    setLoading(true);
    try {
      const params: Query = { page, pageSize: PAGE_SIZE };
      const res = await effectsApi.getEffects(params);
      setEffects(res.data.items);
      setTotalPages(Math.max(1, Math.ceil(res.data.totalCount / PAGE_SIZE)));
    } catch (e) {
      setEffects([]);
      setTotalPages(1);
    } finally {
      setLoading(false);
    }
  }, [page]);

  useEffect(() => {
    fetchEffects();
  }, [fetchEffects]);

  return (
    <div className={`flex flex-col gap-2 bg-linear-to-br from-indigo-900 via-violet-900 to-purple-900 rounded-xl p-3 shadow-md ${className || ""}`}>
      <div className="flex flex-col gap-1 max-h-48 overflow-y-auto">
        {loading || effectTypesStore.loading ? (
          <div className="flex justify-center py-6">
            <Spinner size="w-8 h-8" color="text-indigo-400" />
            <h1 className="text-violet-400 text-center py-4">
              Loading Effects
            </h1>
          </div>
        ) : effects.length === 0 ? (
          <div className="text-violet-400 text-center py-4">No effects found.</div>
        ) : (
          effects.map((effect) => {
            const typeObj = effectTypesStore.items.find(t => t.value === effect.type);
            return (
              <button
                type="button"
                key={effect.id}
                onClick={() => onChange(effect.id === value ? undefined : effect.id) }
                className={`w-full text-left px-3 py-2 rounded-lg border border-violet-400 text-violet-200 hover:bg-violet-800 transition-all text-xs font-semibold shadow-sm ${value === effect.id ? "bg-indigo-800/60" : "bg-indigo-950/60"}`}
                style={{ lineHeight: 1.3 }}
              >
                <div className="flex flex-wrap gap-x-2 gap-y-0.5 items-center">
                  <span className="font-semibold">{typeObj ? typeObj.name : `Type: ${effect.type}`}</span>
                  {effect.affects !== undefined && <span>A:{effect.affects}</span>}
                  {effect.points !== undefined && <span>P:{effect.points}</span>}
                  {effect.turns !== undefined && <span>T:{effect.turns}</span>}
                  <span>RT:{effect.requiresTarget ? "Y" : "N"}</span>
                  <span>TP:{effect.targetsPlayer ? "Y" : "N"}</span>
                </div>
              </button>
            );
          })
        )}
      </div>
      <MiniPagination
        page={page}
        totalPages={totalPages}
        onPrev={() => setPage((p) => Math.max(1, p - 1))}
        onNext={() => setPage((p) => Math.min(totalPages, p + 1))}
        className="mt-2 self-center"
      />
    </div>
  );
};

export default MiniEffects;
