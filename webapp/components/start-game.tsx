"use client"
import { useRouter } from "next/navigation";
import { useActionState, useState } from "react";
import { v7 as uuid } from "uuid";


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
            <div className="flex items-center flex-wrap">
                <div className="basis-full md:basis-8/12 p-28 ">
                    <form action={formAction}>
                        <h3 className="text-xl font-bold text-red-400">Play Against
                            <span className="ml-2">
                                <select
                                    name="opponent-selection"
                                    className="border-2 border-red-400 rounded-md   focus:ring-red-200"
                                    value={opponent} onChange={handleOpponentSelection}>
                                    <option value="user">User</option>
                                    <option value="computer">Computer</option>
                                </select>
                            </span>
                        </h3>
                        <div>
                            {opponent === "user" ?
                                <div>
                                    <input type="email" name="opponent-email" onChange={(e) => setOpponentEmail(e.target.value)} value={opponentEmail} placeholder="Enter opponents email" className="border-2 border-red-400 rounded-md focus:ring-red-200" />

                                    <button type="submit" className="text-white bg-red-400 rounded-md hover:bg-red-500 focus:ring-red-200">Start Game</button>
                                </div>
                                :
                                <div>
                                    <button type="submit" className="text-white bg-red-400 rounded-md hover:bg-red-500 focus:ring-red-200">Start Game</button>
                                </div>}
                        </div>
                    </form>
                </div>
                <div className=" basis-full md:basis-4/12">jio</div>
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

            let checkIfUserExists = await fetch(`http://localhost:5103/api/user/${email}`)
            if (checkIfUserExists.ok) {
                var gameRoom = uuid();
                try {
                    await fetch(`http://localhost:5103/api/gameroom`, {
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
            await fetch(`http://localhost:5103/api/gameroom`, {
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

