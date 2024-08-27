import { auth, signIn, signOut } from "@/auth.config";
import Link from "next/link";

import { redirect } from "next/navigation";
export default async function Header() {
    const session = await auth()
    const authenticated = !!session
    const iconLetter = () => {
        if (session != undefined && session.user != undefined && session.user.name != undefined) {
            return session.user.name[0].toUpperCase()
        }
        else return "G"
    }
    const usersEmail = () => {
        if (session != undefined && session.user != undefined && session.user.email != undefined) {
            return session.user.email
        }
        else return "No email"
    }
    const signInToGitHub = async () => {
        "use server"
        return signIn("github", undefined, { prompt: "select_account" })
    }
    const signOutFromGitHub = async () => {
        "use server"
       return redirect("/api/auth/signout");
      //return signOut()
    }
    return (
        <>
            <nav className="flex relative  h-16  items-center pl-4 justify-start md:justify-center md:pl-0  bg-red-400">
                <h1 className="text-4xl font-bold text-white">TicTacToe</h1>
                <div className="absolute right-8">
                    {
                        !authenticated ?
                            <form action={signInToGitHub}>
                                <button className="text-xl text-red-400 bg-white rounded-md hover:bg-gray-200 focus:ring-red-200
                         focus:ring-4 p-2">Login</button>
                            </form>
                            :
                            <div className="group relative">
                                <div className="flex h-10 w-10 cursor-pointer items-center justify-center rounded-full bg-white hover:bg-gray-200">{iconLetter()}</div>
                                <div className="absolute right-0 hidden cursor-pointer bg-red-400 text-white group-hover:block">
                                    <div className=" pl-5 pr-5 pt-3 pb-2 hover:bg-red-500 cursor-default">
                                        <div>{usersEmail()}</div>
                                    </div>
                                    <div className=" pl-5 pr-5 pt-3 pb-2 hover:bg-red-500">
                                        <Link href="/history">History</Link>
                                    </div>
                                    <div className="pb-3 pl-5 pr-5 pt-2  hover:bg-red-500">
                                        <form action={signOutFromGitHub}>
                                            <button type="submit" className="w-full h-full">Logouts</button>
                                        </form>
                                    </div>
                                </div>
                            </div>
                    }

                </div>
            </nav>
        </>

    );
}
