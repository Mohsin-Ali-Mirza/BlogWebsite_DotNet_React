import { Box } from '@mui/material'
import React from 'react'
import { BigPlayButton, LoadingSpinner, Player } from 'video-react'

function VideoPlayer({ source }) {
    return (
        <Box sx={{ width: 600 }}>
            <Player src={source} aspectRatio={'16:9'}>
                <LoadingSpinner />
                <BigPlayButton position="center" />
            </Player>
        </Box>
    )
}

export default VideoPlayer