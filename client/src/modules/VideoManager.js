const baseUrl = '/api/video';

export const getAllVideos = () => {
  return fetch(baseUrl)
    .then((res) => res.json())
};

export const getAllVideosWithComments = () => {
    return fetch(baseUrl+"/GetWithComments")
      .then((res) => res.json())
  };

  export const searchForVideo = (q) => {
    return fetch(baseUrl+`/search?q=${q}&sortDesc=false`)
                .then((response) => response.json())
  }

export const addVideo = (video) => {
  return fetch(baseUrl, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(video),
  });
};