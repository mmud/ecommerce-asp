if (localStorage.getItem("userId")) {
    window.location.href = "./main.html";
  }
const API_BASE_URL = "https://localhost:7019/api";

document.getElementById("loginForm").addEventListener("submit", async (event) => {
  event.preventDefault();

  const email = document.getElementById("email").value;
  const password = document.getElementById("password").value;

  const payload = {
    Email: email,
    Password: password
  };

    const response = await fetch(`${API_BASE_URL}/Customer/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify(payload),
    });

    const result = await response.json();
    console.log(result);

    localStorage.setItem("userId", result.Data.CustomerId);
    localStorage.setItem("role", result.Data.Token); 
    if(localStorage.getItem("userId"))
        window.location.href = "./main.html";
  
});
