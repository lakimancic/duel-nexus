import { backImageUrl } from "@/shared/api/httpClient";
import Card from "@/shared/components/card/Card";

const AdminCards = () => {
  return (
    <div className="flex-1 overflow-auto">
      <h1 className="text-5xl font-bold mb-5 text-purple-200 [text-shadow:0_0_0.8rem_#bb00ff] text-center">
        Cards Editor
      </h1>
      <Card
        name={"Blue Dragon"}
        description={"Blue dragon lorem ipsum dolhe vele harlod sakf saorsn"}
        type={0}
        hidden={false}
        className="text-[1rem]"
        attack={1500}
        defense={1000}
        level={7}
        src={backImageUrl("c43969c4-a4bb-4358-950b-d46feffe1700.png")}
      />
    </div>
  );
};

export default AdminCards;
