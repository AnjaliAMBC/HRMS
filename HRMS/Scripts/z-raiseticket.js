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

$(document).off('click', '.btn-apply-emp-raiseticket-submit').on('click', '.btn-apply-emp-raiseticket-submit', function (event) {
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
        //$('#emp-raiseticket-attach-error').text('Please upload a file.').addClass('error');
        //$('.emp-raiseticket-attach-label').addClass('error');
        //isValid = false;
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
        formData.append('Status', 'Open');
        formData.append('Location', $('.loggedinemplocation').text());
        formData.append('File', $('#emp-raiseticket-attach-file-upload')[0].files[0]);
        formData.append('IsEditRecord', $('.iseditablerecorddiv').text());
        formData.append('EdiatbleRecordNumber', $('.editablerecordticketnumber').text());
        $.ajax({
            url: '/empticketing/raiseticket',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            beforeSend: function () {
                $('.show-progress').show();
            },
            success: function (response) {
                $('#TicketingmessageModal .modal-body').html('<p  style="color: green;">' + response.message + '</p>');
                $('#TicketingmessageModal').modal('show');

                $('#emp-ticket-raise-Form')[0].reset();
                $('#category-div').hide();
                $('#emp-raiseticket-itcategory').hide();
                $('#emp-raiseticket-hrcategory').hide();
                $('.show-progress').hide();
            },
            error: function (response) {
                $('.show-progress').hide();
                // Handle error
                alert('An error occurred. Please try again.');
            }
        });
    }
});


$(document).on('click', '.btn-apply-emp-raiseticket-cancel', function (event) {
    event.preventDefault();

    var isEdit = $(this).data('isedit');

    if (isEdit == "True") {
        window.location.href = '/empTicketing/index';
        return;
    } else {
        $('#empTicketConfirmCancelModal').removeAttr('disabled');
        $('#emp-ticket-raise-Form')[0].reset();       
    }
});


$(document).on('change', '#emp-raiseticket-attach-file-upload', function (event) {
    var fileName = $(this).val().split('\\').pop();
    $('.ticketing_filepath').hide();
    $('.emp-raiseticket-file-name').text(fileName);
    $('.emp-raiseticket-attach-label').removeClass('error');
    $('#emp-raiseticket-attach-error').text('');
});

function toggleLeaveTicketHistoryActionOptions(iconElement) {
    const optionsMenu = $(iconElement).next('.emp-tickethistoryoptions');
    $('.emp-tickethistoryoptions').not(optionsMenu).hide(); // Hide other open menus
    optionsMenu.toggle(); // Toggle the clicked menu

    // Add an event listener to the document to close the menu when clicking outside
    $(document).on('click.hideMenu', function (event) {
        // Check if the click was outside the options menu or the icon that triggered it
        if (!$(event.target).closest(iconElement).length && !$(event.target).closest('.emp-tickethistoryoptions').length) {
            optionsMenu.hide(); // Hide the menu
            $(document).off('click.hideMenu'); // Remove the event listener to prevent multiple bindings
        }
    });
}

var currentSelectedrow;

function emptickethistorycancel(currentthis) {
    currentSelectedrow = currentthis.closest('tr');
    var ticketName = currentthis.attr("data-ticketname");
    $('#empTicketConfirmCancelButton').attr('data-ticketname', ticketName);
    $('#empTicketConfirmCancelModal .modal-body').html('Are you sure you want to cancel this ticket?');
    $('#empTicketConfirmCancelButton').attr('disabled', false);
    $('#empTicketConfirmCancelModal').modal('show');
}

$(document).on('click', '#empTicketConfirmCancelButton', function (event) {
    var ticketName = $(this).attr('data-ticketname');
    var $currentButton = $(this); // Store reference to the clicked button

    if (ticketName) {
        var $row = currentSelectedrow;

        if ($row) {
            $.ajax({
                url: '/empticketing/cancelticket', // Ensure this URL is correct
                method: 'POST',
                data: {
                    ticketName: ticketName,
                    status: 'Cancelled'
                },
                beforeSend: function () {
                    $('.show-progress').show();
                },
                success: function (response) {
                    if (response.success) {
                        $('#empTicketConfirmCancelModal .modal-body').html('<p>Ticket has been cancelled.</p>');
                        $currentButton.attr('disabled', true);

                        // Update the status of the current row to 'Cancelled'
                        $row.find('.res-empticketlisting-status-level').text('Cancelled').addClass('ticket-status-cancelled');
                        $row.find('.emp-ticket-edit-history').hide();
                        toggleLeaveTicketHistoryActionOptions($row.find('.emp-ticket-edit-history'));
                    } else {
                        $('#empTicketConfirmCancelModal .modal-body').html('<p>An error occurred: ' + response.message + '</p>');
                    }

                    $('.show-progress').hide();
                },
                error: function (xhr, status, error) {
                    $('.show-progress').show();
                    $('#empTicketConfirmCancelModal .modal-body').html('<p>An error occurred. Please try again later.</p>');
                }
            });
        } else {
            $('#empTicketConfirmCancelModal .modal-body').html('<p>Row not found in the DataTable.</p>');
        }
    } else {
        $('#empTicketConfirmCancelModal .modal-body').html('<p>Ticket information is missing. Please try again.</p>');
    }
});

// Function to open the modal and set the ticketName attribute
function openCancelModal(ticketNo) {
    $('#empTicketConfirmCancelButton').attr('data-ticketname', ticketNo);
    $('#empTicketConfirmCancelModal').modal('show');
}

// Example usage
// <button onclick="openCancelModal('TICKET123')">Cancel Ticket</button>



//$(document).on('click', '.btn-apply-emp-tickethistory-comment-reopen', function (event) {
//    var ticketName = $(this).attr('data-ticketname');
//    var ticketStatus = $(this).attr('data-status');
//    var comments = $('#emp-tickethistory-comments').val();
//    var $currentButton = $(this); 

//    if (ticketName) {
//        var $row = currentSelectedrow;

//        if ($row) {
//            $.ajax({
//                url: '/empticketing/TicketStatusChangeByEmp', // Ensure this URL is correct
//                method: 'POST',
//                data: {
//                    ticketName: ticketName,
//                    status: ticketStatus,
//                    comments: comments
//                },
//                success: function (response) {
//                    if (response.success) {
//                        $('#empTicketConfirmCancelModal .modal-body').html('<p>Ticket has been cancelled.</p>');
//                        $currentButton.attr('disabled', true);

//                        // Update the status of the current row to 'Cancelled'
//                        $row.find('.res-empticketlisting-status-level').text('Cancelled');
//                        $row.find('.emp-ticket-edit-history').hide();
//                        toggleLeaveTicketHistoryActionOptions($row.find('.emp-ticket-edit-history'));
//                    } else {
//                        $('#empTicketConfirmCancelModal .modal-body').html('<p>An error occurred: ' + response.message + '</p>');
//                    }
//                },
//                error: function (xhr, status, error) {
//                    $('#empTicketConfirmCancelModal .modal-body').html('<p>An error occurred. Please try again later.</p>');
//                }
//            });
//        } else {
//            $('#empTicketConfirmCancelModal .modal-body').html('<p>Row not found in the DataTable.</p>');
//        }
//    } else {
//        $('#empTicketConfirmCancelModal .modal-body').html('<p>Ticket information is missing. Please try again.</p>');
//    }
//});
var currentSelectedrow;

$(document).on('click', '.emp-tickethistory-acknowledge', function (event) {
    event.preventDefault();
    var ticketNo = $(this).closest('tr').find('.emp-ticketing-listing-title').data('ticketnum');
    currentSelectedrow = $(this).closest('tr');

    $('#empTicketCommentsModal').data('ticketNo', ticketNo);

    $.ajax({
        url: '/adminticketing/getticketdetailsbynumber?ticketNo=' + ticketNo,
        method: 'GET',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (data) {
            var modalBody = $('#empTicketCommentsModal .modal-body .emp-ticketing-commentbox-popup-list');
            populateModal(data, modalBody);
            $('#empTicketCommentsModal').modal('show');
            $('.show-progress').hide();
        },
        error: function (err) {
            $('.show-progress').show();
            console.log(err);
        }
    });
});



$(document).on('click', '.btn-apply-emp-tickethistory-comment-reopen, .btn-apply-emp-tickethistory-comment-submitbtn', function (event) {
    event.preventDefault();

    // Retrieve ticket number from the modal's data attribute
    var ticketNo = $('#empTicketCommentsModal').data('ticketNo');
    var ticketStatus = $(this).hasClass('btn-apply-emp-tickethistory-comment-reopen') ? 'Re-Open' : 'Closed';
    var comments = $('#emp-tickethistory-comments').val();
    var updateby = $('.loggedinempname').text();
    var updatebyID = $('.loggedinempid').text();
    var $currentButton = $(this);

    if (ticketNo && ticketStatus) {
        var $modal = $('#empTicketCommentsModal');
        $.ajax({
            url: '/empticketing/ticketstatuschangebyemp?ticketNo=' + ticketNo,
            method: 'POST',
            data: {
                ticketName: ticketNo,
                status: ticketStatus,
                comments: comments,
                updateby: updateby,
                updatebyID: updatebyID
            },
            beforeSend: function () {
                $('.show-progress').show();
            },
            success: function (response) {
                if (response.success) {
                    $('#empTicketCommentsModal').modal('show');

                    // Hide the comment box popup after successful operation
                    $modal.find('.emp-ticketing-commentbox-popup').hide();
                    $modal.find('.emp-ticketing-statuschange-div').html('<p style="color: green;">Ticket status has been updated successfully.</p>');

                    // Update the status in the current row
                    if (ticketStatus === 'Re-Open') {
                        currentSelectedrow.find('.res-empticketlisting-status-level').text('Re-Open');
                    } else if (ticketStatus === 'Closed') {
                        currentSelectedrow.find('.res-empticketlisting-status-level').text('Closed');
                    }

                    // Optionally, disable the current button
                    $currentButton.attr('disabled', true);
                } else {
                    $messageContainer.html('<p>An error occurred: ' + response.message + '</p>');
                }
                $('.show-progress').hide();
            },
            error: function (xhr, status, error) {
                $('.show-progress').show();
                $messageContainer.html('<p>An error occurred. Please try again later.</p>');
            }
        });
    } else {
        // Handle case where ticketName or ticketStatus is missing
        $('#empTicketCommentsModal .modal-body').html('<p>Ticket information is missing. Please try again.</p>');
    }
});


$(document).on('click', '.emp-tickethistory-edit', function (event) {
    var ticketName = $(this).attr('data-ticketname');
    window.location.href = "/empticketing/empticketraise?ticketnumber=" + ticketName;
    return;

});

$(document).on('click', '.btn-closeticketing-refreshpage', function (event) {

    window.location.href = "/empticketing/index";
    return;

});






