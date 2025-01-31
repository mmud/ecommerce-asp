
const menuBar = document.getElementById('menuBar');
const menuList = document.getElementById('menuList');

menuBar.addEventListener('click', () => {
    menuList.classList.toggle('active');
});
    

let slider = document.querySelector('.slider .list');
let items = document.querySelectorAll('.slider .list .item');
let next = document.getElementById('next');
let prev = document.getElementById('prev');
let dots = document.querySelectorAll('.slider .dots li');

let lengthItems = items.length - 1;
let active = 0;
next.onclick = function(){
    active = active + 1 <= lengthItems ? active + 1 : 0;
    reloadSlider();
}
prev.onclick = function(){
    active = active - 1 >= 0 ? active - 1 : lengthItems;
    reloadSlider();
}
let refreshInterval = setInterval(()=> {next.click()}, 3000);
function reloadSlider(){
    const imageWidth = items[active].offsetWidth; 
    slider.style.left = -imageWidth * active + 'px';
    
    let last_active_dot = document.querySelector('.slider .dots li.active');
    last_active_dot.classList.remove('active');
    dots[active].classList.add('active');

    clearInterval(refreshInterval);
    refreshInterval = setInterval(() => { next.click() }, 3000);
}

window.onresize = function() {
    reloadSlider();
};

dots.forEach((li, key) => {
    li.addEventListener('click', ()=>{
         active = key;
         reloadSlider();
    })
})
window.onresize = function(event) {
    reloadSlider();
};





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

const appendMessage = (message, className) => {
    const chatLi = document.createElement("li");
    chatLi.classList.add("chat", className);
    chatLi.innerHTML = className === "outgoing" 
        ? `<p>${message}</p>` 
        : `<span class="fa-solid fa-robot" style="color: #ffffff;"></span><p>${message}</p>`;
    chatbox.appendChild(chatLi);
    chatbox.scrollTo(0, chatbox.scrollHeight);
};

chatbotToggler.addEventListener("click", () => document.body.classList.toggle("show-chatbot"));

document.addEventListener("DOMContentLoaded", async function () {
    const productsList = document.getElementById("productsList");
    const centeredMessage = document.querySelector(".centered-message");
    const welcomeMessage = document.querySelector(".welcome-message");
    const API_URL = "https://localhost:7019/api/Product"; 

    async function fetchProducts() {
            const response = await fetch(API_URL);
            const data = await response.json();
            if (data.Success && Array.isArray(data.Data)) {
                return data.Data;
            } else {
                console.error("Invalid API response format:", data);
                return [];
            }
        
    }

    async function initializePage() {
        const products = await fetchProducts();
        displayProducts(products);
    }

    function displayProducts(filteredProducts) {
        productsList.innerHTML = "";

        if (filteredProducts.length === 0) {
            centeredMessage.style.display = "flex";
            welcomeMessage.style.display = "none"; 
        } else {
            centeredMessage.style.display = "none";
            welcomeMessage.style.display = "none";

            filteredProducts.forEach(product => {
                const productDiv = document.createElement("div");
                productDiv.classList.add("product");

                productDiv.innerHTML = `
                    <a href="Product.html?productId=${product.ProductId}">
                        <img src="https://localhost:7019/${product.ImagePath}" alt="${product.Name}">
                    </a>
                    <h3>${product.Name}</h3>
                    <p>Price: $${product.Price.toFixed(2)}</p>
                `;
                productsList.appendChild(productDiv);
            });
        }
    }

    initializePage();
});
