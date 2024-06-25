$(document).on('click', '.adminleave-Balanve-link', function (event) {
    event.preventDefault();
    //HighlightAdminActiveLink($(this));
    $.ajax({
        url: '/adminleave/adminleavebalance',
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leaveBalance-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error("Error deleting employee:", error);
        }
    });
});


//leave balance update page 

$(document).on('click', '.employeeinfo-balance', function (event) {
    event.preventDefault();
    var employeeId = $(this).attr("data-empid");
    $.ajax({
        url: '/adminleave/adminleavebalanceupdate',
        type: 'GET',
        dataType: 'html',
        data: { selectedEmpID: employeeId },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leaveBalanceUpdate-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            console.error("Error deleting employee:", error);
        }
    });
});

//Admin Leave History of all employees 
$(document).on('click', '.adminleave-history', function (event) {
    event.preventDefault();
    $.ajax({
        url: '/adminleave/adminleavehistory',
        type: 'GET',
        dataType: 'html',
        data: { startdate: $('#leavehistory-fromDate').val(), endDate: $('#leavehistory-toDate').val(), department: "", location: $('#leavehistory-Location-dropdown').val(), status: $('#leavehistory-status-dropdown').val() },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leaveHistory-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            console.error("Error deleting employee:", error);
        }
    });
});



$(document).on('change', '#userSelect', function (event) {
    var employeeId = $(this).val();
    if (employeeId) {
        $.ajax({
            url: '/AdminLeave/AdminLeaveBalanceUpdatebyEmpID',
            type: 'GET',
            data: { selectedEmpID: employeeId },
            success: function (data) {
                var tableBody = $('#leaveBalanceTableBody');
                tableBody.empty();
                $.each(data.AvailableLeaves, function (index, leave) {
                    var row = '<tr>' +
                        '<td class="leaveupdate-type"><span class="' + leave.DashBoardColorCode + '">' + leave.ShortName + '</span> ' + leave.Type + '</td>' +
                        '<td><input type="number" class="form-control total-available-leave" value="' + leave.Available.toString().replace(".00", "") + '"></td>' +
                        '<td><input type="text" disabled class="form-control" value="' + leave.Booked.toString().replace(".00", "") + '"></td>' +
                        '<td><input type="text" disabled class="form-control" value="' + leave.Balance.toString().replace(".00", "") + '"></td>' +
                        '</tr>';
                    tableBody.append(row);
                });
            },
            error: function (xhr, status, error) {
                console.error('Error: ' + error);
            }
        });
    }
});


$(document).on('input', '.total-available-leave', function () {
    var $row = $(this).closest('tr');
    var totalAvailable = parseFloat($(this).val());
    var booked = parseFloat($row.find('input[disabled].form-control').first().val());

    var availableLeaves = totalAvailable - booked;
    $row.find('input[disabled].form-control').last().val(availableLeaves);
});



$(document).on('click', '#updateleavebalanceButton', function () {
    var leaveData = {};

    $('#leaveBalanceTableBody tr').each(function () {
        var $row = $(this);
        var leaveType = $row.find('.leaveupdate-type span').text();
        var totalAvailable = parseFloat($row.find('.total-available-leave').val()) || 0;

        switch (leaveType) {
            case 'EL':
                leaveData.Earned = totalAvailable;
                break;
            case 'EML':
                leaveData.Emergency = totalAvailable;
                break;
            case 'SL':
                leaveData.Sick = totalAvailable;
                break;
            case 'BL':
                leaveData.Bereavement = totalAvailable;
                break;
            case 'HP':
                leaveData.HourlyPermission = totalAvailable;
                break;
            case 'ML':
                leaveData.Marriage = totalAvailable;
                break;
            case 'MTL':
                leaveData.Maternity = totalAvailable;
                break;
            case 'PL':
                leaveData.Paternity = totalAvailable;
                break;
            case 'CO':
                leaveData.CompOff = totalAvailable;
                break;
        }
    });

    // Example EmpID and Year, replace with actual values
    leaveData.EmpID = $('#userSelect').val();

    $.ajax({
        url: '/AdminLeave/UpdateLeaveBalanceBasedonEmpID', // replace with your API endpoint
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(leaveData),
        success: function (response) {
            $('#modalMessage').text('Leave balance updated successfully.');
            $('#messageModal').modal('show');
        },
        error: function (error) {
            console.error('Error updating leave balance:', error);
            $('#modalMessage').text('Failed to update leave balance.');
            $('#messageModal').modal('show');
        }
    });
});


function exportLeaveBalance() {
    $('.show-progress').show();
    window.location.href = "/adminleave/exportleavebalance";
}


//adminleave Compoff page 

$(document).on('click', '.adminleave-CompOff-link', function (event) {
    event.preventDefault();
    //HighlightAdminActiveLink($(this));
    $.ajax({
        url: '/adminleave/adminleavecompensatoryoff',
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leave-compoff-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error("Error deleting employee:", error);
        }
    });
});


$(document).on('click', '.leave-export-history', function (event) {
    // Prevent the default action
    event.preventDefault();

    // Get the values of the date fields
    var fromDate = $('#leavehistory-fromDate').val();
    var toDate = $('#leavehistory-toDate').val();

    // Clear previous validation messages
    $('.validation-error').remove();
    $('#leavehistory-fromDate').removeClass('is-invalid');
    $('#leavehistory-toDate').removeClass('is-invalid');

    // Check if the dates are provided
    var isValid = true;
    if (!fromDate) {
        $('#leavehistory-fromDate').addClass('is-invalid');
        $('#leavehistory-fromDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
        isValid = false;
    }
    if (!toDate) {
        $('#leavehistory-toDate').addClass('is-invalid');
        $('#leavehistory-toDate').closest('.form-group').append('<span class="validation-error text-danger"></span>');
        /* $('#leavehistory-toDate').closest('.form-group').append('<span class="validation-error text-danger">To date is required</span>');*/
        isValid = false;
    }

    // If both dates are provided, proceed with the export
    if (isValid) {
        $('.show-progress').show();
        window.location.href = "/adminleave/exportleavedatatoexcel?startdate=" + fromDate + "&endDate=" + toDate + "&department=" + "" + "&location=" + $('#leavehistory-Location-dropdown').val() + "&status=" + $('#leavehistory-status-dropdown').val();
        $('.show-progress').hide();
    }
});


$(document).on('click', '.btn-totalleaves-import', function (event) {
    event.preventDefault();
  
    $.ajax({
        url: '/adminleave/AdminTotalLeavesImport',
        type: 'GET',
        dataType: 'html',      
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-totalleaveimport-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error("Error deleting employee:", error);
        }
    });
});


$(document).on('click', '.btn-importleave-submit', function (event) {
    var file = $('#leave-file-upload-input')[0].files[0];
    var formData = new FormData();
    formData.append('file', file);

    $.ajax({
        url: '/adminleave/uploadtotalleavesexcel',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                console.log(response.data); 
                $('#modalMessage').text(response.message);
                $('#messageModal').modal('show');
            } else {
                $('#modalMessage').text(response.message);
                $('#messageModal').modal('show');
            }
        },
        error: function (xhr, status, error) {
            $('#modalMessage').text('Error uploading file: ' + error);
            $('#messageModal').modal('show');           
        }
    });
});

function downloadLeaveTemplate() {
    var link = document.createElement('a');
    link.href = '/assets/templates/ToalEMpLeavesTemplate.xlsx';
    link.download = 'TotalEmpLeaveTemplate.xlsx';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

//AdminIndivisual Employee Leave History Calender 
$(document).on('click', '.AdminIndiEmpLeave-History', function (event) {
    event.preventDefault();
    $.ajax({
        url: '/adminleave/AdminEmpLeaveCalender',
        type: 'GET',
        dataType: 'html',
        data: { startdate: $('#leavehistory-fromDate').val(), endDate: $('#leavehistory-toDate').val(), department: "", location: $('#leavehistory-Location-dropdown').val(), status: $('#leavehistory-status-dropdown').val() },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".AdminLeaveEmpCalender-Page").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            console.error("Error deleting employee:", error);
        }
    });
});
