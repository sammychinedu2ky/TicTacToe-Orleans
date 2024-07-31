"use client"
import { useActionState, useState } from "react";




export default function StartGame() {
    const [opponent, setOpponent] = useState<string>("user");
    const [opponentEmail, setOpponentEmail] = useState<string>("");
    const [state, formAction] = useActionState(myAction,"")
    const handleOpponentSelection = (e: React.ChangeEvent<HTMLSelectElement>) => {
        e.preventDefault();
        setOpponent(e.target.value);
    }
    return (
        <>
            <div className="flex items-center flex-wrap">
                <div className="basis-full md:basis-8/12 p-28 ">
                    <h3 className="text-xl font-bold text-red-400">Play Against
                        <span className="ml-2">
                            <select
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
                            <input type="email" name="opponent-email" onChange={(e)=>setOpponentEmail(e.target.value)} value={opponentEmail} placeholder="Enter opponents email" className="border-2 border-red-400 rounded-md focus:ring-red-200" />
                            
                            <button className="text-white bg-red-400 rounded-md hover:bg-red-500 focus:ring-red-200">Start Game</button>
                        </div> 
                        : 
                        <div>computer</div>}
                    </div>
                </div>
                <div className=" basis-full md:basis-4/12">jio</div>
            </div>
        </>
    )
}


async function myAction(prevState:string | undefined, queryData:FormData) {
  const email = queryData.get("opponents-email")
    if(email){
        return "user"
    }
    return "computer"
}

