'use client'

import { useSignalR } from "@/contexts/SignalRContext"
import { GameRoomDTO } from "@/interfaces/interface"
import fetcher from "@/utils/fetch"
import notify from "@/utils/notify"
import { useSession } from "next-auth/react"
import { useRouter } from "next/navigation"
import { use, useEffect, useState } from "react"
import useSWR from "swr"
import { validate } from "uuid"

export default function Page({ params }: { params: { gameId: string } }) {
    let gameId: string = params.gameId
    const session = useSession()
    const connection = useSignalR()
    const router = useRouter()
    let initialGameState: GameRoomDTO = {
        Board: [[" ", " ", " "], [" ", " ", " "], [" ", " ", " "]],
        Draw: 0,
        O: "",
        X: "sa",
        OWins: 0,
        XWins: 0,
        Turn: "X",
        Winner: ""
    }

    const [gameState, setGameState] = useState<GameRoomDTO>(initialGameState)
    if(!validate(gameId)) {
        notify("Invalid game id")
        return <div>Invalid game id</div>
    }
    const { data, isLoading } = useSWR(`${process.env.NEXT_PUBLIC_API_SERVER_URL}/api/game-room/${gameId}`, async (url: string) => {
        try {
            const res = await fetcher(url)
            if (res.ok) {
                return await res.json()
            }
            else {
                notify("Can't find game room")

                return null
            }
        } catch (error) {

            notify("Error fetching game info")
        }
    }, { revalidateIfStale: false, revalidateOnFocus: false, revalidateOnReconnect: false })
    let isAuthenticated = session.status == "authenticated"
    useEffect(() => {
        if (connection && isAuthenticated) {
            connection.on("ReceiveGameState", (gameRoomDTO: GameRoomDTO) => {
                console.log("received invite realtime", gameRoomDTO)
                setGameState(gameRoomDTO)
            })
            connection.on("ReceiveError", (error: string) => {
                notify(error)
                router.push("/")
            })
        }
    })
    useEffect(() => {
        if (connection && isAuthenticated) {
            connection.invoke("JoinGameRoom", gameId)
        }
    }, [connection])
    let flatBoard = gameState.Board.flat()
    let turn = gameState.Turn
    console.log(flatBoard)


    function handleBoardClick(index: number, cell: string): void {
        if (!isAuthenticated) {
            notify("Not authenticated")
            return
        }
        if (cell.length > 0) {
            notify("Already clicked")
            return
        }
        
        alert(session?.data?.user?.email)
        if (userFromTurn() !== session?.data?.user?.email) {
            notify("Not your turn")
            return
        }
        let [row, col] = customConverter(index)
        let newBoard:GameRoomDTO = { ...gameState }
        newBoard.Board[row][col] = gameState["Turn"]
        connection?.invoke("SendGameState", gameId, newBoard)
    }
    function userFromTurn(){
        
        if(gameState["Turn"] === 'X'){
            return gameState["X"]
        }else{
            return gameState["O"]
        }
    }
    return (
        <>
            {isLoading && <div className="text-center mt-8">Loading...</div>}
            {!isLoading && data &&
                <>
                    <div className="text-center m-auto mt-4">Turn to play: {userFromTurn()}</div>
                    <div className="text-center m-auto grid grid-cols-3  h-[70vh] w-8/12 mt-12 border-8 border-red-400 rounded-md">
                        {flatBoard.map((cell, index) => <div className="border text-center flex items-center justify-center border-red-400 " key={index} onClick={() => handleBoardClick(index, cell)}>{cell}</div>)}
                    </div>
                </>}
            <div className="text-center m-auto mt-4">Turn to play: {userFromTurn()}</div>
            <div className="text-center m-auto grid grid-cols-3  h-[50vh] w-8/12 mt-12 border-8 border-red-400 rounded-md">
                {flatBoard.map((cell, index) => <div className="border text-center flex items-center justify-center border-red-400 " key={index} onClick={() => handleBoardClick(index, cell)}>{cell}</div>)}
            </div>
        </>
    )
}


function customConverter(n: number) {
    const firstDigit = Math.floor(n / 3);
    const secondDigit = n % 3;

    return [firstDigit, secondDigit]
}
