// CHARUSAT Register Portal JavaScript with Debug Logging
console.log('🚀 register.js file loaded successfully');

document.addEventListener('DOMContentLoaded', function () {
    console.log('📋 DOM loaded, initializing register form');

    // Updated field references to match your HTML
    const form = document.getElementById('registerForm');
    const fullNameInput = document.getElementById('FullName');
    const mobileInput = document.getElementById('MobileNumber');
    const emailInput = document.getElementById('EmailId');

    console.log('🔍 Form elements found:');
    console.log('Form:', form);
    console.log('FullName input:', fullNameInput);
    console.log('MobileNumber input:', mobileInput);
    console.log('EmailId input:', emailInput);

    if (!form) {
        console.error('❌ ERROR: registerForm not found! Check if the form has id="registerForm"');
        return;
    }

    if (!fullNameInput) {
        console.error('❌ ERROR: FullName input not found! Check if input has id="FullName"');
    }

    if (!emailInput) {
        console.error('❌ ERROR: EmailId input not found! Check if input has id="EmailId"');
    }

    // Validation patterns
    const patterns = {
        mobile: /^[6-9]\d{9}$/,
        email: /^[^@\s]+@[^@\s]+\.[^@\s]+$/
    };

    console.log('✅ Validation patterns initialized');

    // Form submission - AJAX instead of regular form POST
    form.addEventListener('submit', function (e) {
        console.log('📝 Form submit event triggered');
        e.preventDefault(); // Prevent default form submission
        console.log('🚫 Default form submission prevented');

        const fullName = fullNameInput ? fullNameInput.value.trim() : '';
        const mobile = mobileInput ? mobileInput.value.trim() : '';
        const email = emailInput ? emailInput.value.trim() : '';

        console.log('📊 Form values captured:');
        console.log('Full Name:', fullName);
        console.log('Mobile:', mobile);
        console.log('Email:', email);

        let isValid = true;

        // Clear previous validation states
        clearValidationStates();
        console.log('🧹 Previous validation states cleared');

        // Validate full name
        if (!fullName) {
            console.log('❌ Validation failed: Full name is empty');
            showFieldError(fullNameInput, 'Full name is required');
            isValid = false;
        } else {
            console.log('✅ Full name validation passed');
        }

        // Validate email
        if (!patterns.email.test(email)) {
            console.log('❌ Validation failed: Invalid email format');
            showFieldError(emailInput, 'Please enter a valid email address');
            isValid = false;
        } else {
            console.log('✅ Email validation passed');
        }

        // Validate mobile number (optional but if provided, must be valid)
        if (mobile && !patterns.mobile.test(mobile)) {
            console.log('❌ Validation failed: Invalid mobile format');
            showFieldError(mobileInput, 'Please enter a valid 10-digit mobile number');
            isValid = false;
        } else if (mobile) {
            console.log('✅ Mobile validation passed');
        } else {
            console.log('ℹ️ Mobile field is empty (optional)');
        }

        // Prevent submission if validation fails
        if (!isValid) {
            console.log('❌ Form validation failed, stopping submission');
            return false;
        }

        console.log('✅ All validations passed, proceeding with API call');

        // Show loading state
        const submitBtn = form.querySelector('button[type="submit"]');
        if (submitBtn) {
            console.log('🔄 Setting loading state on submit button');
            submitBtn.disabled = true;
            submitBtn.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Registering & Sending OTP...';
        } else {
            console.log('⚠️ Submit button not found');
        }

        // Prepare API payload
        const apiPayload = {
            fullName: fullName,
            emailId: email,
            mobileNumber: mobile || null
        };

        console.log('📦 API payload prepared:', apiPayload);
        console.log('🌐 Making API call to /api/auth/register');

        // AJAX call to your existing register API endpoint
        fetch('/api/auth/register', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(apiPayload)
        })
            .then(response => {
                console.log('📡 API response received');
                console.log('Response status:', response.status);
                console.log('Response ok:', response.ok);
                console.log('Response headers:', response.headers);

                if (!response.ok) {
                    console.error('❌ API response not ok:', response.status, response.statusText);
                }

                return response.json();
            })
            .then(data => {
                console.log('📋 API response data:', data);

                if (submitBtn) {
                    console.log('🔄 Resetting submit button state');
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = '<i class="fas fa-user-plus me-2"></i>Register & Send OTP';
                }

                if (data && data.success) {
                    console.log('✅ Registration successful!');
                    console.log('📧 Showing success notification');

                    // Show success notification
                    showNotification('Registration successful! OTP sent to your email. Redirecting to login...', 'success');

                    console.log('⏰ Setting redirect timer for 2 seconds');
                    // Redirect to login page after 2 seconds
                    setTimeout(() => {
                        console.log('🔄 Redirecting to login page...');
                        window.location.href = '/Home/Login'; // Change to your actual login page URL
                    }, 2000);
                } else {
                    console.log('❌ Registration failed');
                    console.log('Error message:', data ? data.message : 'No data received');
                    showNotification(data && data.message ? data.message : 'Registration failed', 'error');
                }
            })
            .catch(error => {
                console.error('💥 API call failed with error:', error);
                console.error('Error details:', error.message, error.stack);

                if (submitBtn) {
                    console.log('🔄 Resetting submit button state after error');
                    submitBtn.disabled = false;
                    submitBtn.innerHTML = '<i class="fas fa-user-plus me-2"></i>Register & Send OTP';
                }

                console.log('📧 Showing error notification');
                showNotification('An error occurred during registration. Please check the console for details.', 'error');
            });
    });

    console.log('✅ Form submit event listener attached');

    // Real-time validation on input
    if (fullNameInput) {
        fullNameInput.addEventListener('input', function () {
            if (this.value.trim()) {
                this.classList.remove('is-invalid');
                console.log('✅ Full name input cleared validation error');
            }
        });
        console.log('✅ Full name input validation listener attached');
    }

    if (mobileInput) {
        mobileInput.addEventListener('input', function () {
            const value = this.value.trim();
            if (!value || patterns.mobile.test(value)) {
                this.classList.remove('is-invalid');
            }

            // Restrict input to numbers only
            this.value = this.value.replace(/[^0-9]/g, '');

            // Limit to 10 digits
            if (this.value.length > 10) {
                this.value = this.value.slice(0, 10);
            }
        });

        // Additional mobile number formatting and validation
        mobileInput.addEventListener('keypress', function (e) {
            // Allow only numbers
            if (!/[0-9]/.test(e.key) && !['Backspace', 'Delete', 'Tab', 'Escape', 'Enter'].includes(e.key)) {
                e.preventDefault();
            }
        });
        console.log('✅ Mobile input validation listeners attached');
    }

    if (emailInput) {
        emailInput.addEventListener('input', function () {
            const value = this.value.trim();
            if (patterns.email.test(value)) {
                this.classList.remove('is-invalid');
                console.log('✅ Email input cleared validation error');
            }
        });
        console.log('✅ Email input validation listener attached');
    }

    // Full name validation - allow only letters and spaces
    if (fullNameInput) {
        fullNameInput.addEventListener('keypress', function (e) {
            if (!/[a-zA-Z\s]/.test(e.key) && !['Backspace', 'Delete', 'Tab', 'Escape', 'Enter'].includes(e.key)) {
                e.preventDefault();
            }
        });
    }

    // Clear validation errors on focus
    [fullNameInput, mobileInput, emailInput].filter(input => input).forEach(input => {
        input.addEventListener('focus', function () {
            this.classList.remove('is-invalid');
        });
    });

    console.log('✅ All event listeners attached successfully');

    // Test notification after 3 seconds (remove this after debugging)
    setTimeout(() => {
        console.log('🧪 Testing notification system...');
        showNotification('Test notification - If you see this, notifications are working!', 'success');
    }, 3000);
});

// Utility functions
function showFieldError(field, message) {
    console.log('⚠️ Showing field error:', message);
    if (field) {
        field.classList.add('is-invalid');
        const feedback = field.parentNode.parentNode.querySelector('.invalid-feedback');
        if (feedback) {
            feedback.textContent = message;
            feedback.style.display = 'block';
            console.log('✅ Field error displayed');
        } else {
            console.log('⚠️ Invalid feedback element not found');
        }
    } else {
        console.log('❌ Field element not provided to showFieldError');
    }
}

function clearValidationStates() {
    console.log('🧹 Clearing validation states');
    const invalidFields = document.querySelectorAll('.is-invalid');
    invalidFields.forEach(function (field) {
        field.classList.remove('is-invalid');
    });

    const feedbacks = document.querySelectorAll('.invalid-feedback');
    feedbacks.forEach(function (feedback) {
        feedback.style.display = 'none';
    });
    console.log('✅ Validation states cleared');
}

function showNotification(message, type) {
    console.log('📧 Creating notification:', type, message);

    // Create notification element
    const notification = document.createElement('div');
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
    console.log('✅ Notification added to DOM');

    // Auto remove after 8 seconds (longer for redirect message)
    setTimeout(function () {
        if (notification.parentNode) {
            notification.remove();
            console.log('🗑️ Notification auto-removed');
        }
    }, 8000);
}

console.log('🎯 register.js initialization complete');

// Test if JavaScript is working
document.addEventListener('DOMContentLoaded', function () {
    console.log('🧪 register.js loaded - form should be intercepted');

    // Test after 2 seconds
    setTimeout(() => {
        const form = document.getElementById('registerForm');
        if (form) {
            console.log('✅ Form found:', form);
        } else {
            console.error('❌ Form NOT found');
        }
    }, 2000);
});
