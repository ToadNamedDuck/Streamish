import { useEffect, useState } from "react"
import { CardHeader, Card, CardBody, FormGroup } from "reactstrap"
import { addVideo, getAllVideosWithComments } from "../modules/VideoManager"

const VideoForm = ({setVideos}) => {

    const [videoToSubmit, setVideo] = useState({
        Title: "",
        Description: "",
        Url: ""
    })

    const changeValue = (key, value) => {//this function gets all of the keys on the videoToSubmit object, and checks to see if the
        //supplied key exists or not on the object. if it does, it copies the object, sets the value of the key on the copy, and replaces
        //the original with the edited copy.
        Object.keys(videoToSubmit).forEach(keyIterator => {
            if(keyIterator === key){
                const videoCopy = {...videoToSubmit}
                videoCopy[keyIterator] = value;
                setVideo(videoCopy)
            }
        })

        }

    const submitOnClickHandler = () => {
        addVideo(videoToSubmit);
        getAllVideosWithComments()
            .then(videos => setVideos(videos))
    }

    return <>
        <Card>
            <CardHeader>
                <h2>
                    Submit Video
                </h2>
            </CardHeader>
            <CardBody>
                <FormGroup class="column">
                    <input type="text" placeholder="Title" required onChange={ e => {changeValue("Title", e.target.value)} } />
                    <input type="text" placeholder="Description" onChange={e => {changeValue("Description", e.target.value)} } />
                    <input type="text" placeholder="Video Link" required onChange={e => {changeValue("Url", e.target.value)} } />
                    <button type="button" onClick={submitOnClickHandler}>Submit</button>
                </FormGroup>
            </CardBody>
        </Card>
    </>
}

export default VideoForm;