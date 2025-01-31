if (!localStorage.getItem("userId")) {
  window.location.href = "./login.html"; 
}


const chatbotToggler = document.querySelector(".chatbot-toggler");
const chatbox = document.querySelector(".chatbox");
const faqOptions = document.querySelector(".faq-options");

const faqResponses = {
    1: "We provide web development, app development, and digital marketing services.",
    2: "You can contact support via email at support@example.com or call us at +20xxxxxxxxxx.",
    3: "Our working hours are Monday to Friday, 9 AM to 6 PM.",
    4: "check about us page to know.",
};

faqOptions.addEventListener("click", (event) => {
    const selectedOption = event.target.getAttribute("data-option");
    if (selectedOption) {
        const userMessage = event.target.textContent;
        const botResponse = faqResponses[selectedOption];

        appendMessage(userMessage, "outgoing");

        setTimeout(() => {
            appendMessage(botResponse, "incoming");
        }, 600); 
    }
});
let productdata={};

document.addEventListener("DOMContentLoaded", async function () {
  const API_URL = "https://localhost:7019/api/Product"; 
  function getQueryParam(param) {
      const urlParams = new URLSearchParams(window.location.search);
      return urlParams.get(param);
  }

  async function fetchProductById(productId) {
          const response = await fetch(`${API_URL}/${productId}`);
         
          const data = await response.json();
          if (data.Success && data.Data) {
              return data.Data;
          } else {
              console.error("Invalid API response format:", data);
              return null;
          }
     
  }
  function displayProduct(product) {
      if (!product) {
          console.error("No product data available");
          return;
      }

      const fullImg = document.getElementById("imagebox");
      fullImg.src = "https://localhost:7019"+product.ImagePath ;

      document.getElementById("product-name").textContent = product.Name;
      document.getElementById("product-description").textContent = product.Description;
      document.getElementById("product-price").textContent = `$${product.Price.toFixed(2)}`;

      const imageGallery = document.getElementById("image-gallery");
      const imagePaths = [
          "https://localhost:7019"+product.ImagePath,
          "https://localhost:7019"+product.AdditionalImage1,
          "https://localhost:7019"+product.AdditionalImage2,
          "https://localhost:7019"+product.AdditionalImage3,
      ].filter(Boolean); 
      imageGallery.innerHTML = ""; 
      imagePaths.forEach((path, index) => {
          const img = document.createElement("img");
          img.src = path;
          img.alt = `Product Image ${index + 1}`;
          img.classList.add("thumbnail");
          img.addEventListener("click", () => clickimg(img));
          imageGallery.appendChild(img);
      });
  }
  async function initializePage() {
      const productId = getQueryParam("productId");
      if (!productId) {
          console.error("No productId in query string");
          return;
      }

      const product = await fetchProductById(productId);
      productdata=product;
      displayProduct(product);
  }

  initializePage();
});

let currentImageIndex = 0;
const images = [];

function clickimg(smallImg) {
  const fullImg = document.getElementById("imagebox");
  fullImg.src = smallImg.src;
  currentImageIndex = images.indexOf(smallImg.src); 
}

function autoChangeImage() {
  if (images.length > 0) {
      currentImageIndex = (currentImageIndex + 1) % images.length;
      const fullImg = document.getElementById("imagebox");
      fullImg.src = images[currentImageIndex];
  }
}

setInterval(autoChangeImage, 5000);

document.addEventListener("DOMContentLoaded", function () {
  function getCart() {
      return JSON.parse(localStorage.getItem("cart")) || [];
  }

  function saveCart(cart) {
      localStorage.setItem("cart", JSON.stringify(cart));
  }

  function addToCart(product) {
      const cart = getCart();
      const existingProduct = cart.find(item => item.id === product.id);

      if (existingProduct) {
          existingProduct.quantity += product.quantity;
      } else {
          cart.push(product);
      }

      saveCart(cart);
      alert("Product added to cart!");
  }

  document.getElementById("add-to-cart-btn").addEventListener("click", function () {
      const quantity = parseInt(document.getElementById("product-quantity").value) || 1;
      
      if (quantity < 1) {
        alert("Please enter a valid quantity.");
        return;
      }
      
      const product = {
        id: productdata.ProductId,
        name: productdata.Name,
        price: productdata.Price,
        quantity: quantity,
      };
      addToCart(product);
  });
});
