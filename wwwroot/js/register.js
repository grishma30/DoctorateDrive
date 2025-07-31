// Register form validation and handling
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('registerForm');
    const fullNameInput = document.getElementById('FullName');
    const mobileInput = document.getElementById('Mobile');
    const emailInput = document.getElementById('Email');

    // Validation patterns
    const patterns = {
        mobile: /^[6-9]\d{9}$/,
        email: /^[^@\s]+@[^@\s]+\.[^@\s]+$/
    };

    // Form submission validation
    form.addEventListener('submit', function (e) {
        const fullName = fullNameInput.value.trim();
        const mobile = mobileInput.value.trim();
        const email = emailInput.value.trim();

        let isValid = true;

        // Validate full name
        if (!fullName) {
            fullNameInput.classList.add('is-invalid');
            isValid = false;
        } else {
            fullNameInput.classList.remove('is-invalid');
        }

        // Validate mobile number (Indian format: 10 digits starting with 6-9)
        if (!patterns.mobile.test(mobile)) {
            mobileInput.classList.add('is-invalid');
            isValid = false;
        } else {
            mobileInput.classList.remove('is-invalid');
        }

        // Validate email
        if (!patterns.email.test(email)) {
            emailInput.classList.add('is-invalid');
            isValid = false;
        } else {
            emailInput.classList.remove('is-invalid');
        }

        // Prevent submission if validation fails
        if (!isValid) {
            e.preventDefault();
            return false;
        }
    });

    // Real-time validation on input
    fullNameInput.addEventListener('input', function () {
        if (this.value.trim()) {
            this.classList.remove('is-invalid');
        }
    });

    mobileInput.addEventListener('input', function () {
        const value = this.value.trim();
        if (patterns.mobile.test(value)) {
            this.classList.remove('is-invalid');
        }

        // Restrict input to numbers only
        this.value = this.value.replace(/[^0-9]/g, '');

        // Limit to 10 digits
        if (this.value.length > 10) {
            this.value = this.value.slice(0, 10);
        }
    });

    emailInput.addEventListener('input', function () {
        const value = this.value.trim();
        if (patterns.email.test(value)) {
            this.classList.remove('is-invalid');
        }
    });

    // Additional mobile number formatting and validation
    mobileInput.addEventListener('keypress', function (e) {
        // Allow only numbers
        if (!/[0-9]/.test(e.key) && !['Backspace', 'Delete', 'Tab', 'Escape', 'Enter'].includes(e.key)) {
            e.preventDefault();
        }
    });

    // Full name validation - allow only letters and spaces
    fullNameInput.addEventListener('keypress', function (e) {
        if (!/[a-zA-Z\s]/.test(e.key) && !['Backspace', 'Delete', 'Tab', 'Escape', 'Enter'].includes(e.key)) {
            e.preventDefault();
        }
    });

    // Clear validation errors on focus
    [fullNameInput, mobileInput, emailInput].forEach(input => {
        input.addEventListener('focus', function () {
            this.classList.remove('is-invalid');
        });
    });
});