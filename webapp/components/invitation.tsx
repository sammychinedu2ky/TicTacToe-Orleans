'use client'

import { useSignalR } from "@/contexts/SignalRContext"
import { signIn, useSession } from "next-auth/react"
import useSWR from "swr"
import dummy  from "./invitationdummy"
import InvitationCard from "./invitation-card"
export default function Invitation() {
    let dum = dummy.slice(0, 10)
    const session = useSession()
    const signalR = useSignalR()
    console.log("rendered")
    const { data, error,isLoading,mutate } = useSWR(`${process.env.NEXT_PUBLIC_API_SERVER_URL}/api/invite/my-invites`, async (url) => {

        const res = await fetch(url,{credentials: 'include'})
        return res.json()
    })
    // use tool tip to show error
    console.log(error)
    console.log(data)
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
                    {/* {!isLoading && data && data.length > 0 && data.map((invite:any)=><InvitationCard key={invite.Id} invite={invite}/>)} */}
                    {dum.map((invite: any) => <InvitationCard key={invite.Id} invitation={invite} />)}
                </div>}
        </>
    );
}
