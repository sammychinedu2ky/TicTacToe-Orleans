import Github from "next-auth/providers/github";
import jose,{SignJWT, jwtVerify} from "jose"
import type {NextAuthConfig} from "next-auth"
import NextAuth from "next-auth";

export const authOptions  = {
    callbacks: {
      authorized({request,auth}){
        console.log("authorized",request,auth)
      const protectedRoutes = ["/game-room","/history"]
      const isLoggedIn = !!auth
      if(protectedRoutes.includes(request.nextUrl.pathname)){
        if(isLoggedIn) return true
        return false;
      }
      return true
    }
    },
    providers: [
        Github
    ],
    session:{
        strategy: "jwt",
    },
    cookies:{
        sessionToken:{
            name: "authToken",
        }
    },
    jwt:{
      async encode({token, secret, maxAge}){
        if(!token){
          throw new Error("No token provided")
        }
        let encodedSecret = new TextEncoder().encode(secret as string)
        return new SignJWT(token).setIssuedAt().setExpirationTime(maxAge as number).sign( encodedSecret )
      },
      async decode({token, secret}){
        if(!token){
          throw new Error("No token provided")
        }
        let encodedSecret = new TextEncoder().encode(secret as string)
        try {
            const { payload } = await jwtVerify(token, encodedSecret);
            return payload;
          } catch (error) {
            if (error instanceof jose.errors.JWTExpired) {
              throw new Error('Token has expired');
            }
              throw new Error(`Token claim invalid`);
          }
      }
    }
} satisfies NextAuthConfig


export const { handlers, auth, signIn, signOut } = NextAuth(authOptions)
