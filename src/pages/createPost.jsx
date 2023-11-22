import React, { useState, useRef } from "react";
import axios from "axios";
import {
  Typography,
  Box,
  Divider,
  Container,
  Paper,
  TextField,
  Button,
} from "@mui/material";

import { faYoutube } from "@fortawesome/free-brands-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faImage,
  faAward,
  faChartColumn,
} from "@fortawesome/free-solid-svg-icons";
import Navbar from "../components/common/Navbar";
import UploadButton from "../components/common/UploadButton";

function CreatePost() {
  const [uploadStatus, setUploadStatus] = useState("");
  const [content, setContent] = useState("");
  const [postId, setpostId] = useState("");
  

  const handlePostClick = async () => {
    try {
      const response = await axios.post(
        "https://localhost:7039/Post/CreatePost",
        {
          title: "IPT project",
          content: content,
          courseId: 0,
        },
        {
          headers: {
            Authorization:
              "Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEzIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoidGVhY2hlciIsImV4cCI6MTcwMDU3MDI5Nn0.-2wL2iWVe2gPyP8tpLyAp-emei3lVrSzejWKIMcDyDa8rHyDGJHuJ9_VhglMFa1rFR3lecRl2_fZcyMDb5jQJQ",
          },
        }
      );
      setpostId(response.body.postId);
      console.log("Post created successfully!");
    } catch (error) {
      console.error("Failed to create post!", error);
    }
  };

  return (
    <Box sx={{ minHeight: "100vh" }}>
      <Navbar />
      <Box sx={{ p: 3, mt: "10vh" }}>
        <Typography
          sx={{
            color: "#000",
            fontFamily: "Montserrat",
            fontSize: "35px",
            fontStyle: "normal",
            fontWeight: "800",
            lineHeight: "normal",
          }}
        >
          Create a Post
        </Typography>
        <Paper
          sx={{
            my: 5,
            p: 3,
            backgroundColor: "white",
            borderRadius: "2",
            width: "702px",
            height: "400px",
            flexShrink: "0",
          }}
          elevation={1}
        >
          <Box>
            <TextField
              id="outlined-multiline-static"
              label="What do you want to talk about?"
              multiline
              rows={12}
              defaultValue=""
              variant="outlined"
              fullWidth
              InputProps={{
                style: { border: "none" },
              }}
              onChange={(event) => setContent(event.target.value)}
            />
          </Box>
          <div
            style={{
              display: "flex",
              justifyContent: "space-between",
              marginTop: "10px",
            }}
          >
            <div>
              <UploadButton icon={faImage} postId={postId}/>
              <UploadButton icon={faYoutube} postId={postId}/>
              <UploadButton icon={faAward} postId={postId}/>
              <UploadButton icon={faChartColumn} postId={postId}/>
            </div>
            <Button
              variant="contained"
              color="primary"
              onClick={handlePostClick}
            >
              Post
            </Button>
          </div>
          {uploadStatus && <p>{uploadStatus}</p>}
        </Paper>
      </Box>
    </Box>
  );
}

export default CreatePost;
