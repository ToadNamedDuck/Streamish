const baseUrl = "/api/UserProfiles"

export const GetUserAndVideos = (id) => {
    return fetch(baseUrl+`/GetWithVideos/${id}`).then(response => response.json())
}