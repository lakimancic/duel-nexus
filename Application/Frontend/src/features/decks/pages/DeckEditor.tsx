import DeckEditorPanel from "../components/DeckEditorPanel";

const DeckEditorPage = () => {
  return (
    <div className="h-screen w-full p-4 md:p-6">
      <DeckEditorPanel showBackToLobby />
    </div>
  );
};

export default DeckEditorPage;
