function redirectToSubscriptionHistory(subscriptionId) {
    window.location.href = '/Subscription/SubscriptionHistory/' + subscriptionId;
}

function toggleSubscriptioninfoActionOptions(element) {
    $(element).siblings('.subscriptioninfooptions').toggle();
}

function subscriptioninfodelete(id) {
    $('#confirm-delete-btn').attr('onclick', 'confirmDeleteSubscription(' + id + ')');
}

// Function to confirm and perform the deletion of a subscription
function confirmDeleteSubscription(id) {
    $.ajax({
        url: '/Subscription/DeleteSubscription',
        type: 'POST',
        data: { id: id },
        success: function (response) {
            if (response.success) {
                alert(response.message);
                location.reload(); // Reload the page to reflect changes
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + xhr.responseText);
        }
    });}


function redirectToAddSubscription() {
    window.location.href = '/Subscription/AddSubscription';
}

function validateAndSubmitForm() {
    var form = document.getElementById('subscriptionadd-form');
    var isValid = true;

     
    //$(form).find('input[required], select[required], textarea[required]').each(function () {
    //    if (!this.checkValidity()) {
    //        $(this).addClass('is-invalid');  // Add red border to invalid fields
    //        isValid = false;
    //    } else {
    //        $(this).removeClass('is-invalid');  // Remove red border from valid fields
    //    }
    //});

    if (isValid) {
        var formData = new FormData(); // Gather form data, including files
        formData.append("SubscriptionID", $('#SubscriptionID').val());
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

        $.ajax({
            url: '/Subscription/AddUpdateSubscription', // Ensure proper casing for URL         
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    $('#subscriptionadd-success-popup').modal('show');
                    setTimeout(function () {
                        $('#subscriptionadd-success-popup').modal('hide');
                        location.reload(); 
                    }, 3000);
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

function redirectToEditSubscription(subscriptionID) {
    window.location.href = '/Subscription/AddSubscription?subscriptionID=' + subscriptionID;
}
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
