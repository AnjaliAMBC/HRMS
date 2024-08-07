﻿$(document).ready(function () {
    let availableEmails = [];

    $.ajax({
        url: "/AdminLeave/GetEmployeeEmails",
        method: 'GET',
        success: function (data) {
            availableEmails = data;
        },
        error: function (xhr, status, error) {
            console.error("Failed to fetch email data:", status, error);
        }
    });

    $('#teamEmail').autocomplete({
        source: function (request, response) {
            let input = request.term;
            let lastCommaIndex = input.lastIndexOf(',');
            let query = input.substring(lastCommaIndex + 1).trim();
            let filteredEmails = availableEmails.filter(email => email.toLowerCase().startsWith(query.toLowerCase()));
            response(filteredEmails);
        },
        focus: function () {
            return false;
        },
        select: function (event, ui) {
            let input = $('#teamEmail').val().trim();
            let lastCommaIndex = input.lastIndexOf(',');
            let prefix = input.substring(0, lastCommaIndex + 1);
            let newEmail = ui.item.value.trim();

            // Trim spaces and check for duplicates
            let emailList = input.split(',').map(e => e.trim()).filter(e => e);
            if (!emailList.includes(newEmail)) {
                $('#teamEmail').val(prefix + newEmail + ', ');
            }

            $('#teamEmail').focus();
            return false;
        },
        open: function () {
            $('.ui-menu').css('z-index', 1000);
        },
        close: function () { }
    });

    $('#teamEmail').on('keydown', function (e) {
        if (e.keyCode === $.ui.keyCode.ENTER) {
            e.preventDefault();
        }
    });

    $('#teamEmail').on('keyup', function (e) {
        let input = $(this).val();
        let lastChar = input.slice(-1);
        if (lastChar === ',' || e.keyCode === 32) {
            $(this).autocomplete("close");
        } else {
            $(this).autocomplete("search");
        }
    });
});