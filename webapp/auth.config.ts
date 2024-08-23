import Github from "next-auth/providers/github";
import { SignJWT, jwtVerify, errors } from "jose"
import type { NextAuthConfig } from "next-auth"
import NextAuth from "next-auth";
import { redirect } from "next/navigation";

export const authOptions = {
  callbacks: {
    authorized({ request, auth }) {
      //console.log('authoriszed')
      // remove game room from being protected
      // const protectedRoutes = ["/game-room", "/history"]
      const protectedRoutes = [ "/history"]
      console.log("swaaaaaaaaaaaaaaaac")
      const isLoggedIn = !!auth
      let containsProtectedRoute = false;
      for (let route of protectedRoutes) {
        if (request.nextUrl.pathname.includes(route)) {
          containsProtectedRoute = true;
          break;
        }
      }
      if (containsProtectedRoute) {
        if (isLoggedIn) return true
        return false;
      }
      return true
    },
    async jwt({ token }) {
        
      console.log('tokoen in ', token)
      if (token.newUser == undefined ) {
        // add to database
        try{
        const req =  await fetch(`/api/user`, {
            method: 'POST',
            headers: new Headers({
              'Content-Type': 'application/json',
              'Auth-Secret': process.env.AUTH_SECRET!
            }),
            body: JSON.stringify({ Id: token.email, Name: token.name })
          })
          if(req.ok)
          token.newUser = false
        }
        catch(e){
          console.log(e)
          // redirect to home page during error 
          return redirect('/') 
        }
      
        console.log('new user')
        console.log(token)
      }
      return token
    },
  },
  providers: [
    Github
  ],
  session: {
    strategy: "jwt",
  },
  cookies: {
    sessionToken: {
      name: "authToken",
      options:{

      }
    }
  },
  jwt: {
    async encode({ token, secret }) {
      //  console.log('run')
     // console.log(token)
      if (!token) {
        throw new Error("No token provided")
      }
      let encodedSecret = new TextEncoder().encode(secret as string)
      return new SignJWT(token).setIssuedAt().setProtectedHeader({ alg: 'HS256' }).setExpirationTime("30h").sign(encodedSecret)
    },
    async decode({ token, secret }) {
      if (!token) {
        throw new Error("No token provided")
      }
      let encodedSecret = new TextEncoder().encode(secret as string)
      try {
        const { payload } = await jwtVerify(token, encodedSecret);
        return payload;
      } catch (error) {
        if (error instanceof errors.JWTExpired) {
          throw new Error('Token has expired');
        }
        throw new Error(`Token claim invalid`);
      }
    }
  }
} satisfies NextAuthConfig


export const { handlers, auth, signIn, signOut } = NextAuth(authOptions)
