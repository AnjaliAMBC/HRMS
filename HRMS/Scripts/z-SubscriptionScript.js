function redirectToSubscriptionHistory(subscriptionId) {
    window.location.href = '/Subscription/SubscriptionHistory/' + subscriptionId;
}

function toggleSubscriptioninfoActionOptions(element) {
    $(element).siblings('.subscriptioninfooptions').toggle();
}


$(document).on('click', '.addsubscription-close', function () {
    window.location.href = '/Subscription/SubscriptionInfo';
});

$(document).on('click', '.addsubscription-cancel', function () {
    window.location.href = window.location.href;
})


var deleteSubscriptionID;

function prepareDelete(subscriptionID) {
    deleteSubscriptionID = subscriptionID;
}

$('.delete-Subscription').on('click', function () {
    if (deleteSubscriptionID) {
        $.ajax({
            url: '/Subscription/DeleteSubscription', 
            type: 'POST',
            data: { id: deleteSubscriptionID },
            success: function (response) {
                if (response.success) {
                    // Optionally, remove the deleted record from the UI
                    $('input[name="subscription-list-checkbox"][value="' + deleteSubscriptionID + '"]').closest('.col-6').remove();
                } else {
                    alert('Failed to delete the subscription.');
                }
                $('#deleteConfirmationsubscription').modal('hide');
            },
            error: function () {
                alert('An error occurred while deleting the subscription.');
                $('#deleteConfirmationsubscription').modal('hide');
            }
        });
    }
});

function redirectToAddSubscription() {
    window.location.href = '/Subscription/AddSubscription';
}

function validateAndSubmitForm() {
    var form = document.getElementById('subscriptionadd-form');
    var isValid = true;


    $(form).find('input[required], select[required], textarea[required]').each(function () {
        if (!this.checkValidity()) {
            $(this).addClass('is-invalid');  // Add red border to invalid fields
            isValid = false;
        } else {
            $(this).removeClass('is-invalid');  // Remove red border from valid fields
        }
    });

    if (isValid) {
        var formData = new FormData(); // Gather form data, including files
        formData.append("SubscriptionNumber", $('#SubscriptionID').val());
        formData.append("SubscriptionName", $('#SubscriptionName').val());
        formData.append("SubscriptionLogo", $('#SubscriptionLogo')[0].files[0]); // For file input
        formData.append("SubscriptionCategory", $('#SubscriptionCategory').val());
        formData.append("SubscriptionPurchasedate", $('#SubscriptionPurchasedate').val());
        formData.append("SubscriptionAmount", $('#SubscriptionAmount').val());
        formData.append("SubscriptionLicense", $('#SubscriptionLicense').val());
        formData.append("SubscriptionRenewaldate", $('#SubscriptionRenewaldate').val());
        formData.append("SubscriptionPaymentMethod", $('#SubscriptionPaymentMethod').val());
        formData.append("SubscriptionRemarks", $('#SubscriptionRemarks').val());
        formData.append("SubscriptionAddedBy", $('#SubscriptionAddedBy').val());
        formData.append("SubscriptionAddeddate", $('#SubscriptionAddeddate').val());
        formData.append("EditRecordID", $('#SubscriptionID').val().replace("S#", ""));
        formData.append("IsNewSubscription", $('.isnewsubscription').text());


        $.ajax({
            url: '/Subscription/AddUpdateSubscription',
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    $('#subscriptionadd-success-popup').modal('show');
                    $('#subscriptionadd-success-popup').modal('hide');
                } else {
                    $('#error-message').text('Failed to add subscription: ' + response.message);
                    $('#subscriptionadd-error-popup').modal('show');
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error Status:', status); // Log the status of the error
                console.error('AJAX Error:', error); // Log the error details
                if (xhr.responseText) {
                    console.error('Response Text:', xhr.responseText); // Log responseText only if it's present
                } else {
                    console.error('No response text received from server');
                }
                $('#error-message').text('An error occurred: ' + (xhr.responseText || error));
                $('#subscriptionadd-error-popup').modal('show');
            },
        });
    } else {
        // Scroll to the first invalid field if validation fails
        $('html, body').animate({
            scrollTop: $(".is-invalid").first().offset().top - 20
        }, 1000);
    }
}
// Function to clear the form fields
function clearForm() {
    var form = document.getElementById('subscriptionadd-form');
    form.reset();
    $(form).find('.is-invalid').removeClass('is-invalid');
}
//edit logic


//export functionality 

function exportSubscriptioninfo() {
    var selectedSubscriptions = [];

    $('input[name="subscription-list-checkbox"]:checked').each(function () {
        selectedSubscriptions.push(parseInt($(this).val()));
    });

    if (selectedSubscriptions.length > 0) {
        $.ajax({
            url: '/Subscription/ExportSubscriptionsToExcel',
            type: 'POST',
            data: JSON.stringify({ subscriptionIds: selectedSubscriptions }),
            contentType: 'application/json; charset=utf-8',
            xhrFields: {
                responseType: 'blob'
            },
            success: function (data) {
                var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = 'SelectedSubscriptions.xlsx';
                link.click();
            },
            error: function (xhr, status, error) {
                alert('An error occurred: ' + xhr.responseText);
            }
        });
    } else {
        alert("Please select at least one subscription to export.");
    }
}
