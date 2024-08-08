
import { auth } from "@/auth.config";
import Header from "@/components/header";
import StartGame from "@/components/start-game";
import { SessionProvider } from "next-auth/react";
import Image from "next/image";

export default async function Home() {
  const session = await auth()
  return (
    <main >
      <SessionProvider session={session} refetchOnWindowFocus={false}>
        <StartGame/>
      </SessionProvider>
    </main>
  );
}
