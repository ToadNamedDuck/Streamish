import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Card, CardBody, CardHeader } from "reactstrap";
import { GetUserAndVideos } from "../modules/UserManager";
import Video from "./Video";

const UserProfile = ({loadingVideo}) => {

    const {id} = useParams();

    const defaultProfile = {
        id: 1,
        name: "Loading",
        email: "load@ing.com",
        videos: [loadingVideo]
    }

    const [user, setUser] = useState(defaultProfile)

    useEffect(() => {
        GetUserAndVideos(id).then(user => setUser(user))
    }, [])

    return <>
        <Card>
            <CardHeader>
                <h2>
                    {user.name}
                </h2>
                <h4>
                    {
                        user.videos ? 
                        user.videos.length.toString() + " Videos"
                        :
                        "No Videos"
                    }
                    
                </h4>
            </CardHeader>
            <CardBody>
                {
                    user.videos ?
                    user.videos.map(video => <Video video={video}/>)
                    :
                    "No videos :("
                }
            </CardBody>
        </Card>
    </>
}

export default UserProfile;