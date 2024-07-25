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


function showMessageModal(message, isSuccess) {  
    $('#modalMessage').text(message);   
    if (isSuccess) {
        $('#messageModalLabel').text('Success');
        $('#modalMessage').css('color', 'green');
    } else {
        $('#messageModalLabel').text('Failure');
        $('#modalMessage').css('color', 'red');
    }    
    $('#messageModal').modal('show');
}

$(document).on('click', '.btn-close-refreshpage', function (event) {
    window.location.href = window.location.href;
    return false;
});

