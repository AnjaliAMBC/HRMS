$('.compoff-submit').click(function () {
    var employeeID = $('#CompemployeeName').val();
    var compOffDate = $('#Compdate').val();
    var reason = $('#Compreason').val();

    $.ajax({
        type: "POST",
        url: '@Url.Action("SubmitCompOff", "CompOff")', // Adjust the URL to match your controller and action
        data: {
            employeeID: employeeID,
            compOffDate: compOffDate,
            reason: reason
        },
        success: function (response) {
            if (response.success) {
                $('#compOffMessage').removeClass('alert-danger').addClass('alert-success').text(response.message).show();
            } else {
                $('#compOffMessage').removeClass('alert-success').addClass('alert-danger').text(response.message).show();
            }
        },
        error: function () {
            $('#compOffMessage').removeClass('alert-success').addClass('alert-danger').text('An error occurred while submitting Comp Off.').show();
        }
    });
});