'use client'

import { useSignalR } from "@/contexts/SignalRContext"
import { signIn, useSession } from "next-auth/react"
import useSWR from "swr"
import dummy from "./invitationdummy"
import InvitationCard from "./invitation-card"
import fetcher from "@/utils/fetch"
import { useEffect } from "react"
import { InvitationDTO } from "@/interfaces/interface"
import notify from "@/utils/notify"
export default function Invitation() {
    let dum = dummy.slice(0, 10)
    const session = useSession()
    const connection = useSignalR()
    const { data, error, isLoading, mutate } = useSWR(`/api/orleans/invitations/my-invites`, async (url) => {
        try {
            if(session.status == "authenticated"){
            const res = await fetcher(url)
            return await res.json()
            }
        } catch (error) {
            console.log("error fetching invites", error)
            notify("Error fetching invites")
        }
    })
    useEffect(() => {
        if (connection && session.status == "authenticated") {
            connection.on("ReceiveInvite", (invitation: InvitationDTO) => {
               if(data != undefined){
                mutate({data:[invitation,...data]}).then(() => console.log("mutated"))
               }
               else{
                   mutate({ invitation }).then(() => console.log("mutated"))
               }
            })
        }
        return () => {connection?.stop()}
    },[connection])
    // use tool tip to show error
    console.log(error)

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
                    {!isLoading && data && data.length > 0 && data.map((invitation: InvitationDTO) => <InvitationCard key={invitation.id} invitation={invitation} mutate={mutate} />)}
                    {/* {dum.map((invite: any) => <InvitationCard key={invite.Id} invitation={invite} />)} */}
                </div>}
        </>
    );
}
