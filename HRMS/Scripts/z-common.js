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