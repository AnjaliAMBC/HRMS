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
    if (grecaptcha != undefined) {
        grecaptcha.ready(function () {
            if (grecaptcha != undefined) {
                grecaptcha.reset();              
            }
        });
    }
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

        grecaptcha.ready(function () {
            grecaptcha.execute('6LfVc7wpAAAAAOjHUeyjy_FGS6-gyCmfS_TXDdvk', { action: 'submit' }).then(function (token) {
                // Add the reCAPTCHA response token to a hidden input field
                document.getElementById("g-recaptcha-response").value = token;
            });
            grecaptcha.reset();
        });


        // Reset error messages
        $(".text-danger").text("");

        var isValid = true;

        // Fetch input values
        var empID = $("#EmployeeID").val();
        var email = $("#EmailID").val();
        var password = $("#Password").val();

        // Validation logic
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
            event.preventDefault(); // Prevent form submission
        }
        else {
            var data = {
                EmployeeID: $('#EmployeeID').val(),
                EmailID: $('#EmailID').val(),
                Password: $('#Password').val(),
                StaySignedIn: $('#StaySignedIn').is(':checked'),
                GCaptcha: document.getElementById("g-recaptcha-response").value
            };
            $.ajax({
                type: 'POST',
                url: '/account/login',
                data: data,               
                success: function (data) {
                    $('.error-message').html("");
                    /* $window.grecaptcha.reset();*/

                    if (data.IsValidUser != "") {
                        $('.error-message').hide();
                        window.location.href = "/EmployeeAdmin/index";;
                    }
                    else {
                        $('.error-message').show();
                        $('.error-message').html(data.InvalidLoginMessage);
                        document.getElementById('g-recaptcha-response').value = '';
                        grecaptcha.reset();
                    }
                },
                error: function (xhr, status, error) {
                    var err = eval("(" + xhr.responseText + ")");
                    // Handle error response
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
            url: '/account/ForgotPassword',
            data: data,
            success: function (data) {
                if (data.IsEmailSent == true) {
                    $(this).prop('disabled', false);
                    $('#ajax-overlay').hide();
                    $('.response-message').show();
                }
                else {
                    $('.response-message').hide();
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

    // Check if passwords match
    if (password !== confirmPassword) {
        $("#resetpasswordError").text("Passwords do not match");
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
    window.location.href = "/login";
});
    //end forgot password