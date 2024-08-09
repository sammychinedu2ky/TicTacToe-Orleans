

let fetcher = async (url: string, method?:string) => {
    if(method){
        return await fetch(url, { method, credentials: 'include' })
    }
    return await fetch(url, { credentials: 'include' })
}
export default fetcher
