import React from "react"
import { useState, useEffect } from "react"

const CategorySelection = ({onSelectCategory, activeCategory}) => {

    const [categories, setCategories] = useState([]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        // Get the Bearer token from localStorage
        const token = localStorage.getItem("authToken"); // Make sure to handle authentication appropriately
        if (!token) {
          console.error("Token not found in localStorage");
          return;
        }

        // Make parallel requests to get courses and teachers with the Bearer token
        const [coursesResponse, teachersResponse] = await Promise.all([
          fetch("https://localhost:7039/Post/GetCourses", {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }),
          fetch("https://localhost:7039/User/GetTeachers", {
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }),
        ]);

        if (coursesResponse.ok && teachersResponse.ok) {
          const coursesData = await coursesResponse.json();
          const teachersData = await teachersResponse.json();

          // Extract names from courses and teachers data
          const courseNames = coursesData.map((course) => course.name);
          const teacherNames = teachersData.map((teacher) => teacher.name);

          // Merge names and set to state
          setCategories([...courseNames, ...teacherNames]);
        } else {
          console.error("Failed to fetch data");
        }
      } catch (error) {
        console.error("Error during data fetching:", error);
      }
    };

    fetchData();
  }, []); // Empty dependency array ensures this effect runs once after the component mounts

    return (
        <div className="px-4 mb-8 lg:space-x-16 flex flex-wrap items-center border-b-2 py-5 text-gray-900 font-semibold">
            <button onClick={()=> onSelectCategory(null)} className={`lg:ml-12 ${activeCategory ? "":
            "active-button"}`}>All</button>
                {
                    categories.map((category) => (
                        <button 
                        onClick={()=> onSelectCategory(category)}
                        className={`mr-2 space-x-16 ${activeCategory === category ? "active-button" : ""}`}
                        key={category}>
                        {category} </button>
                    ))
                }
        </div>



    )
}


export default CategorySelection;