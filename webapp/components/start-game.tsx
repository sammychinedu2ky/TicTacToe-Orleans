"use client"
import { useRouter } from "next/navigation";
import { useActionState, useEffect, useState } from "react";
import { v7 as uuid } from "uuid";
import Invitation from "./invitation";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { SignalRProvider } from "@/contexts/SignalRContext";
import { AppRouterInstance } from "next/dist/shared/lib/app-router-context.shared-runtime";
import notify from "@/utils/notify";
import fetcher from "@/utils/fetch";
import postFetcher from "@/utils/postfetcher";


export default function StartGame() {
    const [opponent, setOpponent] = useState<string>("user");
    const [opponentEmail, setOpponentEmail] = useState<string>("");
    const router = useRouter();
    var action = myAction.bind(null, router)
    let [_, formAction,] = useActionState(action, null)

    const notify = (errorMessage: string) => toast(errorMessage)
    const handleOpponentSelection = (e: React.ChangeEvent<HTMLSelectElement>) => {
        e.preventDefault();
        setOpponent(e.target.value);
    }

    let handleSetEmail = (e: React.ChangeEvent<HTMLInputElement>) => {
        e.preventDefault();
        console.log('cleaning')

        setOpponentEmail(e.target.value)
    }

    return (
        <>
            <div className="md:flex  min-h-[80vh] items-center border-4 border-green-700">
                <div className="basis-full  flex flex-wrap">
                    <div className="basis-full  md:basis-8/12  text-center  border-4 border-red-700">
                        <form action={formAction}>
                            <h3 className="text-5xl font-bold text-red-400 mt-3">Play Against
                                <span className="ml-2 leading-loose ">
                                    <select
                                        name="opponents-selection"
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
                                        <input type="email" name="opponents-email" onChange={handleSetEmail} value={opponentEmail} placeholder="Enter opponents email"
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
                    <div className=" basis-full border-4 border-red-600 md:basis-4/12">
                        <SignalRProvider>
                            <Invitation />
                        </SignalRProvider>
                    </div>
                </div>
            </div>
            <ToastContainer
                position="bottom-right"
                autoClose={4000}
                hideProgressBar={false}
                newestOnTop={false}
                closeOnClick
                rtl={false}
                pauseOnFocusLoss={false}
                draggable={false}
                pauseOnHover={false}
                theme="light"
            />
        </>
    )
}


async function myAction(router: AppRouterInstance, prevState: null | void, queryData: FormData) {


    const email = queryData.get("opponents-email")
    const opponent = queryData.get("opponents-selection")

    if (opponent == "user" && email != null) {
        try {
            let checkIfUserExists = await fetcher(`${process.env.NEXT_PUBLIC_API_SERVER_URL}/api/user/${email}`)
            if (checkIfUserExists.ok) {
                var gameRoom = uuid();
                try {
                    await postFetcher(`${process.env.NEXT_PUBLIC_API_SERVER_URL}/api/gameroom`,{ Id: gameRoom, Type: "User", Email: email })
                    // redirect to game room
                    router.push(`/game-room/${gameRoom}`)
                    return;
                } catch (error) {
                    // tool tip an error occured
                    notify("An error occured")
                    console.error("an error occured", error)
                    return;
                }
            }
        }
        catch (error) {
            notify("An error occured")
            console.error("an error occured", error)
            return;
        }
        notify("User not Found")
        return;
    }
    else {
        let gameRoom = uuid();
        try {
            await postFetcher(`${process.env.NEXT_PUBLIC_API_SERVER_URL}/api/gameroom`, { Id: gameRoom, Type: "Computer" })
            // redirect to game room
            router.push(`/game-room/${gameRoom}`)
            return;
        } catch (error) {
            // tool tip an error occured
            notify("An error occured")
            console.error("an error occured", error)

        }

    }
}

