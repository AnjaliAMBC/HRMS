$('.compoff-submit').click(function () {
    var employeeID = $('#CompemployeeName').val();  
    var empName = $("#CompemployeeName option:selected").text();

    var compOffDate = $('#Compdate').val();
    var reason = $('#Compreason').val();
    var submittedUser = $('.loggedinempname').text();
    var selectedholidayname = $('.selectedholidayname').text();
    $.ajax({
        type: "POST",
        url: '/Compoff/submitcompOff', // Adjust the URL to match your controller and action
        data: {
            employeeID: employeeID,
            empName: empName,
            compOffDate: compOffDate,
            reason: reason,
            submittedUser: submittedUser,
            holidayname: selectedholidayname

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