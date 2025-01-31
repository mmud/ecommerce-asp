const dashboardLink = document.getElementById("dashboardLink");
const signInLink = document.getElementById("signInLink");
const profileLink = document.getElementById("profileLink");

const userId = localStorage.getItem("userId");
const userRole = localStorage.getItem("role");

if (userId) {
  signInLink.style.display = "none";
  profileLink.style.display = "block";
  
  if (userRole != "admin") {
    dashboardLink.style.display = "none";
  }
} else {
  signInLink.style.display = "block";
  profileLink.style.display = "none";
  dashboardLink.style.display = "none";
}
