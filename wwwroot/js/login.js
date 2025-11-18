// CHARUSAT Login Portal JavaScript
document.addEventListener('DOMContentLoaded', function () {
    // Register button click handler
    var registerBtn = document.getElementById('registerBtn');
    if (registerBtn) {
        registerBtn.addEventListener('click', function () {
            window.location.href = '/Home/Index'; // or wherever you want to redirect after login

        });
    }

    // Get OTP button click handler
    var getOtpBtn = document.getElementById('getOtpBtn');
    if (getOtpBtn) {
        getOtpBtn.addEventListener('click', function () {
            var emailMobile = document.getElementById('EmailMobile');
            if (emailMobile && emailMobile.value.trim() !== '') {
                getOtpBtn.disabled = true;
                getOtpBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Sending OTP...';

                sendOtpRequest(emailMobile.value);

                // We handle button reset inside sendOtpRequest's promise handlers
            } else {
                showNotification('Please enter email or mobile number first', 'error');
            }
        });


    }

    // Form validation
    var loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            var isValid = true;
            var emailMobile = document.getElementById('EmailMobile');
            var otp = document.getElementById('Otp');

            // Clear previous validation states
            clearValidationStates();

            // Validate email/mobile
            if (!emailMobile || emailMobile.value.trim() === '') {
                showFieldError(emailMobile, 'Email or mobile number is required');
                isValid = false;
            } else if (!isValidEmailOrMobile(emailMobile.value.trim())) {
                showFieldError(emailMobile, 'Please enter a valid email or mobile number');
                isValid = false;
            }

            // Validate OTP
            if (!otp || otp.value.trim() === '') {
                showFieldError(otp, 'OTP is required');
                isValid = false;
            } else if (otp.value.trim().length < 4) {
                showFieldError(otp, 'Please enter a valid OTP');
                isValid = false;
            }

            if (!isValid) {
                e.preventDefault();
                return false;
            }

            // Show loading state on submit button
            var submitBtn = loginForm.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Logging in...';
            }
        });
    }
});

// Utility functions
function isValidEmailOrMobile(value) {
    // Email regex
    var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    // Mobile regex (Indian format)
    var mobileRegex = /^[6-9]\d{9}$/;

    return emailRegex.test(value) || mobileRegex.test(value);
}

function showFieldError(field, message) {
    if (field) {
        field.classList.add('is-invalid');
        var feedback = field.parentNode.parentNode.querySelector('.invalid-feedback');
        if (feedback) {
            feedback.textContent = message;
            feedback.style.display = 'block';
        }
    }
}

function clearValidationStates() {
    var invalidFields = document.querySelectorAll('.is-invalid');
    invalidFields.forEach(function (field) {
        field.classList.remove('is-invalid');
    });

    var feedbacks = document.querySelectorAll('.invalid-feedback');
    feedbacks.forEach(function (feedback) {
        feedback.style.display = 'none';
    });
}

function showNotification(message, type) {
    // Create notification element
    var notification = document.createElement('div');
    notification.className = 'alert alert-' + (type === 'success' ? 'success' : 'danger') + ' alert-dismissible fade show position-fixed';
    notification.style.top = '20px';
    notification.style.right = '20px';
    notification.style.zIndex = '9999';
    notification.style.minWidth = '300px';

    notification.innerHTML = `
        <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-triangle'} me-2"></i>
        ${message}
        <button type="button" class="btn-close" onclick="this.parentElement.remove()"></button>
    `;

    document.body.appendChild(notification);

    // Auto remove after 5 seconds
    setTimeout(function () {
        if (notification.parentNode) {
            notification.remove();
        }
    }, 5000);
}

// Function to send OTP request (to be implemented with actual API call)
function sendOtpRequest(emailOrMobile) {
    fetch('/Account/SendOtp', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ emailOrMobile: emailOrMobile })
    })
        .then(response => response.json())
        .then(data => {
            getOtpBtn.disabled = false;
            getOtpBtn.innerHTML = '<i class="fas fa-mobile-alt me-2"></i>Get a new OTP';

            if (data.success) {
                showNotification('OTP sent successfully!', 'success');
            } else {
                showNotification(data.message || 'Failed to send OTP', 'error');
            }
        })
        .catch(error => {
            getOtpBtn.disabled = false;
            getOtpBtn.innerHTML = '<i class="fas fa-mobile-alt me-2"></i>Get a new OTP';

            console.error('Error:', error);
            showNotification('An error occurred while sending OTP', 'error');
        });
}

function logout() {
    // Remove JWT token from localStorage
    localStorage.removeItem('token');
    localStorage.removeItem('jwtToken');
    localStorage.removeItem('authToken');

    // Remove from sessionStorage if used
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('jwtToken');

    // Clear any other user data
    localStorage.removeItem('userData');
    localStorage.removeItem('userId');

    // Redirect to login page
    window.location.href = '/login.html';
}
