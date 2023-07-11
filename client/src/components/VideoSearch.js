import { useState } from "react";

const VideoSearch = ({ setSearchQuery }) => {
    const [tempQuery, setTempQuery] = useState("");

    return <>
        <input type="text" placeholder="Search for videos..."
            
            onKeyDown={e => {
                if(e.key === "Enter" || e.keyCode === 13 && tempQuery !== "" && tempQuery !== null){
                    setSearchQuery(tempQuery);
                }
            } } 
            onChange={e => setTempQuery(e.target.value)} />
    </>
}

export default VideoSearch;