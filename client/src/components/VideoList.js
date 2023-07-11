import React, { useEffect, useState } from "react";
import Video from "./Video";
import VideoSearch from "./VideoSearch"
import VideoForm from "./VideoForm"
import { getAllVideosWithComments, searchForVideo } from "../modules/VideoManager";

const VideoList = () => {

    const loadingVideo = {
        comments: [{message: "Loading"}],
        name: "Loading",
        userProfile: {name: "Loading"}
    }

  const [videos, setVideos] = useState([loadingVideo]);
  const [searchQuery, setSearchQuery] = useState(null);

  const getVideos = () => {
    getAllVideosWithComments().then(videos => setVideos(videos));
  };

  useEffect(() => {
    getVideos();
  }, []);

  useEffect(() => {
    if(searchQuery !== null){
        setVideos([loadingVideo])
            searchForVideo(searchQuery).then(videos => setVideos(videos))
    }
  }, [searchQuery])

  return (
    <div className="container">
        <VideoSearch setSearchQuery={setSearchQuery}/>
        <div className="row justify-content-center">
            {videos.map(v => 
              <Video video={v} key={v.id + "00251"}/>
            )}
        </div>
        <VideoForm setVideos={setVideos}/>
    </div>
  );
}

export default VideoList;