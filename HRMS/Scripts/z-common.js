// Function to clear form data and select first index from dropdowns
function clearFormDataAndSelectFirstIndex(forms) {
    // Iterate through each form field
    $(forms).each(function () {
        $('#' + $(this).attr("id") + ' :input').each(function () {
            var elementType = $(this).prop('tagName').toLowerCase();
            if (elementType === 'select') {
                $(this).val($(this).find('option:first').val());
            } else if ($(this).attr('type') === 'checkbox') {
                $(this).prop('checked', false);
            } else {
                $(this).val('');
            }
        });
    });
}


function showMessageModal(message, isSuccess, closepopuponly) {
    $('#modalMessage').text(message);
    if (isSuccess) {
        $('#messageModalLabel').text('Success');
        $('#modalMessage').css('color', 'green');
    } else {
        $('#messageModalLabel').text('Failure');
        $('#modalMessage').css('color', 'red');
    }

    // Get the close button in the modal footer
    var closeButton = $('.btn-close-refreshpage');

    if (closepopuponly != undefined && closepopuponly != "") {
        closeButton.attr('data-dismiss', 'modal');
    } else {
        closeButton.removeAttr('data-dismiss');
        // You can add a class if you need to handle something specific when the button doesn't close the modal
        closeButton.addClass('btn-close-refreshpage');
    }

    $('#messageModal').modal('show');
}

$(document).on('click', '.btn-close-refreshpage', function (event) {
    // Check if the button has the data-dismiss attribute
    if (!$(this).attr('data-dismiss')) {
        window.location.href = window.location.href;
    }
    return false;
});

