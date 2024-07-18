$(document).on('click', '.admin-hr-ticketing-listing-title', function () {
    var ticketNum = $(this).attr("data-ticketnum");
    window.location.href = "/adminticketing/gethrticketdetails?ticketNo=" + ticketNum;
});

$(document).on('click', '.admin-it-ticketing-listing-title', function () {
    var ticketNum = $(this).attr("data-ticketnum");
    window.location.href = "/adminticketing/getitticketdetails?ticketNo=" + ticketNum;
});

$(document).on('click', '.btn-hrticketing-back', function () {
    window.location.href = "/adminticketing/hrticketing";
});

$(document).on('click', '.btn-itticketing-back', function () {
    window.location.href = "/adminticketing/itticketing";
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
        $('#hrticketlisting-fromDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
        isValid = false;
    }
    if (!toDate) {
        $('#hrticketlisting-toDate').addClass('is-invalid');
        $('#hrticketlisting-toDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
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
        $('#hrticketlisting-fromDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
        isValid = false;
    }
    if (!toDate) {
        $('#hrticketlisting-toDate').addClass('is-invalid');
        $('#hrticketlisting-toDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
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
                        // Convert the response time from seconds to hours, minutes, and seconds
                        var totalSeconds = ticket.ResponseTime;
                        var totalHours = Math.floor(totalSeconds / 3600);
                        var totalMinutes = Math.floor((totalSeconds % 3600) / 60);

                        responseTimeHtml = 'Response Time ' + totalHours.toString().padStart(2, '0') + ':' + totalMinutes.toString().padStart(2, '0') + ' Hr';
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


$(document).on('click', '.btn-itticketing-back', function () {
    window.location.href = "/adminticketing/itticketing";
});


// it ticket Submit 
$(document).on('click', '.btn-apply-admin-itticket-submit', function (event) {
    event.preventDefault();

    var ticketModel = {
        TicketNo: $(this).data('ticketnum'),
        Status: $('#admin-itticket-status').val(),
        Resolved_by: $('#admin-itticket-closedby').val(),
        ResolvedByName: $('#admin-itticket-closedby option:selected').text(),
        Closed_date: $('#admin-itticketing-closeddate').val(),
        ResolvedDate: $('#admin-itticketing-closeddate').val(),
        ReopenedDate: $('#admin-itticketing-closeddate').val(),
        Closedby: $('#admin-itticket-closedby').val(),
        ClosedByName: $('#admin-itticket-closedby option:selected').text()
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
                $('#modalMessage').text("Ticket " + $('#admin-itticket-status').val() + " updated successfully.");
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



$(document).on('click', '.itticketlisting-export', function (event) {
    event.preventDefault();

    var fromDate = $('#itticketlisting-fromDate').val();
    var toDate = $('#itticketlisting-toDate').val();
    var status = $('#it-ticketlisting-status-dropdown').val();
    var location = $('#it-ticketlisting-location-dropdown').val();
    var closedBy = $('#it-ticketlisting-closedby-dropdown').val();


    $('.validation-error').remove();
    $('#itticketlisting-fromDate').removeClass('is-invalid');
    $('#itticketlisting-toDate').removeClass('is-invalid');

    var isValid = true;
    if (!fromDate) {
        $('#itticketlisting-fromDate').addClass('is-invalid');
        $('#itticketlisting-fromDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
        isValid = false;
    }
    if (!toDate) {
        $('#itticketlisting-toDate').addClass('is-invalid');
        $('#itticketlisting-toDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
        isValid = false;
    }

    if (isValid) {
        $('.show-progress').show();
        var url = "/adminticketing/itexporttoexcel?" +
            "fromDate=" + fromDate +
            "&toDate=" + toDate +
            "&status=" + status +
            "&location=" + location +
            "&closedBy=" + closedBy;

        window.location.href = url;
        $('.show-progress').hide();
    }
});




$(document).on('click', '.itticketlisting-view', function (event) {
    event.preventDefault();

    var fromDate = $('#itticketlisting-fromDate').val();
    var toDate = $('#itticketlisting-toDate').val();
    var status = $('#it-ticketlisting-status-dropdown').val();
    var location = $('#it-ticketlisting-location-dropdown').val();
    var closedBy = $('#it-ticketlisting-closedby-dropdown').val();


    $('.validation-error').remove();
    $('#itticketlisting-fromDate').removeClass('is-invalid');
    $('#itticketlisting-toDate').removeClass('is-invalid');

    var isValid = true;
    if (!fromDate) {
        $('#itticketlisting-fromDate').addClass('is-invalid');
        $('#itticketlisting-fromDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
        isValid = false;
    }
    if (!toDate) {
        $('#itticketlisting-toDate').addClass('is-invalid');
        $('#itticketlisting-toDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
        isValid = false;
    }


    // If both dates are provided, proceed with the export
    if (isValid) {
        $('.show-progress').show();
        $.ajax({
            url: '/adminticketing/getitticketfilter',
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

                    var colorClass = "";
                    if (ticket.Priority == "Mid") {
                        colorClass = "res-it-ticketlisting-color-orange";
                    }
                    if (ticket.Priority == "High") {
                        colorClass = "res-it-ticketlisting-color-red";
                    }
                    if (ticket.Priority == "Low") {
                        colorClass = "res-it-ticketlisting-color-red";
                    }


                    if (ticket.ResponseTime != null) {
                        // Convert the response time from seconds to hours, minutes, and seconds
                        var totalSeconds = ticket.ResponseTime;
                        var totalHours = Math.floor(totalSeconds / 3600);
                        var totalMinutes = Math.floor((totalSeconds % 3600) / 60);

                        responseTimeHtml = 'Response Time ' + totalHours.toString().padStart(2, '0') + ':' + totalMinutes.toString().padStart(2, '0') + ' Hr';
                    }

                    html += '<tr>';
                    html += '<td style="width: 400px">';
                    html += '<div class="res-admin-it-ticketlisting-user-details" style="display: flex; align-items: center;">';
                    html += '<i class="it-ticket-msg-icon fa-regular fa-message"></i>';
                    html += '<span style="margin-top: 0px; margin-right: 10px;">';
                    html += '<div class="admin-it-ticketing-listing-title" data-ticketnum="' + ticket.TicketNo + '">' + ticket.Subject + '</div>';
                    html += '<div class="admin-it-ticketing-listing-details">';
                    html += '<span class="it-ticket-id">#' + ticket.TicketNo + '</span>';
                    html += '<span class="it-ticket-info"><i class="fa-solid fa-folder" aria-hidden="true"></i> ' + ticket.Category + '</span>';
                    html += '<span class="id-name">' + ticket.EmployeeID + ' ' + ticket.EmployeeName + '</span>';
                    html += '</div>';
                    html += '</span>';
                    html += '</div>';
                    html += '</td>';
                    html += '<td style="width: 400px" class="res-admin-it-ticketlisting-priority">';
                    html += '<div class="it-ticketing-response-time">';
                    html += responseTimeHtml + '</div>';
                    html += '<div class="it-ticketing-priority-date">';
                    html += '<span class="it-ticket-prioritylevel">';
                    html += '<span class="res-it-ticketlisting-color ' + colorClass + '"></span>';
                    html += '<span class="res-it-ticketlisting-priority-status">' + ticket.Priority + '</span>';
                    html += '</span>';
                    html += '<span class="res-admin-it-ticketlisting-date">';
                    html += '<i class="fa-solid fa-clock" aria-hidden="true"></i>';
                    html += '<span class="res-itticketlisting-priority-status">';
                    html += (ticket.Created_date ? new Date(ticket.Created_date).toLocaleString('en-US', { day: '2-digit', month: 'short', year: 'numeric', weekday: 'short', hour: 'numeric', minute: 'numeric', hour12: true }) : 'N/A');
                    html += '</span>';
                    html += '</span>';
                    html += '</div>';
                    html += '</td>';
                    html += '<td class="res-admin-itticketlisting-location-info">';
                    html += '<div class="it-ticketing-empty-block"></div>';
                    html += '<div class="res-itticketlisting-location">' + ticket.Location + '</div>';
                    html += '</td>';
                    html += '<td class="res-admin-itticketlisting-status">';
                    html += '<div class="it-ticketing-empty-block"></div>';
                    html += '<div class="res-itticketlisting-status-level ticket-status-' + ticket.Status + '" onclick="adminitticketcommentpopup($(this))">' + ticket.Status + '</div>';
                    html += '</td>';
                    html += '</tr>';
                });

                // Clear the table body and destroy the existing DataTable
                var table = $('#adminitticketlistingTable').DataTable();
                table.clear().destroy();

                // Update the table body with new content
                $('#adminitticketlistingTable tbody').html(html);

                // Re-initialize the DataTable after the table body is updated
                table = $('#adminitticketlistingTable').DataTable({
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

                $(document).on('keyup', '.it-search', function () {
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

$(document).on('click', '.res-itticketlisting-status-level', function (event) {
    event.preventDefault();
    var ticketNo = $(this).closest('tr').find('.admin-it-ticketing-listing-title').data('ticketnum');

    $.ajax({
        url: '/adminticketing/getticketdetailsbynumber?ticketNo=' + ticketNo,
        method: 'GET',
        success: function (data) {
            var modalBody = $('#adminItTicketCommentsModal .modal-body .admin-it-ticketing-commentbox-popup-list');
            populateModal(data, modalBody);
            $('#adminItTicketCommentsModal').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    });
});

function populateModal(ticketdata, modalBody) {

    modalBody.empty();

    // Determine which details to display based on ticket status
    var html = '';
    var ticket = $.parseJSON(ticketdata);

    // Add HTML for comment box and buttons specific to Resolved status
    html += `
           <div class="modal-header">
                 <h5 class="modal-title">Ticket Flow</h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                     <span aria-hidden="true">&times;</span>
              </button>
           </div>
        `;

    // Add HTML for comment box and buttons specific to Resolved status
    if (ticket.Status == "Resolved") {
        html += `
            <div class="emp-ticketing-commentbox-popup">
                <div class="form-group">
                    <label for="emp-tickethistory-comment-label">Comment</label>
                    <textarea class="form-control" id="emp-tickethistory-comments"></textarea>
                </div>
                <div class="emp-tickethistory-comment-submit">
                    <button type="button" class="btn btn-apply-emp-tickethistory-comment-reopen emp-ticket-status-change mb-4" data-ticketname="${ticket.TicketNo}" data-status="Re-Open">Re-Open</button>
                    <button type="button" class="btn btn-primary btn-apply-emp-tickethistory-comment-submitbtn emp-ticket-status-change mb-4" data-ticketname="${ticket.TicketNo}" data-status="Closed">Acknowledge</button>
                </div>
            </div>
            <div class="emp-ticketing-statuschange-div">
            </div>
        `;
    }


    let formattedOpenDate = ticket.Created_date ? new Date(ticket.Created_date).toLocaleDateString('en-GB') : 'N/A';


    // Initial details always shown
    html += `
        <div class="admin-it-ticketing-commentbox-list">
            <div class="col-lg-1 admin-it-ticketing-commentbox-left">
                <img class="userIcon" src="/Assets/EmpImages/${ticket.EmployeeID}.jpeg">
                <div class="vl"></div>
            </div>
            <span class="col-lg-7 admin-it-ticketing-commentbox-mid">
                <div class="admin-it-ticketing-commentbox-userinfo">${ticket.EmployeeID} ${ticket.EmployeeName}</div>
                <div class="admin-it-ticketing-commentbox-status">Open</div>
                <div class="admin-it-ticketing-commentbox-desc" title="${ticket.Subject}">${ticket.Subject}</div>
            </span>
            <span class="col-lg-2 admin-it-ticketing-commentbox-right">
                <div class="admin-it-ticketing-commentbox-date">${formattedOpenDate}</div>
            </span>
        </div>
    `;

    // Additional details based on ticket status
    if (ticket.ResolvedDate !== null) {
        let formattedResolvedDate = ticket.ResolvedDate ? new Date(ticket.ResolvedDate).toLocaleDateString('en-GB') : 'N/A';
        html += `
            <div class="admin-it-ticketing-commentbox-list">
                <div class="col-lg-1 admin-it-ticketing-commentbox-left">
                    <img class="userIcon" src="/Assets/EmpImages/${ticket.Resolved_by}.jpeg">
                    <div class="vl"></div>
                </div>
                <span class="col-lg-7 admin-it-ticketing-commentbox-mid">
                    <div class="admin-it-ticketing-commentbox-userinfo">${ticket.Resolved_by} ${ticket.ResolvedByName}</div>
                    <div class="admin-it-ticketing-commentbox-status">Resolved</div>
                </span>
                <span class="col-lg-2 admin-it-ticketing-commentbox-right">
                    <div class="admin-it-ticketing-commentbox-date">${formattedResolvedDate}</div>
                </span>
            </div>
        `;
    }

    if (ticket.ReopenedDate !== null) {
        let formattedReOpenDate = ticket.ReopenedDate ? new Date(ticket.ReopenedDate).toLocaleDateString('en-GB') : 'N/A';
        html += `
            <div class="admin-it-ticketing-commentbox-list">
                <div class="col-lg-1 admin-it-ticketing-commentbox-left">
                    <img class="userIcon" src="/Assets/EmpImages/${ticket.EmployeeID}.jpeg">
                    <div class="vl"></div>
                </div>
                <span class="col-lg-7 admin-it-ticketing-commentbox-mid">
                    <div class="admin-it-ticketing-commentbox-userinfo">${ticket.EmployeeID} ${ticket.EmployeeName}</div>
                    <div class="admin-it-ticketing-commentbox-status">Re-Open</div>
                    <div class="admin-it-ticketing-commentbox-desc">${ticket.ReopenedComments}</div>
                </span>
                <span class="col-lg-2 admin-it-ticketing-commentbox-right">
                    <div class="admin-it-ticketing-commentbox-date">${formattedReOpenDate}</div>
                </span>
            </div>
        `;
    }

    if (ticket.Closed_date !== null) {
        let formattedClosedDate = ticket.Closed_date ? new Date(ticket.Closed_date).toLocaleDateString('en-GB') : 'N/A';
        html += `
        <div class="admin-it-ticketing-commentbox-list">
            <div class="col-lg-1 admin-it-ticketing-commentbox-left">
                <img class="userIcon" src="/Assets/EmpImages/${ticket.Closedby}.jpeg">
                <div class="vl"></div>
            </div>
            <span class="col-lg-7 admin-it-ticketing-commentbox-mid">
                <div class="admin-it-ticketing-commentbox-userinfo">${ticket.Closedby} ${ticket.ResolvedByName}</div>
                <div class="admin-it-ticketing-commentbox-status">Closed</div>
                <div class="admin-it-ticketing-commentbox-desc">${ticket.AcknowledgeComments !== null ? ticket.AcknowledgeComments : 'NA'}</div>
                ${ticket.isacknowledge != null ? '<div><input type="checkbox" checked disabled> Acknowledged</div>' : '<div><input type="checkbox" disabled> Acknowledged</div>'}
            </span>
            <span class="col-lg-2 admin-it-ticketing-commentbox-right">
                <div class="admin-it-ticketing-commentbox-date">${formattedClosedDate}</div>
            </span>
        </div>
    `;
    }


    modalBody.append(html);
}


$(document).on('click', '.res-ticketlisting-status-level', function (event) {
    event.preventDefault();
    var ticketNo = $(this).closest('tr').find('.admin-hr-ticketing-listing-title').data('ticketnum');

    $.ajax({
        url: '/adminticketing/getticketdetailsbynumber?ticketNo=' + ticketNo,
        method: 'GET',
        success: function (data) {
            var modalBody = $('#adminHrTicketCommentsModal .modal-body .admin-hr-ticketing-commentbox-popup-list');
            populateModal(data, modalBody);
            $('#adminHrTicketCommentsModal').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    });
});


