import { useState } from "react";
import PaginationControls from "../components/PaginationControls";
import SearchBar from "../components/SearchBar";
import useCardsQuery from "../hooks/useCardsQuery";
import CardsGrid from "../components/cards/CardsGrid";
import CardModal from "../components/cards/CardModal";

const PAGE_SIZE = 20;

const AdminCards = () => {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");

  const { data, totalPages, isLoading } = useCardsQuery({
    page,
    pageSize: PAGE_SIZE,
    search,
  });

  const [selectedCardId, setSelectedCardId] = useState<string | null>(null);
  const [modalOpen, setModalOpen] = useState(false);

  const openCardModal = (cardId: string) => {
    setSelectedCardId(cardId);
    setModalOpen(true);
  };

  const openCreateModal = () => {
    setSelectedCardId(null);
    setModalOpen(true);
  };

  const closeModal = () => {
    setModalOpen(false);
    setSelectedCardId(null);
  };

  return (
    <div className="flex-1 overflow-auto">
      <h1 className="text-4xl font-bold mb-10 text-purple-200 [text-shadow:0_0_0.8rem_#bb00ff] text-center">
        Cards Editor
      </h1>
      <div className="w-[50%] mx-auto my-5 flex">
        <button
          onClick={openCreateModal}
          className="bg-indigo-500 text-white px-3 rounded-md mr-5 cursor-pointer hover:bg-indigo-600 transition-colors duration-200"
        >
          Add New Card
        </button>
        <SearchBar onSearch={setSearch} placeholder="Search Cards..." />
      </div>
      <CardsGrid
        cards={data}
        isLoading={isLoading}
        onCardClick={openCardModal}
      />
      {!isLoading && data.length > 0 && (
        <div className="flex justify-center items-center">
          <PaginationControls
            page={page}
            totalPages={totalPages}
            onPrev={() => setPage((p) => Math.max(1, p - 1))}
            onNext={() => setPage((p) => Math.min(totalPages, p + 1))}
          />
        </div>
      )}
      {modalOpen && <CardModal cardId={selectedCardId} onClose={closeModal} />}
    </div>
  );
};

export default AdminCards;
