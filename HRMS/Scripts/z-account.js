(function () {
    window.onpageshow = function (event) {
        if (event.persisted) {
            window.location.reload();
        }
    };
});

function ResetLoginFields() {
    $("#EmployeeID").val("");
    $("#EmailID").val("");
    $("#Password").val("");
    $('.error-message').html("");
    $(".text-danger").text("");
}

//Login
$(".admin-login").on("click", function (event) {
    event.preventDefault;
    ResetLoginFields();

    $('.emp-login').show();
    $(".admin-login").hide();
    $('.empid-field').hide();
    $("#EmployeeID").val("");
    $("#EmailID").val("");
    $('.empemail-field').show();

});

$(".emp-login").on("click", function (event) {
    event.preventDefault;
    ResetLoginFields();

    $(".admin-login").show();
    $(".admin-login").show();
    $(".emp-login").hide();
    $('.empid-field').show();
    $("#EmployeeID").val("");
    $("#EmailID").val("");
    $('.empemail-field').hide();

});

$(document).ready(function () {
    $('.login-continue').click(function (event) {
        $(".text-danger").text("");
        var isValid = true;

        
        var empID = $("#EmployeeID").val();
        var email = $("#EmailID").val();
        var password = $("#Password").val();

      
        if ($("#EmployeeID").is(":visible") && empID === "") {
            $("#empIDError").text("Please enter your Employee ID");
            isValid = false;
        }
        if ($("#EmailID").is(":visible") && email === "") {
            $("#emailError").text("Please enter your Email ID");
            isValid = false;
        }
        if (password === "") {
            $("#passwordError").text("Please enter your password");
            isValid = false;
        }

        if (!isValid) {
            event.preventDefault(); 
            $('.show-progress').hide(); 
        } else {
            var data = {
                EmployeeID: $('#EmployeeID').val(),
                EmailID: $('#EmailID').val(),
                Password: $('#Password').val(),
                StaySignedIn: $('#StaySignedIn').is(':checked'),
            };

            $.ajax({
                type: 'POST',
                url: '/account/login',
                beforeSend: function () {
                    $('.show-progress').show(); 
                },
                data: data,
                success: function (data) {                  
                    $('.error-message').html("");

                    if (data.IsValidUser) {
                        if (data.IsAdmin) {
                            window.location.href = "/admindash/index";
                            return;
                        }

                        if (data.IsITAdmin) {
                            window.location.href = "/adminticketing/itticketing";
                            return;
                        }

                        if (data.IsUser) {
                            window.location.href = "/empDash/index";
                            return;
                        }

                        if (data.IsAccountAdmin) {
                            window.location.href = "/vendor/index";
                            return;
                        }

                        if (data.IsHiringAdmin) {
                            window.location.href = "/admindashboard/adminjoblisting";
                            return;
                        }

                        if (data.IsTimeAdmin) {
                            window.location.href = "/timesheet/admintimesheet";
                            return;
                        }
                    } else {
                        $('.show-progress').hide();
                        $('.error-message').show();
                        $('.error-message').html(data.InvalidLoginMessage);
                    }
                },
                error: function (xhr, status, error) {
                    $('.show-progress').hide(); 
                    var err = eval("(" + xhr.responseText + ")");
                    $('.error-message').show();
                    $('.error-message').html("An error occurred: " + err.Message);
                }
            });
        }
    });
});


//Forgot password
$('.forgot-password').click(function (e) {
    $('.empemail-field').show();
    $('.login-container').hide();
    $('.forgot-container').show();
});

$('.forgot-continue').click(function (event) {

    $('.response-message').hide();

    // Reset error messages
    $(".text-danger").text("");

    var isValid = true;
    var email = $("#ForgotEmailID").val();

    // Validation logic

    if ($("#ForgotEmailID").is(":visible") && email === "") {
        $("#forgotemailError").text("Please enter your Email ID");
        isValid = false;
    }

    if (!isValid) {
        event.preventDefault(); // Prevent form submission
    }
    else {
        var data = {
            ForgotEmailID: $('#ForgotEmailID').val(),
        };
        $.ajax({
            type: 'POST',
            url: '/account/forgotpassword',
            data: data,
            beforeSend: function () {
                $(this).prop('disabled', true);
                $('#ajax-overlay').show();
            },
            complete: function () {
                // This function will be called when the AJAX request completes (success, error, or complete)
                $(this).prop('disabled', false);
                $('#ajax-overlay').hide();
            },
            success: function (data) {
                if (data.IsEmailSent == true) {
                    $(this).prop('disabled', false);
                    $('#ajax-overlay').hide();
                    $('.response-message').show();
                    $('.response-message-innermessage').css('color', '#2E8B57');
                    $('.response-message-innermessage').html(data.JsonResponse.Message);
                }
                else {
                    $('.response-message-innermessage').html(data.JsonResponse.Message);
                    $('.response-message').show();
                    $('.response-message-innermessage').css('color', 'red');
                }
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                // Handle error response
            }
        });
    }
});

$('.confirmPassword-continue').click(function (event) {
    // Reset any previous error messages
    $(".text-danger").text("");

    // Retrieve input field values
    var password = $("#resetPassword").val().trim();
    var confirmPassword = $("#resetconfirmPassword").val().trim();

    // Validation logic
    var isValid = true;

    if (password == "") {
        $('#resetpasswordError').text("Please enter password");
        isValid = false;
    }

    if (confirmPassword == "") {
        $('#resetrepasswordError').text("Please re-enter password");
        isValid = false;
    }
    if (password == null) {
        $("#resetpasswordError").text("Passwords enter password");
    }
    // Check if passwords match
    if (password !== confirmPassword) {
        $("#resetpasswordError1").text("Passwords do not match");
        isValid = false;
    }

    // Check if passwords meet required criteria (e.g., minimum length)
    //if (password.length < 8) {
    //    $("#resetpasswordError").text("Password must be at least 8 characters long");
    //    isValid = false;
    //}


    // If all validations pass, continue with form submission
    if (isValid) {
        var data = {
            Password: $('#resetPassword').val(),
            Empid: $('.reset-empid').html()
        };

        $.ajax({
            type: 'POST',
            url: '/account/UpdatePassword',
            data: data,
            success: function (data) {
                $(".confirmPassword-message").html("<b>" + data.ResponseMessage + "</b1><br><br>");
                $(".confirmPassword-message").show();
                $('.login-heading').hide();
                $('.reset-form').hide();
                $('.backtologin-btn').show();
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
                // Handle error response
            }
        });
    }
    else {
        event.preventDefault();
    }
});


$('.backtologin-btn').click(function (event) {
    window.location.href = "/account/login";
});
    //end forgot password