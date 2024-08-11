export interface InvitationDTO {
    Id: string; // Guid in C# maps to string in TypeScript
    From: string;
    To: string;
    GameRoom: string; // Guid in C# maps to string in TypeScript
   
}

export interface GameRoomDTO{
    X: string;
    O: string;
    Winner: string;
    Turn: string;
    XWins: number;
    OWins: number;
    Draw: number;
    Board: string[][];
}
