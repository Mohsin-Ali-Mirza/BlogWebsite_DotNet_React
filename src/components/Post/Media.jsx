import { Box, Card, CardContent, Typography } from '@mui/material'
import React from 'react'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faFile } from '@fortawesome/free-solid-svg-icons'

function Media({ name, source, type, showDoc, setVideo }) {
    return (
        <Card
            sx={{
                display: 'flex',
                width: 200,
                p: 3,
                pl: 0,
                cursor: 'pointer',
                '&:active': { boxShadow: 0 }
            }}
            elevation={2}
            onClick={() => {
                if (type.includes('video')) {
                    setVideo(source)
                } else {
                    showDoc(source)
                }
            }}
        >
            <Box sx={{ width: 100, display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                <Box sx={{ borderRadius: '100%', backgroundColor: '#dddddd' }}>
                    <FontAwesomeIcon icon={faFile} size='2xl' />
                </Box>
            </Box>
            <Box sx={{ width: 100, display: 'flex', alignItems: 'center' }}>
                <Typography variant="body1" sx={{ whiteSpace: 'nowrap', overflowX: 'hidden', textOverflow: 'ellipsis', }}>
                    {name}
                </Typography>
            </Box>
        </Card>
    )
}

export default Media