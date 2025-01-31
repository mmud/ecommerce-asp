function showLogin() {
    document.getElementById('login-form').classList.remove('hidden');
    document.getElementById('register-form').classList.add('hidden');
    document.getElementById('login-btn').classList.add('active');
    document.getElementById('register-btn').classList.remove('active');
}

function showRegister() {
    document.getElementById('register-form').classList.remove('hidden');
    document.getElementById('login-form').classList.add('hidden');
    document.getElementById('register-btn').classList.add('active');
    document.getElementById('login-btn').classList.remove('active');
}

function login(event) {
    event.preventDefault();
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;

    alert(`Login successful for ${email}`);
}


