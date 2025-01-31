if (!localStorage.getItem("userId")) {
    // Redirect to login or register page if not authenticated
    window.location.href = "./login.html"; // Replace with your login or register page
  }

const employeeName = document.getElementById("employeeName");
const employeeEmail = document.getElementById("employeeEmail");
const employeeRole = document.getElementById("employeeRole");
const employeeId = document.getElementById("employeeId");
const logoutButton = document.getElementById("logoutButton");

async function fetchUserData() {
  const userId = localStorage.getItem("userId");

  if (!userId) {
    alert("User not logged in. Redirecting to login page.");
    window.location.href = "index.html"; 
    return;
  }
    const response = await fetch(`https://localhost:7019/api/Customer/${userId}`);

    const res = await response.json();
    console.log(res)

    employeeName.textContent = res.Data.Name;
    employeeEmail.textContent = res.Data.Email;
    employeeRole.textContent = res.Data.Role;
    employeeId.textContent = res.Data.CustomerId;
 
}

function handleLogout() {
  localStorage.removeItem("userId");
  localStorage.removeItem("role");
  localStorage.removeItem("cart");
  alert("Logged out successfully.");
  window.location.href = "index.html"; 
}

logoutButton.addEventListener("click", handleLogout);

fetchUserData();
