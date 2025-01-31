
function myFunction() {
    var dots = document.getElementById("dots");
    var moreText = document.getElementById("more");
    var btnText = document.getElementById("myBtn");

    if (dots.style.display === "none") {

        dots.style.display = "inline";
        btnText.innerHTML = "Read more";
        moreText.style.display = "none";
    } else {
        dots.style.display = "none";
        btnText.innerHTML = "Read less";
        moreText.style.display = "inline";
    }
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
