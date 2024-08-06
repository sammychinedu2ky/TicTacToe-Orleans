interface Invite {
    id: string; // Guid in C# maps to string in TypeScript
    from: string;
    to: string;
    gameRoom: string; // Guid in C# maps to string in TypeScript
    newInvite: boolean;
    accept: boolean;
}


export default function InvitationCard({ invite }: { invite: Invite }) {

return (
    <></>
);
}
