
$(document).on('change', '#addvendor', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/vendor/addvendor';
    } else if (selectedValue === "importuser") {
        window.location.href = '/vendor/importvendor';
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
        url: '/vendor/addvendor',
        type: 'POST',
        data: vendor,
        beforeSend: function () {
            $('.show-progress').show(); // Show loader before the request is sent
        },
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
        },
        complete: function () {
            $('.show-progress').hide(); // Hide loader after request completes
        }
    });
}


// Button actions
$(document).on('click', '.vendoradd-Cancel', function (event) {
    event.preventDefault();
    window.location.href = "/vendor/index"; // Redirect to vendor list page on cancel
});



$(document).on('click', '.vendoradd-Update', function (event) {
    event.preventDefault();
    if (validateForm()) {
        saveVendor();
    }
});

function downloadVendorTemplate() {
    var link = document.createElement('a');
    link.href = '/assets/templates/VendorImportTemplate.xlsx';
    link.download = 'VendorImportTemplate.xlsx';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}



$(document).on('click', '.btn-vendor-import-submit', function (event) {
    event.preventDefault();

    // Check if a file is selected
    var fileInput = document.getElementById('vendor-file-upload-input');
    if (!fileInput || fileInput.files.length === 0) {
        alert("Please select a file to upload.");
        return;
    }

    var file = fileInput.files[0];
    var formData = new FormData();
    formData.append('file', file);

    $.ajax({
        url: '/Vendor/ImportVendor',
        type: 'POST',
        data: formData,
        processData: false, 
        contentType: false, // Prevent jQuery from setting the Content-Type header
        dataType: 'json',
        beforeSend: function () {
            $('.show-progress').show(); // Show progress indicator if any
        },
        success: function (response) {
            if (response.success) { // Check if the response indicates success
                $('#vendorImportSuccessModal').modal('show');
                $('.vendor-success-message').text(response.message);
            } else {               
                alert(response.message || "An error occurred during import.");
            }
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error('Error uploading file:', error);
            alert("An error occurred during file upload: " + error);
        },
        complete: function () {
            $('.show-progress').hide(); // Hide progress indicator
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
            url: '/vendor/addvendortype',
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
    window.location.href = "/vendor/addvendor?vendorid=" + slectedVendor;
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
        url: '/vendor/exportselectedvendors',
        type: 'POST',
        data: JSON.stringify({ selectedVendorIds: selectedVendorIds }),
        contentType: 'application/json; charset=utf-8',
        xhrFields: {
            responseType: 'blob',
        },
        beforeSend: function () {
            $('.show-progress').show(); // Show loader before the request is sent
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
        },
        complete: function () {
            $('.show-progress').hide(); // Hide loader after request completes
        }
    });
}





function approveVendor(vendorId) {
    var reason = $('#approvalReason').val();
    $.ajax({
        type: "POST",
        url: "/vendor/approvevendorsuperadmin", // Update the URL according to your routing setup
        data: { vendorId: $('#modalVendorID').text(), approvalReason: reason },
        success: function (response) {
            $('#sadmin-approvalcard').modal('show');
            if (response.success) {
                updateVendorStatus($('#modalVendorID').text(), "Approved");
                generateModalFooter("Approved");
                refreshVendorTable();
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

function refreshVendorTable() {
    $.ajax({
        url: "/vendor/approvevendorpartial", // Make sure this URL matches your routing setup
        type: "GET",
        success: function (data) {
            $('.res-sadmin-vendorapproval-table').html(data);

            // Update the hidden field with the new JSON data
            var newVendorsJson = $('#vendorsJson').val();
            $('#vendorsJson').val(newVendorsJson);

            LoadVendorTable();
        },
        error: function () {
            alert("Failed to refresh the table. Please try again.");
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
                updateVendorStatus($('#modalVendorID').text(), "Rejected");
                refreshVendorTable();
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


//function updateVendorStatus(vendorId, status) {
//    var row = $('#sadminvendorapprovaltable .tdvendorapprovalid').filter(function () {
//        return $(this).text() == vendorId;
//    }).closest('tr');

//    row.find('td:eq(7)').html('<span class="sadmin-vendorapproved-btn"><img src="/assets/' + status + '.png" alt="' + status + '" style="width:25px"></span>');
//}


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


function updateVendorStatus(vendorId, newStatus) {
    // Find the table row by Vendor ID
    $('#sadminvendorapprovaltable tbody tr').each(function () {
        var row = $(this);
        var currentVendorId = row.find('.tdvendorid').text().trim();

        if (currentVendorId == vendorId) {
            // Update the status text and image based on the new status
            var statusCell = row.find('td').eq(7); // Assuming the status is in the 8th column (index 7)

            if (newStatus === 'Approved') {
                statusCell.html('<span class="sadmin-vendorapproved-btn"><img src="/assets/Approve.png" alt="Approved" style="width:25px"><div style="display: block">Approved</div></span>');
            } else if (newStatus === 'Rejected') {
                statusCell.html('<span class="sadmin-vendorapproved-btn"><img src="/assets/Reject.png" alt="Rejected" style="width:25px"><div style="display: block">Rejected</div></span>');
            } else {
                statusCell.html('<span class="sadmin-vendorapproved-btn"><img src="/assets/Pending.png" alt="Pending" style="width:25px"><div style="display: block">Pending</div></span>');
            }
        }
    });
}

function LoadVendorTable() {
    // Initialize the table as a DataTable
    var table = $('#sadminvendorapprovaltable').DataTable({
        "responsive": true,
        "paging": true,
        "searching": true, // Enable searching
        "ordering": false,
        "info": true,
        "autoWidth": false,
        "lengthMenu": [[10, 20, 50, -1], [10, 20, 50, "All"]]
    });

    // Implement global search functionality
    $('.vendorapproval-search').on('keyup', function () {
        table.search(this.value).draw();
    });

    // Filter table based on status dropdown selection
    $(document).on('change', '#sadmin-vendorapproval-status-dropdown', function () {
        var status = $(this).val();
        if (status == "All") {
            status = "";
        }
        table.column(7).search(status).draw(); // Assume status is in the 8th column (index 7)
    });
}


$(document).ready(function () {
    if ($('#sadminvendorapprovaltable').length) {
        LoadVendorTable();
    }
});




function getVendorById(vendor, vendorID) {
    var selectedVendor = vendor.find(v => v.VedorID == vendorID);
    return selectedVendor;
}

function openVendorModal(vendorid) {
    // Find the table row corresponding to the vendor ID
    var row = $('#sadminvendorapprovaltable').find('tr[data-vendorid="' + vendorid + '"]');

    // Extract data from the row
    var vendorID = row.find('.tdvendorid').text().trim();
    var vendorName = row.find('.tdvendorapprovalname').text().trim();
    var vendorContact = row.find('.tdvendorcontact').text().trim();
    var vendorAddress = row.find('.tdvendoraddress').text().trim();
    var createdDate = row.find('.tdcreateddate').text().trim();
    var createdBy = row.find('.tdcreatedby').text().trim();
    var vendorStatus = row.find('.tdvendorstatus div').text().trim();
    var approveRejectReason = row.find('.tdvendorstatus').attr('data-approve-reject-reason'); // Assuming you store the reason in a data attribute

    // Clear the modal fields
    $('#modalVendorID').text("");
    $('#modalVendorName').text("");
    $('#modalVendorContact').text("");
    $('#modalVendorAddress').text("");
    $('#approvalReason').text("");

    // Set the modal fields with the extracted data
    $('#modalVendorID').text(vendorID);
    $('#modalVendorName').text(vendorName);
    $('#modalVendorContact').text(vendorContact);
    $('#modalVendorAddress').text(vendorAddress);
    $('#approvalReason').text(approveRejectReason);

    generateModalFooter(vendorStatus);
}


$(document).on('click', '.btn-vendor-import-submit', function () {
    var inputFile = document.getElementById('vendor-file-upload-input');
    if (!inputFile.files.length) {
        alert('Please select a file to upload');
        return;
    }

    var formData = new FormData();
    formData.append('file', inputFile.files[0]);

    $.ajax({
        url: '/vendor/importvendors',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response.StatusCode == 200) {
                $('#vendorImportSuccessModal').modal('show');
                $('.vendor-success-message').text(response.Message);
                $('.vendor-success-message').css('color', 'green');
            } else {
                $('#assetImportSuccessModal').modal('show');
                $('.vendor-success-message').text(response.Message);
                $('.vendor-success-message').css('color', 'red');
            }
        },
        error: function (xhr, status, error) {
            $('#vendorImportSuccessModal').modal('show');
            $('.vendor-success-message').text(response.Message);
            $('.vendor-success-message').css('color', 'red');
        }
    });
});

