/** @type {import('next').NextConfig} */
const nextConfig = {
    reactStrictMode:false,
    async rewrites(){
        return [
            {
                source:'/api/:path*',
                destination: `${process.env.API_SERVER_URL}/api/:path*`
            }
        ]
    }
};

export default nextConfig;
