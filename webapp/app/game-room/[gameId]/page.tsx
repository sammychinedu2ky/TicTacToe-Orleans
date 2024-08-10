'use client'

import { useSignalR } from "@/contexts/SignalRContext"
import { GameRoomDTO } from "@/interfaces/interface"
import fetcher from "@/utils/fetch"
import notify from "@/utils/notify"
import { useSession } from "next-auth/react"
import { useEffect } from "react"
import useSWR from "swr"

export default function Page({ params }: { params: { gameId: string } }) {
    let gameId: string = params.gameId
    const session = useSession()
    const connection = useSignalR()
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
        </div>
    )
}

