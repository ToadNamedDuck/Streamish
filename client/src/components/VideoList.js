import React, { useEffect, useState } from "react";
import Video from "./Video";
import VideoSearch from "./VideoSearch"
import { searchForVideo } from "../modules/VideoManager";
import { getAllVideosWithComments } from "../modules/VideoManager";

const VideoList = ( {loadingVideo} ) => {

  const [searchQuery, setSearchQuery] = useState(null);

  const [videos, setVideos] = useState([loadingVideo]);

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
              <Video video={v}/>
            )}
        </div>
    </div>
  );
}

export default VideoList;