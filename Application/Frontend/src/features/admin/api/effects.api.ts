import { httpClient } from "@/shared/api/httpClient"
import type { InformativeResult, PagedResult } from "@/shared/types/result.types"
import type { CreateEffectDto, EffectDto } from "../types/effect.types";
import type { EnumDto } from "@/shared/types/enum.types";

export const effectsApi = {

    getEffects:()=> httpClient.get<PagedResult<EffectDto>>('/admin/effects'),

    getEffectById:(id:string)=> httpClient.get<EffectDto>(`/admin/effects/${id}`),

    createEffect: (dto:CreateEffectDto) => httpClient.post<EffectDto>('/admin/effects',dto),

    deleteEffect:(id:string) => httpClient.delete<InformativeResult>(`/admin/effects/${id}`),

    getTypes:() => httpClient.get<EnumDto>(`/admin/effects/types`)

}