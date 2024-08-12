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
        board: [[" ", " ", " "], [" ", " ", " "], [" ", " ", " "]],
        draw: 0,
        o: "",
        x: "",
        oWins: 0,
        xWins: 0,
        turn: "X",
        winner: ""
    }

    const [gameState, setGameState] = useState<GameRoomDTO>(initialGameState)
    if (!validate(gameId)) {
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
            console.log('swacky')
            connection.on("ReceiveGameState", (gameRoomDTO: GameRoomDTO) => {
                console.log("received invite realtime", gameRoomDTO)
                setGameState(gameRoomDTO)
            })
            connection.on("ReceiveError", (error: string) => {
                notify(error)
                console.log("error", error)
                //router.push("/")
            })
            connection.on("Connected",()=>{
                connection.invoke("JoinGameRoom", gameId)
            })
        }
    },[connection])
 
    let flatBoard = gameState.board.flat()
    let turn = gameState.turn
    console.log(flatBoard)


    function handleBoardClick(index: number, cell: string): void {
        if (!isAuthenticated) {
            notify("Not authenticated")
            return
        }
        if (cell === 'x' || cell === 'o') {
            notify("Already clicked")
            return
        }

        alert(session?.data?.user?.email)
        if (userFromTurn() !== session?.data?.user?.email) {
            notify("Not your turn")
            return
        }
        let [row, col] = customConverter(index)
        let newBoard: GameRoomDTO = { ...gameState }
        newBoard.board[row][col] = gameState["turn"]
        connection?.invoke("SendGameState", gameId, newBoard)
    }
    function userFromTurn() {

        if (gameState["turn"] === 'x') {
            return gameState["x"]
        } else {
            return gameState["o"]
        }
    }
    return (
        <>
            {isLoading && <div className="text-center mt-8">Loading...</div>}
            {!isLoading && data &&
                <>
                    <div className="text-center m-auto mt-4">Turn to play: {userFromTurn()}</div>
                    <div className="text-center m-auto grid grid-cols-3  h-[50vh] w-8/12 mt-12 border-8 border-red-400 rounded-md">
                        {flatBoard.map((cell, index) => <div className="border text-center flex items-center justify-center border-red-400 " key={index} onClick={() => handleBoardClick(index, cell)}>{cell.toUpperCase()}</div>)}
                    </div>
                </>}
        </>
    )
}


function customConverter(n: number) {
    const firstDigit = Math.floor(n / 3);
    const secondDigit = n % 3;

    return [firstDigit, secondDigit]
}
