"use client"
import { useRouter } from "next/navigation";
import { useActionState, useState } from "react";
import { v7 as uuid } from "uuid";
import Invitation from "./invitation";
import { SignalRProvider } from "@/contexts/SignalRContext";


export default function StartGame() {
    const [opponent, setOpponent] = useState<string>("user");
    const [opponentEmail, setOpponentEmail] = useState<string>("");
    const [state, formAction] = useActionState(myAction, "")
    const handleOpponentSelection = (e: React.ChangeEvent<HTMLSelectElement>) => {
        e.preventDefault();
        setOpponent(e.target.value);
    }
    return (
        <>
            <div className="md:flex  h-[80vh] items-center border-4 border-green-700">
                <div className="basis-full  flex flex-wrap">
                    <div className="basis-full  md:basis-8/12  text-center  border-4 border-red-700">
                        <form action={formAction}>
                            <h3 className="text-5xl font-bold text-red-400">Play Against
                                <span className="ml-2">
                                    <select
                                        name="opponent-selection"
                                        className=" border-red-400 rounded-md text-center  bg-red-400 text-white  focus:ring-red-200 mb-36"
                                        value={opponent} onChange={handleOpponentSelection}>
                                        <option value="user">User</option>
                                        <option value="computer">Computer</option>
                                    </select>
                                </span>
                            </h3>
                            <div>
                                {opponent === "user" ?
                                    <div>
                                        <input type="email" name="opponent-email" onChange={(e) => setOpponentEmail(e.target.value)} value={opponentEmail} placeholder="Enter opponents email"
                                            className="border-2 border-red-400 rounded-md focus:ring-red-200 w-6/12 h-12 mr-5" />

                                        <button type="submit" className="text-white bg-red-400 rounded-md hover:bg-red-500 focus:ring-red-200 h-12 w-3/12">Start Game</button>
                                    </div>
                                    :
                                    <div>
                                        <button type="submit" className="text-white bg-red-400 rounded-md hover:bg-red-500 focus:ring-red-200 h-12 w-3/12">Start Game</button>
                                    </div>}
                            </div>
                        </form>
                    </div>
                    <div className=" basis-full border-4 w-full border-red-600 md:basis-4/12">
                    <SignalRProvider>
                        <Invitation />
                    </SignalRProvider>
                    </div>
                </div>
            </div>
        </>
    )
}


async function myAction(prevState: string | undefined, queryData: FormData) {
    const router = useRouter()
    const email = queryData.get("opponents-email")
    const opponent = queryData.get("opponent")
    if (opponent == "user" && email != null) {
        try {

            let checkIfUserExists = await fetch(`${process.env.NEXT_PUBLIC_API_SERVER_URL}/api/user/${email}`)
            if (checkIfUserExists.ok) {
                var gameRoom = uuid();
                try {
                    await fetch(`${process.env.API_SERVER_URL}/api/gameroom`, {
                        method: 'POST',
                        headers: new Headers({
                            'Content-Type': 'application/json',
                        }),
                        body: JSON.stringify({ Id: gameRoom, Type: "User" })
                    })
                    // redirect to game room
                    router.push(`/game-room/${gameRoom}`)
                } catch (error) {
                    // tool tip an error occured
                    console.error("an error occured", error)
                    alert("An error occured")
                }
            }
            else {
                return "User not Found"
            }
        } catch (error) {
            // tool tip an error occured
            console.error("an error occured", error)
            alert("An error occured")
        }
        return "user"
    }
    else {
        let gameRoom = uuid();
        try {
            await fetch(`${process.env.NEXT_PUBLIC_API_SERVER_URL}/api/gameroom`, {
                method: 'POST',
                headers: new Headers({
                    'Content-Type': 'application/json',
                }),
                body: JSON.stringify({ Id: gameRoom, Type: "Computer" })
            })
            // redirect to game room
            router.push(`/game-room/${gameRoom}`)
        } catch (error) {
            // tool tip an error occured
            console.error("an error occured", error)
            alert("An error occured")
        }
        return "computer"
    }
}

