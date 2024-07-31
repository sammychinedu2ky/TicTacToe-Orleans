export default function Header () {
    return (
        <>
            <nav className="flex h-16 w-full items-center justify-center  bg-red-400">
                <h1 className="text-4xl font-bold text-white">TicTacToe</h1>
                <div className="absolute right-8">
                    {/* <button className="text-xl text-red-400 bg-white rounded-md hover:bg-gray-200 focus:ring-red-200 focus:ring-4 p-2">Login</button>  */}
                    <div className="group relative">
                        <div className="flex h-10 w-10 cursor-pointer items-center justify-center rounded-full bg-white hover:bg-gray-200">S</div>
                        <div className="absolute right-0 hidden cursor-pointer bg-red-400 text-white group-hover:block">
                            <p className="border-b-2 pl-5 pr-5 pt-5">
                                <a>History</a>
                            </p>
                            <p className="pb-3 pl-5 pr-5 pt-2">
                                <a>Logout</a>
                            </p>
                        </div>
                    </div>
                </div>
            </nav>
        </>

    );
}
