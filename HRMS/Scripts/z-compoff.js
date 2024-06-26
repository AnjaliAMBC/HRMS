$('.compoff-submit').click(function () {
    var employeeID = $('#CompemployeeName').val();
    var empName = $("#CompemployeeName option:selected").text();

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
            holidynumber: selectedHolidayNumber,
            holidaylocation: selectedHolidayLocation

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

$(document).on('click', '.clearLeave-compoff-filter', function () {
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
    let $row = $this.closest('.statusBtnFlex');
    $.ajax({
        url: '/adminleave/ChangeCompoffStatus', 
        type: 'POST',
        data: {
            compoffNum: compoffNum,
            status: 'Approved'
        },
        success: function (response) {           
            if (response.success == true) {
                //$('#statusLabel').text('Approved').show();
                $row.find('.statusLabel').text('Approved').show();
                $this.hide();
                $this.siblings('.rejectLeavecompoffBtn').hide();
            }

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
    let $row = $this.closest('.statusBtnFlex');
    $.ajax({
        url: '/adminleave/ChangeCompoffStatus', 
        type: 'POST',
        data: {
            compoffNum: compoffNum,
            status: 'Rejected'
        },
        success: function (response) {
           
            if (response.success == true) {
                $row.find('.statusLabel').text('Rejected').show();
                //$('#statusLabel').text('Rejected').show();
                $this.hide();
                $this.siblings('.acceptLeavecompoffBtn').hide();
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
});


