import { Invitation } from "@/interfaces/interface";



export default function InvitationCard({ invitation }: { invitation: Invitation }) {

    return (
        <>
            <div className="bg-red-400 text-center m-auto mt-4 rounded-xl min-h-32 w-8/12 text-white">
                <div className=" text-xl">Invitation</div>
                <div>From: {invitation.From}</div>
                <div className="flex justify-evenly mt-2">
                    <button className="bg-white text-red-400 p-2 rounded-md">Accept</button>
                    <button className="bg-white text-red-400 p-2 rounded-md">Reject</button>
                </div>
            </div>
        </>
    );
}
