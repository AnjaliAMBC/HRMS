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
    /* HighlightAdminActiveLink($(this));*/
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
    //HighlightAdminActiveLink($(this));
    $.ajax({
        url: '/adminleave/adminLalleavehistory',
        type: 'GET',
        dataType: 'html',
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



