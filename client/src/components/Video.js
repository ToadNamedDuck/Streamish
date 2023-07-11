import React from "react";
import { Card, CardBody, CardHeader } from "reactstrap";

const Video = ({ video }) => {
  return (
    <Card >
      <p className="text-left px-2">Posted by: {video.userProfile.name}</p>
      <CardBody>
        <iframe className="video"
          src={video.url}
          title="YouTube video player"
          frameBorder="0"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen />

        <p>
          <strong>{video.title}</strong>
        </p>
        <p>{video.description}</p>
        <Card>
            <CardHeader>Comments</CardHeader>
            <CardBody>
                {
                    video.comments ?
                    video.comments.map(comment => {
                        return <CardBody>{comment.message}</CardBody>
                    })
                    :
                    ""
                }
            </CardBody>
        </Card>
      </CardBody>
    </Card>
  );
};

export default Video;