import { useEffect } from "react";
import { useForm, type SubmitHandler } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import useCardById from "../../hooks/useCardById";
import Modal from "../Modal";
import Spinner from "@/shared/components/Spinner";
import Card from "@/shared/components/Card";
import { useCardTypesStore } from "@/shared/enums/cardTypes.store";
import { getImageUrl } from "@/shared/api/httpClient";

const cardSchema = z.object({
  name: z.string().min(1, "Name is required"),
  description: z.string().min(10, "Description is required"),
  level: z.preprocess(
    (v) => (Number.isNaN(v) ? undefined : Number(v)),
    z.number().int().min(1).optional(),
  ),
  attack: z.preprocess(
    (v) => (Number.isNaN(v) ? undefined : Number(v)),
    z.number().int().optional(),
  ),
  defense: z.preprocess(
    (v) => (Number.isNaN(v) ? undefined : Number(v)),
    z.number().int().optional(),
  ),
  type: z.preprocess(
    (v) => Number(v),
    z.number().int().min(0, "Type is required"),
  ),
  image: z.string().min(1, "Image is required"),
});

type CardForm = z.infer<typeof cardSchema>;

const CardModal = ({
  cardId,
  onClose,
}: {
  cardId?: string | null;
  onClose: () => void;
}) => {
  const cardQuery = cardId
    ? useCardById(cardId)
    : { data: null as any, isLoading: false, error: null };
  const { data, isLoading } = cardQuery;

  const {
    register,
    handleSubmit,
    reset,
    watch,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<CardForm>({
    resolver: zodResolver(cardSchema) as any,
    defaultValues: {
      name: "",
      description: "",
      attack: undefined,
      defense: undefined,
      type: -1,
    },
  });

  const cardTypes = useCardTypesStore((state) => state.items);

  useEffect(() => {
    if (data) {
      reset({
        name: data.name ?? "",
        description: data.description ?? "",
        attack: typeof data.attack === "number" ? data.attack : undefined,
        defense: typeof data.defense === "number" ? data.defense : undefined,
        type: data.type ?? "",
        image: data.image ?? "",
        level: typeof data.level === "number" ? data.level : undefined,
      });
    } else {
      reset({
        name: "",
        description: "",
        attack: undefined,
        defense: undefined,
        type: 0,
      });
    }
  }, [data, cardId, reset]);

  // Handler for image upload
  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    // TODO: Call API to upload image and get URL
    // Example: const url = await uploadImageApi(file);
    // setValue('image', url);
    // For now, just set the file name as a placeholder
    // Remove this line when implementing real upload
    setValue("image", file.name);
  };

  const onSubmit: SubmitHandler<CardForm> = (values) => {
    console.log(cardId ? "editing card" : "creating card", values);
    onClose();
  };

  return (
    <Modal onClose={onClose}>
      <div className="bg-linear-to-br from-indigo-900 via-violet-900 to-purple-900 rounded-2xl shadow-2xl p-6 min-w-175 max-w-[95vw]">
        {isLoading ? (
          <div className="w-full flex flex-col items-center justify-center p-20">
            <Spinner size="size-20 text-indigo-400" />
            <h1 className="text-indigo-300 text-xl my-5 font-semibold">
              Loading Card
            </h1>
          </div>
        ) : (
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="flex flex-row gap-8 text-white"
          >
            <div className="flex-1 flex flex-col gap-4">
              <h1 className="text-center text-3xl font-bold py-2 text-violet-200 [text-shadow:0_0_0.8rem_#7c3aed]">
                {cardId ? "Edit" : "Add New"} Card
              </h1>
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-indigo-200">Name</label>
                <input
                  {...register("name")}
                  className="p-2 w-full rounded-lg border border-violet-400 bg-indigo-950/60 placeholder:text-violet-300/60 text-violet-200 outline-none focus:ring-2 focus:ring-violet-500 transition-all"
                />
                {errors.name && (
                  <span className="text-pink-400 text-xs font-semibold mt-1">
                    {errors.name.message}
                  </span>
                )}
              </div>
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-indigo-200">
                  Description
                </label>
                <textarea
                  {...register("description")}
                  className="p-2 w-full rounded-lg border border-violet-400 bg-indigo-950/60 placeholder:text-violet-300/60 text-violet-200 outline-none focus:ring-2 focus:ring-violet-500 transition-all min-h-17.5 resize-y"
                />
                {errors.description && (
                  <span className="text-pink-400 text-xs font-semibold mt-1">
                    {(errors.description as any).message}
                  </span>
                )}
              </div>
              <div className="flex flex-col gap-1">
                <label className="font-semibold text-indigo-200">Type</label>
                <select
                  {...register("type", { required: true })}
                  className="p-2 w-full rounded-lg border border-violet-400 bg-indigo-950/60 text-violet-200 outline-none focus:ring-2 focus:ring-violet-500 transition-all"
                >
                  {cardTypes.map((t) => (
                    <option key={t.value} value={t.value}>
                      {t.name}
                    </option>
                  ))}
                </select>
              </div>
              <div className="flex flex-col gap-1">
                  <label className="font-semibold text-indigo-200">Image</label>
                  <div className="flex items-center gap-3">
                    <input
                      type="file"
                      accept="image/*"
                      onChange={handleImageUpload}
                      className="cursor-pointer block w-fit text-sm text-gray-400 file:mr-4 file:py-2 file:px-4 file:rounded-lg file:border-0 file:text-sm file:font-semibold file:bg-violet-700 file:text-white hover:file:bg-violet-800"
                    />
                    {watch("image") ? (
                      <span className="text-green-300 font-semibold text-sm">
                        {watch("image")}
                      </span>
                    ) : (
                      <span className="text-pink-400 font-semibold">
                        No image
                      </span>
                    )}
                  </div>
                </div>
              <div className="flex flex-row gap-4 items-end w-full">
                <div className="flex flex-col gap-1 flex-1">
                  <label className="font-semibold text-indigo-200">Level</label>
                  <input
                    type="number"
                    {...register("level", { valueAsNumber: true })}
                    className="p-2 w-full rounded-lg border border-violet-400 bg-indigo-950/60 placeholder:text-violet-300/60 text-violet-200 outline-none focus:ring-2 focus:ring-violet-500 transition-all"
                  />
                  {errors.level && (
                    <span className="text-pink-400 text-xs font-semibold mt-1">
                      {(errors.level as any).message}
                    </span>
                  )}
                </div>
                <div className="flex flex-col gap-1 flex-1">
                  <label className="font-semibold text-indigo-200">Attack</label>
                  <input
                    type="number"
                    {...register("attack", { valueAsNumber: true })}
                    className="p-2 w-full rounded-lg border border-violet-400 bg-indigo-950/60 placeholder:text-violet-300/60 text-violet-200 outline-none focus:ring-2 focus:ring-violet-500 transition-all"
                  />
                  {errors.attack && (
                    <span className="text-pink-400 text-xs font-semibold mt-1">
                      {(errors.attack as any).message}
                    </span>
                  )}
                </div>
                <div className="flex flex-col gap-1 flex-1">
                  <label className="font-semibold text-indigo-200">Defense</label>
                  <input
                    type="number"
                    {...register("defense", { valueAsNumber: true })}
                    className="p-2 w-full rounded-lg border border-violet-400 bg-indigo-950/60 placeholder:text-violet-300/60 text-violet-200 outline-none focus:ring-2 focus:ring-violet-500 transition-all"
                  />
                  {errors.defense && (
                    <span className="text-pink-400 text-xs font-semibold mt-1">
                      {(errors.defense as any).message}
                    </span>
                  )}
                </div>
              </div>
              <div className="flex flex-row gap-4 mt-7 justify-start items-center">
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="cursor-pointer bg-linear-to-r from-green-600 via-emerald-500 to-green-700 px-6 py-2 rounded-lg font-bold text-white shadow-md hover:via-green-400 transition-all disabled:opacity-60 disabled:cursor-not-allowed"
                >
                  {isSubmitting ? "Saving..." : "Save"}
                </button>
                <button
                  type="button"
                  onClick={onClose}
                  className="cursor-pointer bg-linear-to-r from-slate-600 via-gray-500 to-slate-700 px-6 py-2 rounded-lg font-bold text-white shadow-md hover:via-gray-400 transition-all"
                >
                  Cancel
                </button>
                {cardId && (
                  <button
                    type="button"
                    onClick={() => {
                      /* TODO: implement delete logic */
                    }}
                    className="cursor-pointer bg-linear-to-r from-pink-700 via-red-500 to-pink-800 px-6 py-2 rounded-lg font-bold text-white shadow-md hover:via-red-400 transition-all ml-auto"
                  >
                    Delete
                  </button>
                )}
              </div>
            </div>
            <div className="flex flex-col items-center justify-center min-w-65 max-w-[320px]">
              <Card
                name={watch("name")}
                description={watch("description")}
                type={watch("type")}
                attack={
                  Number.isNaN(watch("attack")) ? undefined : watch("attack")
                }
                defense={
                  Number.isNaN(watch("defense")) ? undefined : watch("defense")
                }
                level={
                  Number.isNaN(watch("level")) ? undefined : watch("level")
                }
                hidden={false}
                className="text-[1rem] text-black"
                src={getImageUrl(watch("image"))}
              />
            </div>
          </form>
        )}
      </div>
    </Modal>
  );
};

export default CardModal;
