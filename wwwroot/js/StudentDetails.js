﻿// M.Tech CGPA Validation
document.getElementById('cgpaInput').addEventListener('blur', function () {
    const cgpa = parseFloat(this.value);
    if (!isNaN(cgpa) && cgpa < 5) {
        alert("You are not eligible.");

        // Disable all remaining form fields
        const formElements = document.querySelectorAll('#registrationForm input, #registrationForm select, #registrationForm button, #registrationForm textarea');
        formElements.forEach(el => {
            if (el.id !== 'cgpaInput') {
                el.disabled = true;
            }
        });
    }
});



document.getElementById('registrationForm').addEventListener('submit', function (e) {
    e.preventDefault();
    // Add your validation or submission logic here
    alert('Form submitted!');
});

// Graduate
document.getElementById('graduateSelect').addEventListener('change', function () {
    document.getElementById('graduateUpload').style.display = this.value ? 'block' : 'none';
    document.getElementById('graduateCertificate').required = !!this.value;
});

// Post Graduate
document.getElementById('postGraduateSelect').addEventListener('change', function () {
    document.getElementById('postGraduateUpload').style.display = this.value ? 'block' : 'none';
    document.getElementById('postGraduateCertificate').required = !!this.value;
});

// GATE dropdown logic
const gateSelect = document.getElementById('gateSelect');
const gateUpload = document.getElementById('gateUpload');
const gateCertificate = document.getElementById('gateCertificate');

gateSelect.addEventListener('change', function () {
    if (this.value === 'yes') {
        gateUpload.style.display = 'block';
        gateCertificate.required = true;
    } else {
        gateUpload.style.display = 'none';
        gateCertificate.required = false;
        gateCertificate.value = '';
    }
});
// OTP Verification Code
let generatedOtp = null;

document.getElementById('sendOtpBtn').addEventListener('click', function () {
    const mobile = document.getElementById('mobileNumber').value;
    if (!/^\d{10}$/.test(mobile)) {
        alert('Please enter a valid 10-digit mobile number.');
        return;
    }

    // Generate a 6-digit OTP
    generatedOtp = Math.floor(100000 + Math.random() * 900000).toString();

    // Simulate sending OTP
    alert(`Your OTP is: ${generatedOtp}`); // Simulate SMS API

    document.getElementById('otpSection').style.display = 'block';
    document.getElementById('otpStatus').textContent = '';
});

document.getElementById('verifyOtpBtn').addEventListener('click', function () {
    const enteredOtp = document.getElementById('otpInput').value;
    const status = document.getElementById('otpStatus');

    if (enteredOtp === generatedOtp) {
        status.textContent = '✅ OTP verified successfully!';
        status.style.color = 'lightgreen';
    } else {
        status.textContent = '❌ Incorrect OTP. Please try again.';
        status.style.color = 'red';
    }
});
// Preferences dropdown logic
const preferenceSelects = document.querySelectorAll('.preference-select');

function updateDropdownOptions() {
    const selectedValues = Array.from(preferenceSelects).map(select => select.value);

    preferenceSelects.forEach(select => {
        const currentValue = select.value;
        Array.from(select.options).forEach(option => {
            if (option.value === "") return; // skip the default option
            if (selectedValues.includes(option.value) && option.value !== currentValue) {
                option.disabled = true;
            } else {
                option.disabled = false;
            }
        });
    });
}

// Attach event listener to all preference selects
preferenceSelects.forEach(select => {
    select.addEventListener('change', updateDropdownOptions);
});
// Alert if equivalent percentage < 60%
document.getElementById('equivalentPercentage').addEventListener('blur', function () {
    const percentage = parseFloat(this.value);
    if (!isNaN(percentage) && percentage < 60) {
        alert("⚠️ You are not eligible. If you register, the fees will not be refundable.");
    }
});

document.addEventListener('DOMContentLoaded', function () {
    const departmentDropdown = document.getElementById('departmentDropdown');
    const selectedDepartment = document.getElementById('selectedDepartment');
    const selectedSubSubject = document.getElementById('selectedSubSubject');

    // Department & sub-subject mapping
    const data = {
        "Computer Science": ["AI", "ML", "Data Science", "Networking"],
        "Electronics": ["Communication", "Signal Processing", "VLSI"],
        "Mechanical": ["Thermodynamics", "Fluid Mechanics", "Design"],
        "Civil": ["Structural", "Transportation", "Environmental"]
    };

    // Populate dropdown
    for (const dept in data) {
        data[dept].forEach(sub => {
            const option = document.createElement('option');
            option.value = `${dept} > ${sub}`;
            option.textContent = `${dept} - ${sub}`;
            departmentDropdown.appendChild(option);
        });
    }

    // Update displayed selected values
    departmentDropdown.addEventListener('change', function () {
        const value = this.value;
        if (value) {
            const parts = value.split(' > ');
            selectedDepartment.textContent = parts[0];
            selectedSubSubject.textContent = parts[1];
        } else {
            selectedDepartment.textContent = "None";
            selectedSubSubject.textContent = "None";
        }
    });
});

document.addEventListener('DOMContentLoaded', () => {
    const dropdown = document.querySelector('.dropdown');
    const selectedText = document.getElementById('selectedValueText');
    const selectedDeptInput = document.getElementById('selectedDepartment');
    const selectedSubInput = document.getElementById('selectedSubSubject');
    const dropdownToggle = dropdown.querySelector('.dropdown-toggle');

    dropdown.querySelectorAll('.sub-item').forEach(item => {
        item.addEventListener('click', e => {
            e.preventDefault();
            const dept = item.getAttribute('data-dept');
            const sub = item.getAttribute('data-sub');

            selectedDeptInput.value = dept;
            selectedSubInput.value = sub;

            selectedText.textContent = `${dept} > ${sub}`;
            dropdownToggle.textContent = `${dept} > ${sub}`;

            // Close the dropdown manually
            const bsDropdown = bootstrap.Dropdown.getInstance(dropdownToggle);
            if (bsDropdown) {
                bsDropdown.hide();
            }
        });
    });
});


dropdownMenu.querySelectorAll('[data-os]').forEach(item => {
    item.addEventListener('click', (e) => {
        e.preventDefault();
        const category = item.getAttribute('data-category');
        const os = item.getAttribute('data-os');
        // Update button text
        dropdownToggle.textContent = `${category}: ${os}`;
        // Update display
        selectedOSDisplay.textContent = `Selected: ${category} - ${os}`;
        // Store values in hidden inputs for form submission
        document.getElementById('osCategory').value = category;
        document.getElementById('osValue').value = os;
        // Hide dropdown
        dropdownMenu.classList.remove('show');
        // Trigger change event if needed
        const event = new Event('change');
        dropdownToggle.dispatchEvent(event);
    });
});
