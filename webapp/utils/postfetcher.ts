const postFetcher = async (url: string, obj: any) => {
    return await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        credentials: 'include',
        body: JSON.stringify(obj)
    })
}
export default postFetcher
