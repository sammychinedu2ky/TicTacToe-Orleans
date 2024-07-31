import Github from "next-auth/providers/github";
import jose,{SignJWT, jwtVerify} from "jose"
import type {NextAuthConfig} from "next-auth"
import NextAuth from "next-auth";

export const authOptions  = {
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
