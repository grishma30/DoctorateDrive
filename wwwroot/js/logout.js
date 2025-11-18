// logout.js - JWT Logout Handler

function logout() {
    console.log('Logout initiated...');

    // Clear all possible token storage locations
    localStorage.removeItem('token');
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('authToken');
    localStorage.removeItem('bearerToken');
    localStorage.removeItem('access_token');
    localStorage.removeItem('userData');
    localStorage.removeItem('userId');
    localStorage.removeItem('userEmail');

    // Clear sessionStorage
    sessionStorage.clear();

    // Clear cookies (if you're storing token in cookies)
    document.cookie.split(";").forEach(function (c) {
        document.cookie = c.replace(/^ +/, "").replace(/=.*/, "=;expires=" + new Date().toUTCString() + ";path=/");
    });

    console.log('All tokens cleared');

    // Redirect to login page
    window.location.href = '/login.html'; // Change this to your actual login page path
}

// Attach logout to window for global access
window.logout = logout;
