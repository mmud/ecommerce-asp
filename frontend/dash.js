if (localStorage.getItem("role")!="admin") {
    window.location.href = "./main.html"; 
  }


const productForm = document.getElementById("productForm");

productForm.addEventListener("submit", async (event) => {
    event.preventDefault(); 

    const formData = new FormData(productForm);

    try {
    const response = await fetch("https://localhost:7019/api/Product", {
        method: "POST",
        body: formData,
    });

    if (!response.ok) {
        throw new Error(`Error: ${response.statusText}`);
    }

    alert("Product added successfully!");

    location.reload();
    } catch (error) {
    console.error("Error submitting form:", error);
    alert("Failed to submit the form. Please try again.");
    }
});


const productTableBody = document.getElementById("productTableBody");

async function fetchProducts() {
    const response = await fetch("https://localhost:7019/api/Product"); 

    const res = await response.json();

    const products=res.Data;

    productTableBody.innerHTML = "";

    products.forEach((product) => {
      const row = document.createElement("tr");

      row.innerHTML = `
        <td><img src="https://localhost:7019/${product.ImagePath}" alt="Product Image" width="50"></td>
        <td>${product.Name}</td>
        <td>${product.Price}</td>
        <td>${product.Quantity}</td>
        <td>${product.Description}</td>
        <td>
          <button class="btn edit" data-id="${product.ProductId}">Edit</button>
          <button class="btn delete" data-id="${product.ProductId}">Delete</button>
        </td>
      `;

      productTableBody.appendChild(row);
    });

    document.querySelectorAll(".btn.edit").forEach((button) =>
      button.addEventListener("click", handleEdit)
    );
    document.querySelectorAll(".btn.delete").forEach((button) =>
      button.addEventListener("click", handleDelete)
    );
}

async function handleEdit(event) {
    const row = event.target.closest("tr");
    const productId = event.target.dataset.id;
      const response = await fetch(`https://localhost:7019/api/Product/${productId}`);
    
  
      const res = await response.json();
        const product=res.Data;
      row.innerHTML = `
        <td>
          ${
            product.ImagePath
              ? `<img src="${product.ImagePath}" alt="Product Image" width="50">`
              : "No Image"
          }
        </td>
        <td><input type="text" id="editName" value="${product.Name}" /></td>
        <td><input type="number" id="editPrice" value="${product.Price}" /></td>
        <td><input type="number" id="editQuantity" value="${product.Quantity}" /></td>
        <td><textarea id="editDescription">${product.Description}</textarea></td>
        <td>
          <button class="btn save" data-id="${productId}">Save</button>
          <button class="btn cancel">Cancel</button>
        </td>
      `;
  
      row.querySelector(".btn.save").addEventListener("click", async (e) => {
        await handleSave(e, productId, row);
      });
  
      row.querySelector(".btn.cancel").addEventListener("click", () => {
        fetchProducts(); 
      });

  }
  
  async function handleSave(event, productId, row) {
    const newName = document.getElementById("editName").value;
    const newPrice = parseFloat(document.getElementById("editPrice").value);
    const newQuantity = parseInt(document.getElementById("editQuantity").value);
    const newDescription = document.getElementById("editDescription").value;
  
    if (!newName || isNaN(newPrice) || isNaN(newQuantity) || !newDescription) {
      alert("All fields are required.");
      return;
    }
    
        const formData = new FormData();
        formData.append("Name", newName);
        formData.append("Description", newDescription);
        formData.append("Price", newPrice);
        formData.append("Quantity", newQuantity);
      const response = await fetch(`https://localhost:7019/api/Product/${productId}`, {
        method: "PUT",
        body: formData
      });
  

  
      alert("Product updated successfully!");
      fetchProducts(); 

  }
  

async function handleDelete(event) {
  const productId = event.target.dataset.id;
  if (confirm("Are you sure you want to delete this product?")) {
    try {
      const response = await fetch(`https://localhost:7019/api/Product/${productId}`, {
        method: "DELETE",
      });

      if (!response.ok) {
        throw new Error("Failed to delete product.");
      }

      alert("Product deleted successfully!");
      fetchProducts(); 
    } catch (error) {
      console.error("Error deleting product:", error);
    }
  }
}

fetchProducts();

async function fetchOrders() {
    try {
        const response = await fetch('https://localhost:7019/api/Order');
        const data = await response.json();

        if (data.Success) {
            const ordersTableBody = document.getElementById('orders-table-body');
            ordersTableBody.innerHTML = ''; 
            data.Data.forEach(order => {
                const row = document.createElement('tr');

                const customerDetails = `
                    Name: ${order.Customer.Name}<br>
                    Address: ${order.Customer.Address}<br>
                `;

                const productDetails = order.Items.map(item => `
                    <div>
                        Name: ${item.ProductName}<br>
                        Price: $${item.PriceAtOrder}<br>
                        Quantity: ${item.Quantity}
                    </div>
                    <hr>
                `).join('');

                row.innerHTML = `
                    <td>${order.OrderId}</td>
                    <td>$${order.TotalAmount}</td>
                    <td>${new Date(order.OrderDate).toLocaleString()}</td>
                    <td>${customerDetails}</td>
                    <td>${productDetails}</td>
                    <td><button class="confirm btn" onclick="deleteOrder(${order.OrderId})">Confirm</button>
                    <button class="delete btn" onclick="deleteOrder(${order.OrderId})">Delete</button></td>
                `;

                ordersTableBody.appendChild(row);
            });
        } else {
            console.error('Failed to retrieve orders:', data.Message);
        }
    } catch (error) {
        console.error('Error fetching orders:', error);
    }
}

async function deleteOrder(orderId) {
    try {
        const response = await fetch(`https://localhost:7019/api/Order/${orderId}`, {
            method: 'DELETE',
        });

        if (response.ok) {
            alert('Order deleted successfully.');
            fetchOrders(); 
        } else {
            alert('Failed to delete the order.');
        }
    } catch (error) {
        console.error('Error deleting order:', error);
    }
}

fetchOrders();