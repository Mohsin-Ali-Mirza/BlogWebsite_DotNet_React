import React from "react"
import { useNavigate } from "react-router-dom";

const BlogCards = ({ blogs, currentPage, selectedCategory, pageSize, onSelectCategory, activeCategory }) => {
    console.log("BLOGS:", blogs)
    const filteredBlogs = blogs
        .filter((blog) => !selectedCategory || blog.category.includes(selectedCategory))
        .slice((currentPage - 1) * pageSize, currentPage * pageSize);

    const navigate = useNavigate()

    const getRandomColor = () => {
        const letters = "0123456789ABCDEF";
        let color = "#";
        for (let i = 0; i < 6; i++) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    };

    const gradientStyle = {
        background: `linear-gradient(45deg, ${getRandomColor()}, ${getRandomColor()})`,
        color: "#000", // Black text
        padding: "20px",
        textAlign: "center",
    };

    return (
        <div className="grid md:grid-cols-3 sm:grid-cols-2 grid-cols-1 gap-8"  >
            {
                filteredBlogs.map((blog) => <div key={blog.id} onClick={() => navigate(`/post/${blog.id}`)} className="p-5 shadow-lg rounded cursor-pointer" style={gradientStyle}>
                    <div>
                        {/* <img src={blog.image} alt="" className="w-full" /> */}
                        <h3 className="mt-4 mb-2 font-bold hover:text-blue-600 cursor-pointer">{blog.title}</h3>
                        <div>
                            {Array.isArray(blog.category) ? (
                                blog.category.map((category, index) => (
                                    <button
                                        key={index}
                                        onClick={() => onSelectCategory(category)}
                                        className={`hover:text-blue-600 cursor-pointer inline-block ${activeCategory === category ? "active-button" : ""
                                            } bg-gray-200 rounded-full px-3 py-1 text-sm font-semibold text-gray-700 mr-2`}
                                    >
                                        {category}
                                    </button>
                                ))
                            ) : (
                                blog.category && (
                                    <button
                                        onClick={() => onSelectCategory(blog.category)}
                                        className={`hover:text-blue-600 cursor-pointer inline-block ${activeCategory === blog.category ? "active-button" : ""
                                            } bg-gray-200 rounded-full px-3 py-1 text-sm font-semibold text-gray-700 mr-2`}
                                    >
                                        {blog.category}
                                    </button>
                                )
                            )}
                        </div>
                    </div>
                </div>)
            }
        </div>



    )
}


export default BlogCards;