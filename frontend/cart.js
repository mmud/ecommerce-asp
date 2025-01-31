if (!localStorage.getItem("userId")) {
  window.location.href = "./login.html"; 
}
document.addEventListener("DOMContentLoaded", function () {
  const cartTable = document.getElementById("cart-table");
  const cartSubtotal = document.getElementById("cart-subtotal");
  const cartTotal = document.getElementById("cart-total");

  function getCartItems() {
      return JSON.parse(localStorage.getItem("cart")) || [];
  }

  function saveCartItems(cartItems) {
      localStorage.setItem("cart", JSON.stringify(cartItems));
  }

  function updateCartTotal() {
      const cartItems = getCartItems();
      let subtotal = 0;

      cartItems.forEach(item => {
          subtotal += item.price * item.quantity;
      });

      cartSubtotal.textContent = `$${subtotal.toFixed(2)}`;
      cartTotal.textContent = `$${subtotal.toFixed(2)}`;
  }

  function removeItem(index) {
      const cartItems = getCartItems();
      cartItems.splice(index, 1); 
      saveCartItems(cartItems);  
      renderCart();              
  }

  function renderCart() {
      const cartItems = getCartItems();

      const rows = Array.from(cartTable.querySelectorAll("tr"));
      rows.slice(1).forEach(row => row.remove());

      if (cartItems.length === 0) {
          const emptyRow = document.createElement("tr");
          emptyRow.innerHTML = `<td colspan="5">Your cart is empty!</td>`;
          cartTable.appendChild(emptyRow);
          updateCartTotal();
          return;
      }

      cartItems.forEach((item, index) => {
          const row = document.createElement("tr");
          row.innerHTML = `
              <td>${item.name}</td>
              <td>$${item.price.toFixed(2)}</td>
              <td>
                  <input type="number" value="${item.quantity}" min="1" class="quantity">
              </td>
              <td class="subtotal">$${(item.price * item.quantity).toFixed(2)}</td>
              <td><button class="remove-button">Remove</button></td>
          `;

          const quantityInput = row.querySelector(".quantity");
          quantityInput.addEventListener("change", function () {
              const newQuantity = parseInt(quantityInput.value);
              cartItems[index].quantity = newQuantity;
              saveCartItems(cartItems); 
              renderCart();            
          });

          const removeButton = row.querySelector(".remove-button");
          removeButton.addEventListener("click", function () {
              removeItem(index); 
          });

          cartTable.appendChild(row);
      });

      updateCartTotal(); 
  }

  renderCart();
});



document.addEventListener("DOMContentLoaded", function () {
  const cartTable = document.getElementById("cart-table");
  const cartSubtotal = document.getElementById("cart-subtotal");
  const cartTotal = document.getElementById("cart-total");
  const placeOrderButton = document.getElementById("place-order");

  function getCartItems() {
      return JSON.parse(localStorage.getItem("cart")) || [];
  }

  async function placeOrder() {
      const customerId = localStorage.getItem("userId");
      const cartItems = getCartItems();

      if (!customerId) {
          alert("User ID is missing. Please log in again.");
          return;
      }

      if (cartItems.length === 0) {
          alert("Your cart is empty. Add items to place an order.");
          return;
      }

      const orderData = {
          CustomerId: parseInt(customerId),
          Items: cartItems.map(item => ({
              ProductId: item.id, 
              Quantity: item.quantity,
          })),
      };

      try {
          const response = await fetch("https://localhost:7019/api/Order", {
              method: "POST",
              headers: {
                  "Content-Type": "application/json",
              },
              body: JSON.stringify(orderData), 
          });

          if (response.ok) {
              const result = await response.json();
              alert("Order placed successfully!");
              localStorage.removeItem("cart"); 
              window.location.reload(); 
          } else {
              const error = await response.json();
              alert(`Error placing order: ${error.message || "Unknown error"}`);
          }
      } catch (err) {
          console.error("Order submission failed:", err);
          alert("Failed to place the order. Please try again.");
      }
  }

  placeOrderButton.addEventListener("click", placeOrder);
});
