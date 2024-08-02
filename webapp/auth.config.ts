import Github from "next-auth/providers/github";
import { SignJWT, jwtVerify, errors } from "jose"
import type { NextAuthConfig } from "next-auth"
import NextAuth from "next-auth";

export const authOptions = {
  callbacks: {
    authorized({ request, auth }) {
      //console.log('authoriszed')
      const protectedRoutes = ["/game-room", "/history"]
      const isLoggedIn = !!auth
      if (protectedRoutes.includes(request.nextUrl.pathname)) {
        if (isLoggedIn) return true
        return false;
      }
      return true
    },
    async jwt({ token, user }) {
        console.log('tokoen in ', token)
      if (token.newUser == undefined) {
        token.newUser = false
        // add to database
        await fetch('http://localhost:3000/api/user', {
          method: 'POST',
          headers: new Headers({
            'Content-Type': 'application/json',
            'Auth-Secret': process.env.AUTH_SECRET!
          }),
          body: JSON.stringify({ Id: user.email, Name: user.name })
        })
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
