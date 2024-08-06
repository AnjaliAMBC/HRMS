$('.compoff-submit').click(function () {
    var employeeID = $('#CompemployeeName').val();
    var empName = $("#CompemployeeName option:selected").text();
    var empEmail = $("#CompemployeeName option:selected").attr("data-empemail");
    var empLocation = $("#CompemployeeName option:selected").attr("data-emplocation");

    var compOffDate = $('#Compdate').val();
    var reason = $('#Compreason').val();
    var submittedUser = $('.loggedinempname').text();
    var selectedholidayname = $('.selectedholidayname').text();
    var selectedHolidayNumber = $('.selectedholidaynumber').text();
    var selectedHolidayLocation = $('.selectedholidaylocation').text();

    // Validation
    if (!employeeID) {
        $('#CompemployeeName').addClass('is-invalid');
        if ($('#CompemployeeName').next('.invalid-feedback').length === 0) {
            $('#CompemployeeName').after('<div class="invalid-feedback">Employee Name is mandatory.</div>');
        }
        return;
    } else {
        $('#CompemployeeName').removeClass('is-invalid');
        $('#CompemployeeName').next('.invalid-feedback').remove();
    }

    $.ajax({
        type: "POST",
        url: '/Compoff/submitcompOff', // Adjust the URL to match your controller and action
        data: {
            employeeID: employeeID,
            empName: empName,
            compOffDate: compOffDate,
            reason: reason,
            submittedUser: submittedUser,
            holidayname: selectedholidayname,
            holidynumber: parseInt(selectedHolidayNumber),
            holidaylocation: empLocation,
            empEmail: empEmail
        },
        success: function (response) {
            if (response.StatusCode == 200) {
                $('#Compreason').val("");
                $('#compOffMessage').removeClass('alert-danger').addClass('alert-success').html(response.Message).show();
            } else {
                $('#compOffMessage').removeClass('alert-success').addClass('alert-danger').html(response.Message).show();
            }
        },
        error: function () {
            $('#compOffMessage').removeClass('alert-success').addClass('alert-danger').text('An error occurred while submitting Comp Off.').show();
        }
    });
});

$(document).on('click', '.btn-admin-compoff-history', function () {
    let fromDate = $('#fromDate').val();
    let toDate = $('#toDate').val();
    let isValid = true;

    if (!fromDate) {
        $('#fromDate').addClass('error');
        isValid = false;
    } else {
        $('#fromDate').removeClass('error');
    }

    if (!toDate) {
        $('#toDate').addClass('error');
        isValid = false;
    } else {
        $('#toDate').removeClass('error');
    }

    if (isValid) {
        window.location.href = '/adminleave/exportcompoffstoexcel?startDate=' + fromDate + "&endDate=" + toDate;
        //$.ajax({
        //    url: '/adminleave/exportcompoffstoexcel',
        //    type: 'POST',
        //    data: {
        //        startDate: fromDate,
        //        endDate: toDate
        //    },
        //    success: function (response) {
        //        console.log(response);
        //    },
        //    error: function (error) {
        //        console.log(error);
        //    }
        //});
    }
});

$(document).on('click', '.clearLeave-compoff-filter', function (e) {
    e.preventDefault();
    $('#fromDate').val('');
    $('#toDate').val('');
    $('#fromDateError').hide();
    $('#toDateError').hide();
});


$(document).on('click', '.acceptLeavecompoffBtn', function (e) {
    e.preventDefault();
    let compoffNum = $(this).data('compoffnum');
    let $this = $(this);
    let $statusBtnFlex = $this.closest('.statusBtnFlex');
    var employeeName = $(".loggedinempname").text();
    $.ajax({
        url: '/adminleave/ChangeCompoffStatus',
        type: 'POST',
        data: {
            compoffNum: compoffNum,
            status: 'Approved'
        },
        success: function (response) {
            //if (response.success == true) {
            //    var $statusLabel = $row.find('.statusLabel');
            //    $statusLabel.find('.statuschangedbyname').text(employeeName);

            //    $statusLabel.html(function (index, oldHtml) {
            //        return oldHtml.replace(/Cancelled|Approved|Rejected|Pending/, '<span style="color: #3E78CF;">Approved</span>');
            //    }).show();

            //    //$statusLabel.text('Approved').show();

            //    $row.find('.compoffdiv').show();

            //    /* $this.hide();*/
            //    $this.siblings('.rejectLeavecompoffBtn').hide();
            //}


            if (response.success == true) {
                /*  var $statusLabel = $('<small class="statusLabel">Rejected</small>');*/
                //$statusBtnFlex.find('.statusLabel').text('Rejected').show();
                ////$statusBtnFlex.append($statusLabel);
                //$statusBtnFlex.find('.statusBtn').hide();
                //$statusBtnFlex.find('.changestatusreject').hide();

                $statusBtnFlex.empty();

                if ($statusBtnFlex.find('.admin-leavehistoryoptions').length === 0) {
                    var adminCompoffApprovalOptions = `
                        <div id="statusLabel" class="statusLabel" style="color: #3E78CF">
                            <span style="color: #212529; margin-right: 5px;">${employeeName}</span>
                            Approved
                        </div>
                        <i class="fas fa-ellipsis-h adminleave-edit-history" onclick="toggleAdminLeaveHistoryActionOptions(this)"></i>
                        <div class="admin-leavehistoryoptions" style="display:none">
                            <a href="" class="dropdown-item rejectLeavecompoffBtn" data-leavename="Rejected" data-compoffnum="${compoffNum}">Change Status</a>
                        </div>
                    `;
                    $statusBtnFlex.append(adminCompoffApprovalOptions);
                }
            } else {
                alert('Failed to update the status. Please try again.');
            }
            $('.show-progress').hide();

        },
        error: function (error) {
            console.log(error);
        }
    });
});

$(document).on('click', '.rejectLeavecompoffBtn', function (e) {
    e.preventDefault();
    let compoffNum = $(this).data('compoffnum');
    let $this = $(this);
    let $statusBtnFlex = $this.closest('.statusBtnFlex');
    var employeeName = $(".loggedinempname").text();
    $.ajax({
        url: '/adminleave/ChangeCompoffStatus',
        type: 'POST',
        data: {
            compoffNum: compoffNum,
            status: 'Rejected'
        },
        success: function (response) {

            //if (response.success == true) {
            //    var $statusLabel = $row.find('.statusLabel');
            //    $statusLabel.find('.statuschangedbyname').text(employeeName);
            //    /* $statusLabel.text('Rejected').show();*/

            //    $statusLabel.html(function (index, oldHtml) {
            //        // Preserve the span while changing the text after it
            //        return oldHtml.replace(/Cancelled|Approved|Rejected/, 'Rejected');

            //    }).show();
            //    //$('#statusLabel').text('Rejected').show();
            //    $this.hide();
            //    $this.siblings('.acceptLeavecompoffBtn').hide();
            //}


            if (response.success == true) {
                /*  var $statusLabel = $('<small class="statusLabel">Rejected</small>');*/
                //$statusBtnFlex.find('.statusLabel').text('Rejected').show();
                ////$statusBtnFlex.append($statusLabel);
                //$statusBtnFlex.find('.statusBtn').hide();
                //$statusBtnFlex.find('.changestatusreject').hide();

                $statusBtnFlex.empty();

                if ($statusBtnFlex.find('.admin-leavehistoryoptions').length === 0) {
                    var adminCompoffApprovalOptions = `
                        <div id="statusLabel" class="statusLabel" style="color: red">
                            <span style="color: #212529; margin-right: 5px;">${employeeName}</span>
                            Rejected
                        </div>
                        <i class="fas fa-ellipsis-h adminleave-edit-history" onclick="toggleAdminLeaveHistoryActionOptions(this)"></i>
                        <div class="admin-leavehistoryoptions" style="display:none">
                            <a href="" class="dropdown-item acceptLeavecompoffBtn" data-leavename="Approved" data-compoffnum="${compoffNum}">Change Status</a>
                        </div>
                    `;
                    $statusBtnFlex.append(adminCompoffApprovalOptions);
                }
            } else {
                alert('Failed to update the status. Please try again.');
            }
            $('.show-progress').hide();
        },
        error: function (error) {
            console.log(error);
        }
    });
});


$(document).on('click', '.cancelLeavecompoffBtn', function (e) {
    e.preventDefault();
    let compoffNum = $(this).data('compoffnum');
    var $btn = $(this);

    $.ajax({
        url: '/adminleave/ChangeCompoffStatus',
        type: 'POST',
        data: {
            compoffNum: compoffNum,
            status: 'Cancelled'
        },
        success: function (response) {
            if (response.success) {
                // Find the closest tr and update the status
                $btn.closest('tr').find('.compoff-status span').html('<b>Cancelled</b>');
                $btn.closest('tr').find('.compoff-actions').hide();
            } else {
                console.error('Failed to cancel compensatory off');
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
});

