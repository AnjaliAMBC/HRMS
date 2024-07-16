$(document).on('change', '#emp-raiseticket-ticketype', function (event) {
    var ticketType = $(this).val();
    if (ticketType === 'HR') {
        $('#emp-raiseticket-hrcategory').show();
        $('#emp-raiseticket-itcategory').hide();
    } else if (ticketType === 'IT') {
        $('#emp-raiseticket-itcategory').show();
        $('#emp-raiseticket-hrcategory').hide();
    }
    $('#category-div').show();
});

$(document).on('click', '.btn-apply-emp-raiseticket-submit', function (event) {
    event.preventDefault();
    var isValid = true;
    $('.error-message').text('');
    $('.form-control').removeClass('error');

    var ticketType = $('#emp-raiseticket-ticketype').val();
    var category = ticketType === 'HR' ? $('#emp-raiseticket-hrcategory').val() : $('#emp-raiseticket-itcategory').val();

    if (ticketType === null) {
        $('#emp-raiseticket-ticketype-error').text('Please select a ticket type.').addClass('error');
        $('#emp-raiseticket-ticketype').addClass('error');
        isValid = false;
    }
    if (category === null) {
        $('#emp-raiseticket-category-error').text('Please select a category.').addClass('error');
        if (ticketType === 'HR') {
            $('#emp-raiseticket-hrcategory').addClass('error');
        } else {
            $('#emp-raiseticket-itcategory').addClass('error');
        }
        isValid = false;
    }
    if ($('#emp-raiseticket-subject').val().trim() === '') {
        $('#emp-raiseticket-subject-error').text('Please enter a subject.').addClass('error');
        $('#emp-raiseticket-subject').addClass('error');
        isValid = false;
    }
    if ($('#emp-raiseticket-description').val().trim() === '') {
        $('#emp-raiseticket-description-error').text('Please enter a description.').addClass('error');
        $('#emp-raiseticket-description').addClass('error');
        isValid = false;
    }
    if ($('#emp-raiseticket-attach-file-upload').val() === '') {
        $('#emp-raiseticket-attach-error').text('Please upload a file.').addClass('error');
        $('.emp-raiseticket-attach-label').addClass('error');
        isValid = false;
    } else {
        var file = $('#emp-raiseticket-attach-file-upload')[0].files[0];
        if (file.size > 2097152) { // 2MB
            $('#emp-raiseticket-attach-error').text('File size must be less than 2MB.').addClass('error');
            $('.emp-raiseticket-attach-label').addClass('error');
            isValid = false;
        }
        if (!['image/jpeg', 'image/png'].includes(file.type)) {
            $('#emp-raiseticket-attach-error').text('Only JPEG and PNG files are allowed.').addClass('error');
            $('.emp-raiseticket-attach-label').addClass('error');
            isValid = false;
        }
    }
    if ($('#emp-raiseticket-priority').val() === null) {
        $('#emp-raiseticket-priority-error').text('Please select a priority.').addClass('error');
        $('#emp-raiseticket-priority').addClass('error');
        isValid = false;
    }

    if (isValid) {
        var formData = new FormData();

        formData.append('TicketType', ticketType);
        formData.append('Category', category);
        formData.append('Subject', $('#emp-raiseticket-subject').val());
        formData.append('Description', $('#emp-raiseticket-description').val());
        formData.append('Priority', $('#emp-raiseticket-priority').val());
        formData.append('EmployeeID', $('.loggedinempid').text());
        formData.append('EmployeeName', $('.loggedinempname').text());
        formData.append('OfficialEmailID', $('.loggedinempemail').text());
        formData.append('Status', 'Pending');
        formData.append('Location', $('.loggedinemplocation').text());
        formData.append('File', $('#emp-raiseticket-attach-file-upload')[0].files[0]);

        $.ajax({
            url: '/empticketing/raiseticket',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                $('#messageModal').modal('show');
                // Reset form fields
                $('#emp-ticket-raise-Form')[0].reset();
                $('#category-div').hide(); // Hide category for next ticket
                $('#emp-raiseticket-itcategory').hide(); // Hide IT category
                $('#emp-raiseticket-hrcategory').hide(); // Hide HR category
            },
            error: function (response) {
                // Handle error
                alert('An error occurred. Please try again.');
            }
        });
    }
});


$(document).on('click', '.btn-apply-emp-raiseticket-cancel', function (event) {
    event.preventDefault();
    $('#emp-ticket-raise-Form')[0].reset();
    $('#category-div').show();
});


$(document).on('change', '#emp-raiseticket-attach-file-upload', function (event) {
    var fileName = $(this).val().split('\\').pop();
    $('.emp-raiseticket-file-name').text(fileName);
    $('.emp-raiseticket-attach-label').removeClass('error');
    $('#emp-raiseticket-attach-error').text('');
});