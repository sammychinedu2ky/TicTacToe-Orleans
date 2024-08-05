'use client'

import { useSignalR } from "@/contexts/SignalRContext"
import { signIn, useSession } from "next-auth/react"
import useSWR from "swr"
import InvitationCard from "./invitation-card"
export default function Invitation() {
    const session = useSession()
    const signalR = useSignalR()
    const { data, error,isLoading,mutate } = useSWR("/api/invites", async (url) => {
        const res = await fetch(url)
        return res.json()
    })
    let isUserAuthenticated = session.status == "authenticated"

    return (
        <>
            {!isUserAuthenticated ?
                <div className="text-center mt-8">
                    <h3 className="text-red-400 font-bold text-2xl">Login to view invites</h3>
                    <button className="text-white bg-red-400 rounded-md hover:bg-red-500 focus:ring-red-200 h-12 w-3/12" onClick={() => signIn("github")}>Login</button>
                </div>




                : <div>
                    {isLoading && <div className="text-center mt-8">Loading...</div>}
                    {!isLoading && data && data.length > 0 && data.map((invite:any)=><InvitationCard key={invite.Id} invite={invite}/>)}
                </div>}
        </>
    );
}
