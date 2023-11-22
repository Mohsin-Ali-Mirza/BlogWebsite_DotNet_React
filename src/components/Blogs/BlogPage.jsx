import React, { useEffect, useState } from "react"
import BlogCards from "./BlogCards";
import Pagination from "../Pagination/Pagination";
import CategorySelection from "../CategorySelection/CategorySelection";

const BlogPage = () => {

    const [blogs, setBlogs] = useState([])


    // Pages CHANGE PAGE
    const [currentPage, setCurrentPage] = useState(1)
    const pageSize = 12 //blogs per page
    
    //Category
    const [selectedCategory, setSelectedCategory] = useState(null);
    const [activeCategory, setActiveCategory] = useState(null);

    //server-dont-need-this
    // useEffect(()=> {
    //     async function fetchBlogs() {
    //         let url = `http://localhost:5000/blogs?page=${currentPage}&limit=${pageSize}`;
    //         //original url http://localhost:5000/blogs CHANGE PAGE


    //         // filter by category
    //         if(selectedCategory){
    //             url += `&category=${selectedCategory}`;
    //         }
    //         const reponse =  await fetch(url);
    //         const data = await reponse.json();
    //         console.log("hello",data);
    //         setBlogs(data);
    //     }

    //     fetchBlogs()

    

    //     //}, []) CHANGE PAGE
    // }, [currentPage, pageSize, selectedCategory])


    useEffect(() => {
        async function fetchBlogs() {
            try {
                const authToken = localStorage.getItem("authToken"); // Make sure to handle authentication appropriately
                let url = `http://localhost:5000/blogs?page=${currentPage}&limit=${pageSize}`;
                //original url http://localhost:5000/blogs CHANGE PAGE


                // filter by category
                if (selectedCategory) {
                    url += `&category=${selectedCategory}`;
                }

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

                    // Add 'category' attribute to each blog item
                    const blogsWithCategory = myPostsData.map(blog => ({
                        ...blog,
                        category: [blog.teacherName, blog.courseName]
                    }));
                
                    setBlogs(blogsWithCategory);
                }
                else {
                    console.error("Failed to fetch my posts");
                }
            } catch (error) {
                console.error("Error during API request:", error);
            }

            
        };

        fetchBlogs()

        //}, []) CHANGE PAGE
    }, [currentPage, pageSize, selectedCategory])
    


    //CHANGE PAGE
    //Page change Button
    const handlePageChange = (pageNumebr) => {
        setCurrentPage(pageNumebr);
    }

    const handleCategoryChange = (category) => {
        setSelectedCategory(category);
        setCurrentPage(1);
        setActiveCategory(category);
    }

    return (
        <div>
            {/* Category Section */}
            <div><CategorySelection onSelectCategory={handleCategoryChange} selectedCategory={selectedCategory}
            activeCategory={activeCategory}/></div>

            {/* Blog Cards */}
            {/* PAGE CHANGE */}
            <div><BlogCards blogs={blogs} currentPage={currentPage} selectedCategory={selectedCategory} pageSize={pageSize} 
            onSelectCategory={handleCategoryChange}   activeCategory={activeCategory}/></div>

            {/* Next, Previous Page Section */}
            <div><Pagination onPageChange={handlePageChange} blogs={blogs} currentPage={currentPage} pageSize={pageSize}/></div>

        </div>
    )
}

export default BlogPage;