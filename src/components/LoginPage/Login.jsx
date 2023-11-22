import React, {useState} from "react"
import Logo from "../../Assets/logo.png"
import { useNavigate } from 'react-router-dom';


function Login ()
{
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleLogin = async (e) => {
    e.preventDefault();
    
    try {

      const response = await fetch("https://localhost:7039/User/Login", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          email,
          password,
        }),
      });

      if (response.ok) {
        console.log("Login successful");
        const responseBody = await response.json();
        const { token } = responseBody; // Extract only the "token" par

        // Save token in localStorage
        localStorage.setItem("authToken", token);
        navigate('/blog');

      } else {
        console.error("Login failed");
      }
    } catch (error) {
      console.error("Error during login:", error);
    }
  };

  const handleEmailChange = (e) => {
    setEmail(e.target.value);
  };

  const handlePasswordChange = (e) => {
    setPassword(e.target.value);
  };

    return (
        <div class="flex h-screen">

        <div class="w-full bg-gray-100 lg:w-1/2 flex items-center justify-center">
          <div class="max-w-md w-full p-6">
            <h1 style={{fontSize:"3rem", textAlign:'center'}}>Educational App</h1>
            <h1 class="text-3xl font-semibold mb-6 text-black text-center">Login</h1>
            <div class="mt-4 flex flex-col lg:flex-row items-center justify-between">
            </div>
            <form onSubmit={handleLogin} class="space-y-4">
              <div>
                <label for="email" class="block text-sm font-medium text-gray-700">Email</label>
                <input type="text" id="email" name="email" value={email} onChange={handleEmailChange} class="mt-1 p-2 w-full border rounded-md focus:border-gray-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-300 transition-colors duration-300" />
              </div>
              <div>
                <label for="password" class="block text-sm font-medium text-gray-700">Password</label>
                <input type="password" id="password" name="password" value={password} onChange={handlePasswordChange} class="mt-1 p-2 w-full border rounded-md focus:border-gray-200 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-300 transition-colors duration-300" />
              </div>
              <div>
                <button type="submit" class="w-full bg-orange-500 text-white p-2 rounded-md hover:bg-gray-800  focus:bg-black focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-900 transition-colors duration-300">Login</button>
              </div>
            </form>
            <div class="mt-4 text-sm text-gray-600 text-center">
        <p>
          Dont have an Account?{" "}
          <button
            onClick={() => navigate("/register")}
            className="text-black hover:underline focus:outline-none"
          >
            Sign Up here
          </button>
        </p>
      </div>
          </div>
        </div>
      
        <div className="hidden lg:flex items-center justify-center relative flex-1 bg-orange-500 text-black">
          <img
            src={Logo}
            alt="Logo"
            className="absolute left-[-20%] top-1/2 transform -translate-y-1/2 lg:h-auto lg:w-4/5 xl:w-3/5"
            />
          <div className="max-w-md text-center">
          </div>
      </div>
    </div>


      
    )
}

export default Login