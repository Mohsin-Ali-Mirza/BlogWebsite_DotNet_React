import React from 'react'
import { Typography, Box, Divider, Paper } from '@mui/material'
import Attachments from '../components/Post/Attachments'
import Navbar from '../components/common/Navbar'
import { useParams } from 'react-router-dom'
import { useGetPostQuery } from '../features/api.slice'
import Markdown from 'react-markdown'
import remarkGfm from 'remark-gfm' 

function Post() {
    const token = localStorage.getItem("authToken");
    const { id } = useParams()
    const { data, refetch, isLoading, isError } = useGetPostQuery({ token: token, id })

    if (isLoading) return <div>loading...</div>
    if (isError) return <div>oh no, an error!</div>

    return (
        <Box sx={{ minHeight: '100vh' }}>
            <Navbar />
            <Box sx={{ p: 3, mt: '10vh' }}>
                <Typography variant='h3' >{data?.title}</Typography>
                <Divider sx={{ my: 2 }} />
                <Attachments attachments={data?.attachments} />
                <Paper sx={{ my: 5, p: 3, backgroundColor: 'white', borderRadius: 2 }} elevation={1} >
                    <Typography>
                        <Markdown remarkPlugins={[remarkGfm]}>{data?.content}</Markdown>
                    </Typography>
                </Paper>
            </Box>
        </Box>
    )
}

export default Post