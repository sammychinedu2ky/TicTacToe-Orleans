import { SignalRProvider } from "@/contexts/SignalRContext";

export default async function RootLayout({
    children,
}: Readonly<{
    children: React.ReactNode;
}>) {

    return (
        <SignalRProvider>
            {children}
        </SignalRProvider>
    );
}
