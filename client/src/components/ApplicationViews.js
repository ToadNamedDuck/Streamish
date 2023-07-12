import { Routes, Route } from "react-router-dom";
import VideoList from "./VideoList";
import VideoForm from "./VideoForm";
import VideoDetails from "./VideoDetails";
import UserProfile from "./UserProfile";

const ApplicationViews = () => {

    const loadingVideo = {
        comments: [{ message: "Loading" }],
        name: "Loading",
        userProfile: { name: "Loading" },
        Id: -1
    }

    return (
        <Routes>
            <Route path="/" >
                <Route index element={<VideoList loadingVideo={loadingVideo} />} />
                <Route path="videos">
                    <Route index element={<VideoList loadingVideo={loadingVideo}/>} />
                    <Route path="add" element={<VideoForm />} />
                    <Route path=":id" element={<VideoDetails />} />
                </Route>
                <Route path="users/:id" element={<UserProfile loadingVideo={loadingVideo}/>}/>
            </Route>
            <Route path="*" element={<p>Whoops, nothing here...</p>} />
        </Routes>
    );
};

export default ApplicationViews;