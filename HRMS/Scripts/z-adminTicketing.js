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


$(document).on('click', '.hrticketlisting-export', function (event) {
    event.preventDefault();

    var fromDate = $('#hrticketlisting-fromDate').val();
    var toDate = $('#hrticketlisting-toDate').val();
    var status = $('#hr-ticketlisting-status-dropdown').val();
    var location = $('#hr-ticketlisting-location-dropdown').val();
    var closedBy = $('#hr-ticketlisting-closedby-dropdown').val();


    $('.validation-error').remove();
    $('#hrticketlisting-fromDate').removeClass('is-invalid');
    $('#hrticketlisting-toDate').removeClass('is-invalid');

    var isValid = true;
    if (!fromDate) {
        $('#hrticketlisting-fromDate').addClass('is-invalid');
        $('#hrticketlisting-fromDate').closest('.form-group').append('<span class="validation-error text-danger">From date is required</span>');
        isValid = false;
    }
    if (!toDate) {
        $('#hrticketlisting-toDate').addClass('is-invalid');
        $('#hrticketlisting-toDate').closest('.form-group').append('<span class="validation-error text-danger">To date is required</span>');
        isValid = false;
    }

    if (isValid) {
        $('.show-progress').show();
        var url = "/adminticketing/hrexporttoexcel?" +
            "fromDate=" + fromDate +
            "&toDate=" + toDate +
            "&status=" + status +
            "&location=" + location +
            "&closedBy=" + closedBy;

        window.location.href = url;
        $('.show-progress').hide();
    }
});

$(document).on('click', '.hrticketlisting-view', function (event) {
    event.preventDefault();

    var fromDate = $('#hrticketlisting-fromDate').val();
    var toDate = $('#hrticketlisting-toDate').val();
    var status = $('#hr-ticketlisting-status-dropdown').val();
    var location = $('#hr-ticketlisting-location-dropdown').val();
    var closedBy = $('#hr-ticketlisting-closedby-dropdown').val();

    // Clear previous validation messages
    $('.validation-error').remove();
    $('#hrticketlisting-fromDate').removeClass('is-invalid');
    $('#hrticketlisting-toDate').removeClass('is-invalid');

    // Check if the dates are provided
    var isValid = true;
    if (!fromDate) {
        $('#hrticketlisting-fromDate').addClass('is-invalid');
        $('#hrticketlisting-fromDate').closest('.form-group').append('<span class="validation-error text-danger">From date is required</span>');
        isValid = false;
    }
    if (!toDate) {
        $('#hrticketlisting-toDate').addClass('is-invalid');
        $('#hrticketlisting-toDate').closest('.form-group').append('<span class="validation-error text-danger">To date is required</span>');
        isValid = false;
    }

    // If both dates are provided, proceed with the export
    if (isValid) {
        $('.show-progress').show();
        $.ajax({
            url: '/adminticketing/gethrticketfilter',
            type: 'GET',
            data: {
                fromDate: fromDate,
                toDate: toDate,
                status: status,
                location: location,
                closedBy: closedBy
            },
            success: function (data) {
                var html = '';

                var response = $.parseJSON(data);

                $.each(response, function (index, ticket) {
                    var responseTimeHtml = '';

                    if (ticket.ResponseTime != null) {
                        // Parse the TimeSpan string (hh:mm:ss.ffffff)
                        var timeParts = ticket.ResponseTime.split(':');
                        var totalHours = parseInt(timeParts[0]);
                        var totalMinutes = parseInt(timeParts[1]);

                        responseTimeHtml = '<p>Response Time: ' + totalHours.toString().padStart(2, '0') + ':' + totalMinutes.toString().padStart(2, '0') + ' Hr</p>';
                    }

                    html += '<tr>';
                    html += '<td style="width: 400px">';
                    html += '<div class="res-admin-hr-ticketlisting-user-details" style="display: flex; align-items: center;">';
                    html += '<i class="hr-ticket-msg-icon fa-regular fa-message"></i>';
                    html += '<span style="margin-top: 0px; margin-right: 10px;">';
                    html += '<div class="admin-hr-ticketing-listing-title" data-ticketnum="' + ticket.TicketNo + '">' + ticket.Subject + '</div>';
                    html += '<div class="admin-hr-ticketing-listing-details">';
                    html += '<span class="hr-ticket-id">' + ticket.TicketNo + '</span>';
                    html += '<span class="hr-ticket-info"><i class="fa-solid fa-folder" aria-hidden="true"></i> ' + ticket.Category + '</span>';
                    html += '<span class="id-name">' + ticket.EmployeeID + ' ' + ticket.EmployeeName + '</span>';
                    html += '</div>';
                    html += '</span>';
                    html += '</div>';
                    html += '</td>';
                    html += '<td style="width: 400px" class="res-admin-hr-ticketlisting-priority">';
                    html += '<div class="hr-ticketing-response-time">';
                    html += responseTimeHtml;
                    html += '</div>';
                    html += '<div class="hr-ticketing-priority-date">';
                    html += '<span class="hr-ticket-prioritylevel">';
                    html += '<span class="res-hr-ticketlisting-color res-hr-ticketlisting-color-red"></span>';
                    html += '<span class="res-hr-ticketlisting-priority-status">' + ticket.Priority + '</span>';
                    html += '</span>';
                    html += '<span class="res-admin-hr-ticketlisting-date">';
                    html += '<i class="fa-solid fa-clock" aria-hidden="true"></i>';
                    html += '<span class="res-ticketlisting-priority-status">';
                    html += (ticket.Created_date ? new Date(ticket.Created_date).toLocaleString('en-US', { weekday: 'short', year: 'numeric', month: 'short', day: '2-digit', hour: 'numeric', minute: 'numeric', hour12: true }) : 'N/A');
                    html += '</span>';
                    html += '</span>';
                    html += '</div>';
                    html += '</td>';
                    html += '<td class="res-admin-ticketlisting-location-info">';
                    html += '<div class="hr-ticketing-empty-block"></div>';
                    html += '<div class="res-ticketlisting-location">' + ticket.Location + '</div>';
                    html += '</td>';
                    html += '<td class="res-admin-ticketlisting-status">';
                    html += '<div class="hr-ticketing-empty-block"></div>';
                    html += '<div class="res-ticketlisting-status-level ticket-status-' + ticket.Status + '">' + ticket.Status + '</div>';
                    html += '</td>';
                    html += '</tr>';
                });

                // Clear the table body and destroy the existing DataTable
                var table = $('#adminhrticketlistingTable').DataTable();
                table.clear().destroy();

                // Update the table body with new content
                $('#adminhrticketlistingTable tbody').html(html);

                // Re-initialize the DataTable after the table body is updated
                table = $('#adminhrticketlistingTable').DataTable({
                    "paging": true,
                    "searching": true,
                    "ordering": false,
                    "info": true,
                    "autoWidth": false,
                    "lengthMenu": [[7, 14, 21, -1], [7, 14, 21, "All"]],
                    "columnDefs": [
                        { "orderable": false, "targets": 1 },
                        { "orderable": false, "targets": 3 }
                    ]
                });

                $(document).on('keyup', '.hr-search', function () {
                    table.search(this.value).draw(); // Search value across all columns and redraw table
                });

                $('.show-progress').hide();

            },
            error: function (xhr, status, error) {
                $('.show-progress').hide();
            }
        });

    }
});






