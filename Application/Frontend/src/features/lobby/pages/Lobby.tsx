import { gameHub } from "@/shared/realtime/gameHub";
import type { MessageDto } from "@/shared/types/message.types";
import { useEffect, useState } from "react";

const LobbyPage = () => {
  const [message, setMessage] = useState('');

  useEffect(() => {
    const messageHandler = (msg: MessageDto) => {
      console.log("RECIEVED: ", msg);
    };

    gameHub.onGlobalChat(messageHandler);

    return () => {
      gameHub.offGlobalChat(messageHandler);
    };
  }, []);

  return (
    <div className="h-screen w-full relative flex justify-center items-center">
      <h1>LOBBY</h1>
      <input type="text" value={message} onChange={msg => setMessage(msg.currentTarget.value)} className="border"/>
      <button type="button" onClick={() => gameHub.sendGlobalChat(message)} className="bg-amber-300">SEND</button>
    </div>
  );
}

export default LobbyPage;
