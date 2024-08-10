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
    return (

        <div className="text-center">
            {isLoading && isAuthenticated && <div>Loading...</div>}
            {data != null && isAuthenticated &&
                <div>hi</div>
            }
            <div className="border-2 border-red-900 w-9/12 h-[80vh] m-auto text-black">
                <div className="flex">
                    {gameState.board[0].map(box => (
                        <div className="border-2 border-red-900 w-4/12 h-8/12">{box}</div>
                    ))}
                </div>
                <div className="flex">
                    {gameState.board[1].map(box => (
                        <div className="border-2 border-red-900 w-4/12 h-4/12">{box}jjjjjj</div>
                    ))}
                </div>
                <div className="flex">
                    {gameState.board[2].map(box => (
                        <div className="border-2 border-red-900 w-4/12 h-4/12">{box}ssss</div>
                    ))}
                </div>
            </div>
        </div>
    )
}

