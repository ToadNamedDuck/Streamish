import React from "react";
import { Link } from "react-router-dom";
import { Card, CardBody, CardHeader } from "reactstrap";

const Video = ({ video }) => {
  return (
    <Card >
      {
        video.userProfile ?
        <Link to={`/users/${video.userProfile.id}`} className="text-left px-2">Posted by: {video.userProfile.name}</Link>
        :
        ""
      }
        
      <CardBody>
        <iframe className="video"
          src={video.url}
          title="YouTube video player"
          frameBorder="0"
          allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
          allowFullScreen />

        <p>
          <Link to={`/videos/${video.id}`}>
            <strong>{video.title}</strong>
          </Link>
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