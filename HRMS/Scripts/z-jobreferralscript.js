
$(document).on('click', '.ed-b-job-referal', function () {
    window.location.href = "/EmpDash/JobReferral";
});


$(document).on('click', '.emp-jobreferal-data-title', function () {
    window.location.href = "/EmpDash/JobDetail";
});

$(document).on("click", ".admin-jobreferal-data-title", function (e) {
    e.preventDefault();
    var jobID = $(this).data("jobid");
    window.location.href = "/AdminDashboard/AdminJobDetail?jobId=" + jobID;
});
$(document).on('click', '.btn_admin_postjobreferal', function () {
    window.location.href = "/AdminDashboard/AdminPostJobs";
});

$(document).on('click', '[data-toggle="modal"]', function () {
    var jobID = $(this).data('jobid');
    var empID = $('.loggedinempid').text();
    $(".referfriend-submit").next('.message').text("");

    $('#ReferFriendPopup').find('#ReferFriend-JobID').val(jobID);

    $('#ReferFriendPopup').find('#ReferFriend-EmployeeID').val(empID);
});


$(document).on('click', '.referfriend-submit', function () {
    var jobID = $("#ReferFriend-JobID").val();
    var candidateName = $("#ReferFriend-ReferralName").val();
    var candidateContact = $("#ReferFriend-Contact").val();
    var referredBy = $("#ReferFriend-Referredby").val();
    var ReferredByID = $('#ReferFriend-Referralid').val();
    var referredByEmail = $("#ReferFriend-ReferralEmail").val();
    var file = $("#referfriend-attach-file-upload")[0].files[0];

    // Reset all previous error messages
    resetValidationErrors();

    // Optional: Perform validation before submission
    var isValid = true;

    if (!candidateName) {
        showValidationError('#ReferFriend-ReferralName', 'Referral Name is required.');
        isValid = false;
    }

    if (!candidateContact) {
        showValidationError('#ReferFriend-Contact', 'Referral Contact is required.');
        isValid = false;
    }

    if (!referredByEmail) {
        showValidationError('#ReferFriend-ReferralEmail', 'Referral Email is required.');
        isValid = false;
    }

    if (file) {
        var allowedFileTypes = ['.doc', '.docx', '.pdf'];
        var fileExtension = file.name.split('.').pop().toLowerCase();
        if (!allowedFileTypes.includes('.' + fileExtension)) {
            showFileValidationError('Only doc, docx, pdf files are allowed.');
            isValid = false;
        }
    } else {
        showFileValidationError('Please upload a file.');
        isValid = false;
    }

    if (!isValid) {
        return;
    }

    var formData = new FormData();
    formData.append("JobID", jobID);
    formData.append("CandidateName", candidateName);
    formData.append("CandidateContact", candidateContact);
    formData.append("ReferredBy", referredBy);
    formData.append("ReferredByID", ReferredByID);

    formData.append("ReferredByEmail", referredByEmail);
    if (file) {
        formData.append("Resume", file);
    }


    $.ajax({
        url: '/EmpDash/ReferJob',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                displayMessage('Referral submitted successfully!', 'success');
                $('#ReferFriend-Contact').val('');
                $('#ReferFriend-ReferralName').val('');
                $('#ReferFriend-ReferralEmail').val('');
                $('#referfriend-attach-file-upload').val('');
                $('.referfriend-file-name').text('');
                $("#ReferFriend-Contact").val('');
                $("#ReferFriend-Referredby").val();
            } else {
                // Show the error message
                displayMessage('Error: ' + response.message, 'error');
            }
        },
        error: function () {
            displayMessage('An error occurred while submitting the referral.', 'error');
        }
    });
});

// Function to display error messages for file upload
function showFileValidationError(message) {
    $('#referfriend-attach-error').text(message).addClass('text-danger');
    $('#referfriend-attach-file-upload').addClass('is-invalid');
}

// Function to display validation error message under each field
function showValidationError(inputId, message) {
    var inputField = $(inputId);
    inputField.addClass('is-invalid');
    var feedback = inputField.next('.invalid-feedback');
    if (feedback.length === 0) {
        feedback = $('<div class="invalid-feedback"></div>').insertAfter(inputField);
    }
    feedback.text(message);
}

// Reset all validation errors
function resetValidationErrors() {
    $('.form-control').removeClass('is-invalid');
    $('.invalid-feedback').remove();
    $('#referfriend-attach-file-upload').removeClass('is-invalid');
    $('#referfriend-attach-error').text('').removeClass('text-danger');
}

// Function to display success/error messages in the modal
function displayMessage(message, type) {
    // Create the message span if it doesn't exist
    var messageSpan = $(".referfriend-submit").next('.message');
    if (!messageSpan.length) {
        messageSpan = $('<span class="message"></span>').insertAfter(".referfriend-submit");
    }

    // Set the message text and style based on the type
    messageSpan.text(message).removeClass('text-success text-error');
    if (type === 'success') {
        messageSpan.addClass('text-success');
    } else {
        messageSpan.addClass('text-error');
    }

    // Ensure the message is visible
    messageSpan.show();
}

$(document).on('change', '#referfriend-attach-file-upload', function () {
    var file = this.files[0];
    if (file) {
        $('.referfriend-file-name').text(file.name);
    } else {
        $('.referfriend-file-name').text('');
    }
});

$(document).on('click', '.admin_applied_jobreferal', function () {
    var jobId = $(this).data('jobid');

    $.ajax({
        url: '/admindashboard/loadcandidates',
        type: 'GET',
        data: { jobId: jobId },
        success: function (result) {
            $('#adminJobReferralProcessModal .modal-body').html(result);
            $('#adminJobReferralProcessModal').modal('show');
        },
        error: function () {
            alert('Failed to load candidates.');
        }
    });
});


$(document).on('change', '#jobreferred_status_data', function () {
    var sno = $(this).data('sno');

    $.ajax({
        url: '/admindashboard/candidatestatusupdate',
        type: 'POST',
        data: { sno: sno, status: $('#jobreferred_status_data').val() },
        success: function (result) {
        },
        error: function () {
        }
    });
});
