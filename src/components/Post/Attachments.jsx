import React, { useState, useEffect } from 'react'
import { Box } from '@mui/material'
import VideoPlayer from './VideoPlayer'
import Media from './Media'
import MediaViewer from './MediaViewer'

function Attachments({ attachments }) {
    const [video, setVideo] = useState('')

    const [doc, setDoc] = useState({
        show: false,
        uri: ''
    })

    useEffect(() => {
        for (let i = 0; i < attachments.length; i++) {
            if (attachments[i].type.includes('video')) {
                setVideo(attachments[i].path)
                break
            }
        }
    }, [])

    function showDoc(source) {
        setDoc(prevState => ({
            ...prevState,
            show: true,
            uri: source,
        }))
    }

    function closeDoc() {
        setDoc(prevState => ({
            ...prevState,
            show: false,
        }))
    }

    return (
        <Box>
            <Box sx={{ width: '100%', display: 'flex', justifyContent: 'center', alignItems: 'center', py: video === '' ? 0 : 5 }}>
                {video !== '' && <VideoPlayer source={video} />}
            </Box>
            <Box sx={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, 200px)', gap: 2 }}>
                {attachments.map(media => {
                    if (media.path !== video) {
                        return (
                            <Media
                                key={media.id}
                                name={media.path.split('/').slice(-1)}
                                source={media.path}
                                showDoc={showDoc}
                                setVideo={setVideo}
                                type={media.type}
                            />
                        )
                    }
                })}
            </Box>

            {doc.show && <MediaViewer source={doc.uri} closeDoc={closeDoc} />}

        </Box>
    )
}

export default Attachments