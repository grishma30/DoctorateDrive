//document.addEventListener('DOMContentLoaded', function () {
//    // Photo upload functionality
//    const photoInput = document.getElementById('photoInput');
//    const studentPhoto = document.getElementById('studentPhoto');
//    const photoPlaceholder = document.getElementById('photoPlaceholder');

//    photoInput.addEventListener('change', function (e) {
//        const file = e.target.files[0];
//        if (file) {
//            const reader = new FileReader();
//            reader.onload = function (e) {
//                studentPhoto.src = e.target.result;
//                studentPhoto.classList.remove('d-none');
//                photoPlaceholder.style.display = 'none';
//            }
//            reader.readAsDataURL(file);
//        }
//    });

//    // Clear photo functionality
//    document.querySelectorAll('.btn-warning').forEach(button => {
//        if (button.textContent.trim() === 'Clear') {
//            button.addEventListener('click', function () {
//                if (button.classList.contains('btn-sm')) {
//                    // Clear photo
//                    studentPhoto.src = '';
//                    studentPhoto.classList.add('d-none');
//                    photoPlaceholder.style.display = 'flex';
//                    photoInput.value = '';
//                } else {
//                    // Clear form
//                    const form = document.querySelector('form');
//                    if (form) {
//                        const editableFields = form.querySelectorAll('input:not(.readonly-field), textarea, select');
//                        editableFields.forEach(field => {
//                            if (field.type === 'radio' || field.type === 'checkbox') {
//                                field.checked = false;
//                            } else if (field.tagName === 'SELECT') {
//                                field.selectedIndex = 0;
//                            } else {
//                                field.value = '';
//                            }
//                        });

//                        // Reset specific readonly fields if needed
//                        const readonlyTexts = form.querySelectorAll('input.readonly-field');
//                        readonlyTexts.forEach(input => {
//                            if (input.id === 'nationalityField') {
//                                input.value = 'INDIAN';
//                            } else if (input.id === 'motherTongueField') {
//                                input.value = 'GUJARATI';
//                            }
//                        });

//                        // Optionally reset default radio values
//                        const defaultRadios = {
//                            gender: 'female',
//                            nationality: 'indian',
//                            motherTongue: 'gujarati',
//                            enrollmentType: 'partTime',
//                            freeshipCard: 'no'
//                        };

//                        for (const [name, value] of Object.entries(defaultRadios)) {
//                            const defaultRadio = form.querySelector(`input[name="${name}"][value="${value}"]`);
//                            if (defaultRadio) {
//                                defaultRadio.checked = true;
//                            }
//                        }
//                    }
//                }
//            });
//        }
//    });

//    // Save button functionality
//    const saveButton = document.querySelector('.btn.btn-primary');
//    saveButton.addEventListener('click', function () {
//        alert('Profile information saved successfully.');
//        // You can replace the alert with actual form submission logic or AJAX call
//    });
//});

document.addEventListener('DOMContentLoaded', function () {
    // Your full code goes here



// Auto-update display name based on name format selection
const nameFormatSelect = document.querySelector('select[class="form-select"]');
const displayNameInput = document.querySelector('input[value="GAJJAR SRUSHTI RAKESHKUMAR"]');
const firstNameInput = document.querySelector('input[value="GAJJAR"]');
const middleNameInput = document.querySelector('input[value="SRUSHTI"]');
const lastNameInput = document.querySelector('input[value="RAKESHKUMAR"]');

function updateDisplayName() {
    const format = nameFormatSelect.value;
    const first = firstNameInput.value.trim();
    const middle = middleNameInput.value.trim();
    const last = lastNameInput.value.trim();

    let displayName = '';
    switch (format) {
        case 'LFM':
            displayName = `${last} ${first} ${middle}`;
            break;
        case 'FLM':
            displayName = `${first} ${last} ${middle}`;
            break;
        case 'FML':
            displayName = `${first} ${middle} ${last}`;
            break;
        default:
            displayName = `${last} ${first} ${middle}`;
    }

    displayNameInput.value = displayName.trim();
}

// Add event listeners
nameFormatSelect.addEventListener('change', updateDisplayName);
firstNameInput.addEventListener('input', updateDisplayName);
middleNameInput.addEventListener('input', updateDisplayName);
lastNameInput.addEventListener('input', updateDisplayName);

// Photo upload functionality
const uploadBtn = document.querySelector('.btn-custom');
const clearPhotoBtn = document.getElementById("clear-photo");
const photoImg = document.querySelector('.student-photo');
const photoInput = document.getElementById("studentPhoto");
const cancelBtn = document.getElementById("cancel-button");
const saveBtn = document.getElementById("save-button");
const form = document.querySelector("form");
const toast = new bootstrap.Toast(document.getElementById('toastMessage'));

uploadBtn.addEventListener('click', function () {
    const input = document.createElement('input');
    input.type = 'file';
    input.accept = 'image/*';
    input.onchange = function (event) {
        const file = event.target.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function (e) {
                photoImg.src = e.target.result;
            }
            reader.readAsDataURL(file);
        }
    }
    input.click();
});

let initialState = {
    photoSrc: photoImg.src,
    formData: new FormData(form)
};

// Clear photo functionality
clearPhotoBtn.addEventListener("click", function () {
    photoImg.src = "https://via.placeholder.com/150x180/cccccc/ffffff?text=Student+Photo";
    photoInput.value = "";
});

// Cancel changes → reset form and photo to original
cancelBtn.addEventListener("click", function () {
    // Reset form values
    for (const [name, value] of initialState.formData.entries()) {
        const field = form.elements[name];
        if (!field) continue;

        if (field.type === "radio" || field.type === "checkbox") {
            // Reset checkbox/radio
            field.checked = value === field.value;
        } else {
            field.value = value;
        }
    }

    // Reset photo
    photoImg.src = initialState.photoSrc;
    photoInput.value = "";
    toast._element.querySelector(".toast-body").innerText = "Changes discarded.";
    toast.show();
});

saveBtn.addEventListener("click", function () {
    // Update initialState with current values
    initialState = {
        photoSrc: photoImg.src,
        formData: new FormData(form)
    };

    toast._element.querySelector(".toast-body").innerText = "Profile saved successfully!";
    toast.show();
});

    function editDocument(button) {
        uploadDocument(button);
    }

    function deleteDocument(button) {
        const parent = button.parentElement;
        parent.innerHTML = `
        <button type="button" class="btn-action btn-upload" onclick="uploadDocument(this)">
            <i class="fas fa-plus"></i> Add
        </button>
    `;
        parent.removeAttribute('data-filename');
    }

    function addNewRow() {
        const tableBody = document.getElementById('academicTableBody');
        const newRow = document.createElement('tr');

        newRow.innerHTML = `
            <td>
                <select class="form-select">
                    <option value="">Select...</option>
                    <option value="SSC">SSC</option>
                    <option value="HSC">HSC</option>
                    <option value="BE/BTECH">BE / BTECH</option>
                    <option value="MTECH">MTECH</option>
                    <option value="DIPLOMA">DIPLOMA</option>
                </select>
            </td>
            <td><input type="text" class="form-control"></td>
            <td><input type="text" class="form-control"></td>
            <td><input type="text" class="form-control"></td>
            <td><input type="text" class="form-control"></td>
            <td>
                <select class="form-select">
                    <option value="">Select...</option>
                    <option value="JANUARY">JANUARY</option>
                    <option value="FEBRUARY">FEBRUARY</option>
                    <option value="MARCH">MARCH</option>
                    <option value="APRIL">APRIL</option>
                    <option value="MAY">MAY</option>
                    <option value="JUNE">JUNE</option>
                    <option value="JULY">JULY</option>
                    <option value="AUGUST">AUGUST</option>
                    <option value="SEPTEMBER">SEPTEMBER</option>
                    <option value="OCTOBER">OCTOBER</option>
                    <option value="NOVEMBER">NOVEMBER</option>
                    <option value="DECEMBER">DECEMBER</option>
                </select>
            </td>
            <td><input type="text" class="form-control"></td>
            <td><input type="text" class="form-control"></td>
            <td class="action-cell">
                <button type="button" class="btn-action btn-upload" onclick="uploadDocument(this)">
                    <i class="fas fa-plus"></i> Add
                </button>
            </td>
        `;

        tableBody.appendChild(newRow);
    }

    //function uploadDocument(button) {
    //    // Create file input
    //    const fileInput = document.createElement('input');
    //    fileInput.type = 'file';
    //    fileInput.accept = '.pdf,.doc,.docx,.jpg,.jpeg,.png';

    //    fileInput.onchange = function (e) {
    //        if (e.target.files.length > 0) {
    //            const fileName = e.target.files[0].name;
    //            // Replace the Add button with Edit and Delete buttons
    //            button.parentElement.innerHTML = `
    //                <button type="button" class="btn-action btn-edit" onclick="editDocument(this)" title="Edit">
    //                    <i class="fas fa-edit"></i>
    //                </button>
    //                <button type="button" class="btn-action btn-delete" onclick="deleteDocument(this)" title="Delete">
    //                    <i class="fas fa-trash"></i>
    //                </button>
    //            `;

    //            // Store filename as data attribute
    //            button.parentElement.setAttribute('data-filename', fileName);
    //        }
    //    };

    //    fileInput.click();
    //}



    function editDocument(button) {
        // Open file dialog again
        const fileInput = document.createElement('input');
        fileInput.type = 'file';
        fileInput.accept = '.pdf,.doc,.docx,.jpg,.jpeg,.png';

        fileInput.onchange = function (e) {
            if (e.target.files.length > 0) {
                const fileName = e.target.files[0].name;
                button.parentElement.setAttribute('data-filename', fileName);
            }
        };

        fileInput.click();
    }

    function deleteDocument(button) {
        if (confirm('Are you sure you want to delete this document?')) {
            // Replace Edit and Delete buttons with Add button
            button.parentElement.innerHTML = `
                <button type="button" class="btn-action btn-upload" onclick="uploadDocument(this)">
                    <i class="fas fa-plus"></i> Add
                </button>
            `;
            button.parentElement.removeAttribute('data-filename');
        }
    }

   
        const tableBody = document.getElementById('academicTableBody');



});

function uploadDocument(button) {
    const td = button.closest('td');

    // Create file input
    const fileInput = document.createElement('input');
    fileInput.type = 'file';
    fileInput.accept = '.pdf,.doc,.docx,.jpg,.jpeg,.png';

    fileInput.onchange = function (e) {
        if (e.target.files.length > 0) {
            const fileName = e.target.files[0].name;

            // Replace the Add button with Edit and Delete
            td.innerHTML = `
                <button type="button" class="btn-action btn-edit" onclick="editDocument(this)">
                    <i class="fas fa-edit"></i> Edit
                </button>
                <button type="button" class="btn-action btn-delete" onclick="deleteDocument(this)">
                    <i class="fas fa-trash"></i> Delete
                </button>
            `;
            td.setAttribute('data-filename', fileName);

            // ✅ Only now add a new row
            addNewRow();
        }
    };

    fileInput.click();
}
