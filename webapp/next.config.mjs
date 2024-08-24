/** @type {import('next').NextConfig} */
const nextConfig = {
    reactStrictMode:false,
    async rewrites(){
        return [
            {
                source:'/api/orleans/:path*',
                destination: `${process.env.API_SERVER_URL}/api/orleans/:path*`
            }
        ]
    }
};

export default nextConfig;
