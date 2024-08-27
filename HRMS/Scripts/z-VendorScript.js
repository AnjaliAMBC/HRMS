
$(document).on('change', '#addvendor', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/Vendor/AddVendor';
    } else if (selectedValue === "importuser") {
        window.location.href = '/Vendor/ImportVendor';
    }
});


function validateForm() {
    let isValid = true;

    // Validate Vendor ID
    const vendorID = $('#VendorID');
    if (vendorID.val() === "") {
        vendorID.addClass('is-invalid');
        isValid = false;
    } else {
        vendorID.removeClass('is-invalid');
    }

    // Validate Vendor Name
    const vendorName = $('#VendorName');
    if (vendorName.val() === "") {
        vendorName.addClass('is-invalid');
        isValid = false;
    } else {
        vendorName.removeClass('is-invalid');
    }

    // Validate Vendor Email
    const vendorEmail = $('#VendorEmail');
    if (vendorEmail.val() === "") {
        vendorEmail.addClass('is-invalid');
        isValid = false;
    } else {
        vendorEmail.removeClass('is-invalid');
    }

    // Validate Vendor Contact
    const vendorContact = $('#VendorContact');
    const contactPattern = /^[0-9]{10}$/;
    if (vendorContact.val() === "" || !contactPattern.test(vendorContact.val().trim())) {
        vendorContact.addClass('is-invalid');
        isValid = false;
    } else {
        vendorContact.removeClass('is-invalid');
    }

    // Validate Vendor Address
    const vendorAddress = $('#VendorAddress');
    if (vendorAddress.val() === "") {
        vendorAddress.addClass('is-invalid');
        isValid = false;
    } else {
        vendorAddress.removeClass('is-invalid');
    }

    // Validate Vendor GST
    const vendorGST = $('#VendorGST');
    if (vendorGST.val() === "") {
        vendorGST.addClass('is-invalid');
        isValid = false;
    } else {
        vendorGST.removeClass('is-invalid');
    }

    const vendorCreatedBy = $('#VendorCreatedby');
    if (vendorCreatedBy.val() === "") {
        vendorCreatedBy.addClass('is-invalid');
        isValid = false;
    } else {
        vendorCreatedBy.removeClass('is-invalid');
    }


    const vendorCreatedDate = $('#VendorCreateddate');
    if (vendorCreatedDate.val() === "") {
        vendorCreatedDate.addClass('is-invalid');
        isValid = false;
    } else {
        vendorCreatedDate.removeClass('is-invalid');
    }

    return isValid;
}

// Save Vendor Function
function saveVendor() {
    var vendor = {
        VedorID: $('#VendorID').val(),
        VendorName: $('#VendorName').val(),
        VendorEmail: $('#VendorEmail').val(),
        VendorContact: $('#VendorContact').val(),
        VendorAddress: $('#VendorAddress').val(),
        VendorType: $('#VendorType').val(),
        VendorGST: $('#VendorGST').val(),
        CreatedBy: $('#VendorCreatedby').val(),
        CreatedDate: $('#VendorCreateddate').val()
    };

    $.ajax({
        url: '/vendor/AddVendor',
        type: 'POST',
        data: vendor,
        success: function (response) {
            if (response.StatusCode == 200) {
                console.log(response.data);
                $('.vendor-success-message').text(response.Message);
                $('#vendorSuccessModal').modal('show');
            } else {
                $('.vendor-success-message').text(response.Message);
                $('#vendorSuccessModal').modal('show');
            }
        },
        error: function (error) {
            console.error(error);
            alert('An error occurred while saving the vendor. Please try again.');
        }
    });
}

// Button actions
$('.vendoradd-Cancel').click(function () {
    window.location.href = "/Vendor/Index"; // Redirect to vendor list page on cancel
});

$('.vendoradd-Update').click(function () {
    event.preventDefault();
    if (validateForm()) {
        saveVendor();
    }
});

function downloadVendorTemplate() {
    var link = document.createElement('a');
    link.href = '/assets/templates/VendorImport.xlsx';
    link.download = 'VendorImportTemplate.xlsx';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}



$(document).on('click', '.btn-vendor-import-submit', function (event) {
    event.preventDefault();

    var fileInput = document.getElementById('vendor-file-upload-input');
    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);

    $.ajax({
        url: '/Vendor/ImportVendors',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        dataType: 'json',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            $('#vendorImportSuccessModal').modal('show');
            $('.vendor-success-message').text(response.Message);
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error('Error uploading file:', error);
        },
        complete: function () {
            $('.show-progress').hide();
        }
    });
});

function handleVendorFileUpload(input) {
    var fileName = input.files[0].name;
    $('#vendor-uploaded-file-info').show();
    $('#vendor-uploaded-file-text').text(fileName);
}


// vendor type add js 

$(document).on('click', '#saveVendorTypeBtn', function () {
    var newVendorType = $('#newVendorType').val().trim();
    var isDuplicate = false;

    // Check for duplicate vendor type name
    $('#VendorType option').each(function () {
        if ($(this).text().toLowerCase() === newVendorType.toLowerCase()) {
            isDuplicate = true;
            return false; // break the loop
        }
    });

    // Validate input and display messages
    if (newVendorType === '') {
        $('#vendorTypeError').text('Vendor Type cannot be empty.').show();
        $('#vendorTypeSuccessMessage').hide();
    } else if (isDuplicate) {
        $('#vendorTypeError').text('Vendor Type already exists.').show();
        $('#vendorTypeSuccessMessage').hide();
    } else {
        $('#vendorTypeError').hide();
        $.ajax({
            url: '/vendor/AddVendorType',
            method: 'POST',
            data: { VendorType: newVendorType },
            success: function (response) {
                if (response.StatusCode == 200) {
                    // Add the new vendor type to the dropdown
                    $('#VendorType').append('<option value="' + newVendorType + '">' + newVendorType + '</option>');

                    // Clear and hide the input field, show success message
                    $('#newVendorType').val('');
                    $('#addVendorTypeForm').hide();
                    $('#vendorTypeSuccessMessage').text('Vendor Type added successfully.').show();

                    // Hide Save button and change Close button to 'Close'
                    $('#saveVendorTypeBtn').hide();
                    $('.modal-footer .btn-secondary').text('Close');

                    setTimeout(function () {
                        $('#vendorTypeSuccessMessage').hide();
                        $('#addVendorTypeForm').show();
                        $('#addVendorTypeModal').modal('hide');
                        $('#saveVendorTypeBtn').show();
                        $('.modal-footer .btn-secondary').text('Close');
                    }, 3000); // Hide the message after 3 seconds
                } else {
                    $('#vendorTypeError').text('Error while adding Vendor Type to the database.').show();
                    $('#vendorTypeSuccessMessage').hide();
                }
            },
            error: function () {
                $('#vendorTypeError').text('Error while adding Vendor Type to the database.').show();
                $('#vendorTypeSuccessMessage').hide();
            }
        });
    }
});


$(document).on('click', '.refresh-vendortablist', function (event) {
    window.location.href = "/vendor/index";
    return false;
});

$(document).on('click', '.vendor-list-edit', function (event) {
    var slectedVendor = $(this).attr("data-vendorid");
    window.location.href = "/vendor/AddVendor?vendorid=" + slectedVendor;
    return false;
});


//Vendor Export Functionality....

$('#vendor-selectAll').click(function () {
    $('.vendor-check').prop('checked', this.checked);
});


function exportVendor() {
    var selectedVendorIds = [];
    $(".vendor-check:checked").each(function () {
        selectedVendorIds.push($(this).closest("tr").find(".tdvendorid").text());
    });

    if (selectedVendorIds.length === 0) {
        alert("Please select at least one vendor to export.");
        return;
    }

    $.ajax({
        url: '/Vendor/ExportSelectedVendors',
        type: 'POST',
        data: JSON.stringify({ selectedVendorIds: selectedVendorIds }),
        contentType: 'application/json; charset=utf-8',
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data) {
            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = 'SelectedVendors.xlsx';
            link.click();
        },
        error: function (error) {
            console.error(error);
            alert('An error occurred while exporting vendors. Please try again.');
        }
    });
}


//Vendor Import functionality 

$(document).off('change', '#vendor-file-upload-input').on('change', '#vendor-file-upload-input', function (event) {
    event.preventDefault();
    var file = this.files[0];
    formData = new FormData();
    formData.append('file', file);

    // Show selected file info
    if (file) {
        $('#vendor-uploaded-file-text').text("Selected file: " + file.name);
        $('#vendor-uploaded-file-info').show();
    }
});

$(document).on('click', '.btn-vendor-import-submit', function (event) {
    event.preventDefault();
    $.ajax({
        url: '/Vendor/ImportVendors',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        dataType: 'json',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            if (response.success) {
                $('#vendorImportSuccessModal').modal('show');
                $('.vendor-success-message').text("Vendors imported successfully!");
                // Optionally, refresh the vendor table or other UI components
            } else {
                alert('Error: ' + response.message);
            }
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error('Error uploading file:', error);
        },
        complete: function () {
            isSubmitting = false;
            $('.show-progress').hide();
        }
    });
});


function approveVendor(vendorId) {
    var reason = $('#approvalReason').val();
    $.ajax({
        type: "POST",
        url: "/vendor/approvevendorsuperadmin", // Update the URL according to your routing setup
        data: { vendorId: $('#modalVendorID').text(), approvalReason: reason },
        success: function (response) {
            $('#sadmin-approvalcard').modal('show');
            if (response.success) {
                updateVendorStatus(vendorId, "Approved");
                generateModalFooter("Approved");
                $('#modalMessage').text(response.message).removeClass('text-danger').addClass('text-success');

            } else {
                $('#modalMessage').text(response.message).removeClass('text-success').addClass('text-danger');
            }
        },
        error: function () {
            $('#modalMessage').text('An error occurred. Please try again.').removeClass('text-success').addClass('text-danger');
        }
    });
}


function rejectVendor(vendorId) {
    var reason = $('#approvalReason').val();
    $.ajax({
        type: "POST",
        url: "/vendor/rejectvendor", // Update the URL according to your routing setup
        data: { vendorId: $('#modalVendorID').text(), approvalReason: reason },
        success: function (response) {
            $('#sadmin-approvalcard').modal('show');
            if (response.success) {
                updateVendorStatus(vendorId, "Rejected");
                generateModalFooter("Rejected");
                $('#modalMessage').text(response.message).removeClass('text-danger').addClass('text-success');
               
            } else {
                $('#modalMessage').text(response.message).removeClass('text-success').addClass('text-danger');
            }
        },
        error: function () {
            $('#modalMessage').text('An error occurred. Please try again.').removeClass('text-success').addClass('text-danger');
        }
    });
}


function updateVendorStatus(vendorId, status) {
    var row = $('#sadminvendorapprovaltable .tdvendorapprovalid').filter(function () {
        return $(this).text() == vendorId;
    }).closest('tr');

    row.find('td:eq(7)').html('<span class="sadmin-vendorapproved-btn"><img src="/assets/' + status + '.png" alt="' + status + '" style="width:25px"></span>');
}


function generateModalFooter(status) {
    let modalFooter = $('.modal-footer'); // Assuming you have a modal footer with this class
    modalFooter.empty(); // Clear the existing content

    // Add the message div
    modalFooter.append(`
        <div id="modalMessage" class="text-success"></div>
    `);

    if (status === 'Pending') {
        modalFooter.append(`
            <button class="btn btn-success" id="approveBtn" onclick="approveVendor()">Approve</button>
            <button class="btn btn-danger" id="rejectBtn" onclick="rejectVendor()">Reject</button>
        `);
    } else if (status === 'Approved') {
        modalFooter.append(`
            <a href="#" onclick="rejectVendor()">Change Status</a>
            <button class="btn btn-success" disabled>Approved</button>
        `);
    } else if (status === 'Rejected') {
        modalFooter.append(`
            <a href="#" onclick="approveVendor()">Change Status</a>
            <button class="btn btn-danger" disabled>Rejected</button>
        `);
    }
}


