'use client'
import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

const SignalRContext = createContext<signalR.HubConnection | null>(null);
interface SignalRProviderProps {
    children: React.ReactNode;
}
export const SignalRProvider = ({ children}:SignalRProviderProps) => {
  const [connection, setConnection] = useState<signalR.HubConnection|null>(null);
console.log(process.env.NEXT_PUBLIC_API_SERVER_URL)
  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl(`api/gameRoomHub`)
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
