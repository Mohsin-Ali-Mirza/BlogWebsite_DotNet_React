import React from "react"
import BlogPage from "../Blogs/BlogPage";

const MainPage = () => {
  const checkToken = () => {
    const authToken = localStorage.getItem("authToken");
    console.log(authToken);
  }

  const fetchMyPosts = async () => {
    try {
      const authToken = localStorage.getItem("authToken"); // Make sure to handle authentication appropriately
      const response = await fetch("https://localhost:7039/Post", {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${authToken}`,
        },
      });
      if (response.ok) {
        console.log("Successful")
        const myPostsData = await response.json();
        console.log(myPostsData)
      } else {
        console.error("Failed to fetch my posts");
      }
    } catch (error) {
      console.error("Error during API request:", error);
    }
  };

  return (
    <div>
      <div className="px-4 py-32 bg-black mx-auto">
        <div className="text-white text-center">
          <h1 className="text-5xl lg:text-7xl leading-snug font-bold mb-5">Welcome to Educational App</h1>
          <p className="text-gray-100 lg:w-3/5 mx-auto">Start your eduacational journey today and read your first
            article!</p>
        </div>
      </div>
      <div className="max-w-7xl mx-auto">
        <BlogPage />
      </div>
      <div>
        <button onClick={checkToken}>Click Me</button>
      </div>
    </div>



  )
}


export default MainPage;