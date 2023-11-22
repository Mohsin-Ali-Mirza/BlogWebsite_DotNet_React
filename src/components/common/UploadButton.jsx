import React, { useRef } from 'react';
import axios from 'axios';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import Button from '@mui/material/Button';

function UploadButton({ icon, postId }) {
  const fileInputRef = useRef();

  const handleButtonClick = () => {
    fileInputRef.current.click();
  };

  const handleFileChange = async (event) => {
    const file = event.target.files[0];
    console.log(file);

    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await axios.post(
        `https://localhost:7039/Attachment/Store/${postId}`,
        formData,
        {
          headers: {
            'Authorization': 'Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEzIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoidGVhY2hlciIsImV4cCI6MTcwMDU3MDI5Nn0.-2wL2iWVe2gPyP8tpLyAp-emei3lVrSzejWKIMcDyDa8rHyDGJHuJ9_VhglMFa1rFR3lecRl2_fZcyMDb5jQJQ',
          },
        }
      );
      console.log(response);
    } catch (error) {
      console.error('Failed to upload file!', error);
    }
  };

  return (
    <>
      <Button variant="none" color="normal" sx={{ m: 0, p:0 }} onClick={handleButtonClick}>
        <FontAwesomeIcon icon={icon} style={{ marginRight: "10px" }} />
      </Button>
      <input
        type="file"
        style={{ display: 'none' }}
        ref={fileInputRef}
        onChange={handleFileChange}
        accept="/*"
      />
    </>
  );
}

export default UploadButton;
