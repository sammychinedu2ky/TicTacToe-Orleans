'use client'
import { useSignalR } from "@/contexts/SignalRContext";
import { InvitationDTO } from "@/interfaces/interface";
import fetcher from "@/utils/fetch";
import { useRouter } from "next/navigation";
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { KeyedMutator } from "swr";


export default function InvitationCard({ invitation, mutate }: { invitation: InvitationDTO, mutate: KeyedMutator<any> }) {
    const router = useRouter();
   console.log(invitation)
    const notify = (errorMessage: string) => toast(errorMessage)
    const handleInvitation = async (accepted: boolean) => {
        var url = `${process.env.NEXT_PUBLIC_API_SERVER_URL}/api/Invitations`
        if (accepted) {
            url += `/accept/${invitation.id}`
        }
        else {
            url += `/reject/${invitation.id}`
        }
        try {
            var req = await fetcher(url,"PUT")
            if (req.ok) {
                if (accepted) {
                    //notify("Invitation Accepted")
                    router.push(`/game-room/${invitation.gameRoom}`)
                }
                else{
                    mutate();
                }
            }
            else {
                notify("Game Room does not exist")
            }
        } catch (error) {
            notify("An error occured")
        }

    }
    return (
        <>
            <div className="bg-red-400 text-center m-auto mt-4 rounded-xl min-h-32 w-8/12 text-white">
                <div className=" text-xl">Invitation</div>
                <div className="text-wrap ">From: {invitation.from}</div>
                <div className="flex justify-evenly mt-2">
                    <button className="bg-white text-red-400 p-2 rounded-md" onClick={() => handleInvitation(true)}>Accept</button>
                    <button className="bg-white text-red-400 p-2 rounded-md" onClick={() => handleInvitation(false)}>Reject</button>
                </div>
            </div>
        </>
    );
}
