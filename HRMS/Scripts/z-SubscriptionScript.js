function redirectToSubscriptionHistory() {
    window.location.href = '/subscription/history';
}
function redirectToAddSubscription() {
    window.location.href = '/Subscription/AddSubscription';
}


// Add custom inline validation to show red borders and error messages
function validateAndSubmitForm() {
    var form = document.getElementById('subscriptionadd-form');

    // Check if the form is valid
    var isValid = true;
    $(form).find('input[required], select[required], textarea[required]').each(function () {
        if (this.checkValidity() === false) {
            $(this).addClass('is-invalid');  // Add red border to invalid fields
            isValid = false;
        } else {
            $(this).removeClass('is-invalid');  // Remove red border from valid fields
        }
    });

    if (isValid) {
        // If form is valid, gather form data and submit via AJAX
        var formData = new FormData(form);

        $.ajax({
            url: '/subscription/addsubscription',  // Replace with your actual server-side URL
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {  // Check for `success` property
                    alert('Subscription added successfully');
                    // Optionally, you can redirect or reset the form
                } else {
                    alert('Failed to add subscription: ' + response.message);
                }
            },
            error: function (error) {
                alert('An error occurred. Please try again.');
            }
        });
    } else {
        // If form is not valid, scroll to the first invalid field
        $('html, body').animate({
            scrollTop: $(".is-invalid").first().offset().top - 20
        }, 1000);
    }
}
