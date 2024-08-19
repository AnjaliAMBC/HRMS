// Toggle password visibility
$(document).on('click', '.toggle-changepassword, .toggle-newpassword, .toggle-confirmpassword', function () {
    $(this).toggleClass("fa-eye fa-eye-slash");
    var input = $($(this).attr("toggle"));
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});


$(document).on('click', '#empChangePasswordSubmitButton', function () {
    var isValid = true;

    $("#changepasswordError").text("");
    $("#newpasswordError").text("");
    $("#confirmpasswordError").text("");

    $("#changepassword, #newpassword, #confirmpassword").removeClass("is-valid");

    var changepassword = $("#changepassword").val().trim();
    var newpassword = $("#newpassword").val().trim();
    var confirmpassword = $("#confirmpassword").val().trim();

    var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$/;

    if (changepassword === "") {
        $("#changepasswordError").text("Current password is required.");
        isValid = false;
    }

    if (newpassword === "") {
        $("#newpasswordError").text("New password is required.");
        isValid = false;
    } else if (!passwordRegex.test(newpassword)) {
        $("#newpasswordError").text("New password must be at least 8 characters long, contain at least one number, one lowercase letter, one uppercase letter, and one special character.");
        isValid = false;
    }

    if (confirmpassword === "") {
        $("#confirmpasswordError").text("Please confirm your new password.");
        isValid = false;
    } else if (newpassword !== confirmpassword) {
        $("#confirmpasswordError").text("Passwords do not match.");
        isValid = false;
    }

    if (isValid) {
        $("#changepassword, #newpassword, #confirmpassword").addClass("is-valid");
               
        $.ajax({
            url: '/Account/ChangePassword', 
            method: 'POST',
            data: {
                currentPassword: changepassword,
                newPassword: newpassword,
                confirmPassword: confirmpassword,
                empid: $('.loggedinempid').text()
            },
            success: function (response) {
                if (response.success) {
                     $('#empChangePasswordModal').modal('hide');
                    $('#empChangePasswordSuccessModal').modal('show');
                } else {
                    if (response.errors.currentPassword) {
                        $('#empChangePasswordModal').modal('show');
                        $("#changepasswordError").text(response.errors.currentPassword);
                    }
                }
            },
            error: function () {
                alert("An error occurred while changing the password. Please try again.");
            }
        });
    }
});

$(document).on('keyup', '#newpassword, #confirmpassword', function () {
    // Real-time validation for matching passwords
    var newpassword = $("#newpassword").val().trim();
    var confirmpassword = $("#confirmpassword").val().trim();

    if (newpassword !== "" && confirmpassword !== "" && newpassword === confirmpassword) {
        $("#newpassword, #confirmpassword").addClass("is-valid");
    } else {
        $("#newpassword, #confirmpassword").removeClass("is-valid");
    }
});