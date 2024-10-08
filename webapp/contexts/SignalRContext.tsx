'use client'
import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

const SignalRContext = createContext<signalR.HubConnection | null>(null);
interface SignalRProviderProps {
    children: React.ReactNode;
}
export const SignalRProvider = ({ children}:SignalRProviderProps) => {
  const [connection, setConnection] = useState<signalR.HubConnection|null>(null);
  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`/api/orleans/gameRoomHub`,{
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection) {
      connection.start()
        .then(() => {
          console.log('Connected to SignalR');
        })
        .catch(e => console.log('Connection failed: ', e));
    
    }
  }, [connection]);

  return (
    <SignalRContext.Provider value={connection}>
      {children}
    </SignalRContext.Provider>
  );
};

export const useSignalR = () => {
  return useContext(SignalRContext);
};
