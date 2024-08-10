'use client'

import { useSignalR } from "@/contexts/SignalRContext"
import { GameRoomDTO } from "@/interfaces/interface"
import fetcher from "@/utils/fetch"
import notify from "@/utils/notify"
import { useSession } from "next-auth/react"
import { useEffect, useState } from "react"
import useSWR from "swr"

export default function Page({ params }: { params: { gameId: string } }) {
    let gameId: string = params.gameId
    const session = useSession()
    const connection = useSignalR()
    let initialGameState: GameRoomDTO = {
        board: [["x", "o", "x"], ["a", "b", "c"], ["", "", ""]],
        draw: 0,
        o: "",
        x: "",
        oWins: 0,
        xWins: 0,
        turn: "",
        winner: ""
    }

    const [gameState, setGameState] = useState<GameRoomDTO>(initialGameState)
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
    })
    let isAuthenticated = session.status == "authenticated"
    useEffect(() => {
        if (connection && isAuthenticated) {
            connection.on("ReceiveGameState", (gameRoomDTO: GameRoomDTO) => {
                console.log("received invite realtime", gameRoomDTO)
                // Task ReceiveInviteAsync(string userId, InvitationDTO invite);
            })
        }
    })
    let flatBoard = gameState.board.flat()
    let turn = gameState.turn
    console.log(flatBoard)
    return (
        <>
            {isLoading && <div className="text-center mt-8">Loading...</div>}
            {!isLoading && data &&
                <>
                    <div className="text-center m-auto mt-4">Turn to play: {turn}</div>
                    <div className="text-center m-auto grid grid-cols-3  h-[70vh] w-8/12 mt-12 border-8 border-red-400 rounded-md">
                        {flatBoard.map((cell, index) => <div className="border text-center flex items-center justify-center border-red-400 " key={index}>{cell}</div>)}
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
