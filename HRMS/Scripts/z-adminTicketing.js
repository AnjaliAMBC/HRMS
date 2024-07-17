$(document).on('click', '.admin-hr-ticketing-listing-title', function () {
    var ticketNum = $(this).attr("data-ticketnum");
    window.location.href = "/adminticketing/gethrticketdetails?ticketNo=" + ticketNum;
});

$(document).on('click', '.admin-it-ticketing-listing-title', function () {
    var ticketNum = $(this).attr("data-ticketnum");
    window.location.href = "/adminticketing/adminitticketopenclose";
});

$(document).on('click', '.btn-hrticketing-back', function () {
    window.location.href = "/adminticketing/hrticketing";
});

$(document).on('click', '.btn-apply-admin-hrticketing-submit', function (event) {
    event.preventDefault();
    var ticketModel =
    {
        TicketNo: $(this).data('ticketnum'),
        Status: $('#admin-hrticketing-status').val(),
        Resolved_by: $('#hrticketing-closedby').val(),
        isacknowledge: $('#hrticketing-closeddate').val(),
        ReopenedComments: $('#hrticketing-closeddate').val(),
        AcknowledgeComments: $('#hrticketing-closeddate').val(),
        Closedby: $('#hrticketing-closedby').val()
    }

    $.ajax({
        url: '/adminticketing/updateticketstatus',
        type: 'POST',
        data: {
            ticketModel: ticketModel
        },
        dataType: 'json',
        success: function (response) {
            if (response.success) {
                $('#modalMessage').text("Ticket " + $('#admin-hrticketing-status').val() + "  updated successfully.");
                $('#messageModal').modal('show');
            } else {
                $('#modalMessage').text("Error: " + response.message);
                $('#messageModal').modal('show');
            }
        },
        error: function (xhr, status, error) {
            $('#modalMessage').text("An error occurred: " + error);
            $('#messageModal').modal('show');
        }
    });
});




