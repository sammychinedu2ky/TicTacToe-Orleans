export interface InvitationDTO {
    id: string; // Guid in C# maps to string in TypeScript
    from: string;
    to: string;
    gameRoom: string; // Guid in C# maps to string in TypeScript
   
}

export interface GameRoomDTO{
    x: string;
    O: string;
    winner: string;
    turn: string;
    xWins: number;
    oWins: number;
    draw: number;
    board: string[][];
}
