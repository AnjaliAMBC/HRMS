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

//$('.btn-apply-admin-hrticketing-submit').on('click', function (e) {
//    event.preventDefault();
//    window.location.href = "/adminticketing/hrticketing";
//});




$(document).on('click', '.btn-apply-admin-hrticketing-submit', function (event) {
    event.preventDefault();
    var ticketModel =
    {
        TicketNo: $(this).data('ticketnum'),
        Status: $('#admin-hrticketing-status').val(),
        resolvedBy: $('#hrticketing-closedby').val(),
        isacknowledge: $('#hrticketing-closeddate').val(),
        ReopenComments: $('#hrticketing-closeddate').val(),
        closureComments: $('#hrticketing-closeddate').val(),
        Closedby: "",
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
                alert("Ticket status updated successfully.");
            } else {
                alert("Error: " + response.message);
            }
        },
        error: function (xhr, status, error) {
            alert("An error occurred: " + error);
        }
    });
});




