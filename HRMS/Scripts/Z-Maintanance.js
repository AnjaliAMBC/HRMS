
$(document).on('click', '.maintenance-info-view', function (event) {
    event.preventDefault();
    var maintenacesno = $(this).attr("data-maintenancesno");
    window.location.href = '/maintanance/maintananceapproval?sno=' + maintenacesno;
});


$(document).on('click', '.maintanace-emp-history', function (event) {
    event.preventDefault();
    var maintenacesno = $(this).attr("data-maintenancesno");
    window.location.href = '/maintanance/maintananceapproval?sno=' + maintenacesno;
});


$(document).on('click', '.maintenance-updatestatus', function (event) {
    event.preventDefault();

    var maintenancesno = $(this).attr("data-maintenancesno");


    var maintenanceData = {
        Sno: maintenancesno,
        MaintenanceDate: $('#MaintenanceApproved-Date').val(),
        RescheduleDate: $('#MaintenanceApproved-Reschedule').val(),
        AgentID: $('#MaintenanceApproved-AgentName').val(),
        Status: $('#MaintenanceApproved-Status').val(),
        Notes: $('#MaintenanceApproved-Notes').val()
    };

    $.ajax({
        url: '/maintanance/updatemaintenancestatus',
        type: 'POST',
        data: JSON.stringify(maintenanceData),
        contentType: 'application/json',
        success: function (response) {
            $('#MaintenancemodalMessage').text(response.Message);
            if (response.StatusCode === 200) {
                $('#MaintenancemodalMessage').css('color', 'green');
            } else {
                $('#MaintenancemodalMessage').css('color', 'red');
            }

            $('#MaintenancemessageModal').modal('show');
        },
        error: function (xhr, status, error) {
            $('#MaintenancemodalMessage').text('An unexpected error occurred!');
            $('#MaintenancemodalMessage').css('color', 'red');
            $('#MaintenancemessageModal').modal('show');
        }
    });
});


$(document).on('click', '.btn-close-refreshmaintenance', function () {
    window.location.href = "/maintanance/maintananceinfo";
    return false;
});

$(document).on('click', '.maintenance-info-reschedule', function (event) {
    event.preventDefault();

    var messageDiv = $('#MaintenancemodalMessage');
    messageDiv.text('');

    var sno = $(this).attr("data-maintenancesno");
    $.ajax({
        url: '/maintanance/maintanancereschedule',
        type: 'GET',
        data: { sno: sno },
        success: function (data) {
            var response = $.parseJSON(data);
            if (response && response.EditableRecord) {

                $('#RescheduleMaintenance-Date').val(response.EditableRecord.MaintenanceDate ? response.EditableRecord.MaintenanceDate.split('T')[0] : '');
                $('#RescheduleMaintenance-AgentName').val(response.EditableRecord.AgentID);
                $('#RescheduleMaintenance-Reschedule').val(response.EditableRecord.RescheduleDate ? response.EditableRecord.RescheduleDate.split('T')[0] : '');
                $('#RescheduleMaintenance-Notes').val(response.EditableRecord.Notes);
                $('.reschedulemaintenance-id-select').text(response.EditableRecord.EmployeeName);
                $('.reschedulemaintenance-image-btn img').attr('src', '/assets/empimages/' + response.EditableRecord.EmployeeID + '.jpeg' + "?" + new Date);
                $('.reschedulemaintenance-Update').attr("data-maintenancesno", sno);

                // Show the modal
                $('#RescheduleMaintenancePopup').modal('show');
            }
        },
        error: function () {
            alert('Error retrieving maintenance data.');
        }
    });
});


$(document).on('click', '.reschedulemaintenance-Update', function (event) {
    event.preventDefault();

    var maintenancesno = $(this).attr("data-maintenancesno");

    var maintenanceData = {
        Sno: maintenancesno,
        RescheduleDate: $('#RescheduleMaintenance-Reschedule').val(),
        AgentID: $('#RescheduleMaintenance-AgentName').val(),
        AgentName: $('#RescheduleMaintenance-AgentName option:selected').text(),
        Notes: $('#RescheduleMaintenance-Notes').val()
    };

    $.ajax({
        url: '/maintanance/reschedulemaintenance',
        type: 'POST',
        data: JSON.stringify(maintenanceData),
        contentType: 'application/json',
        success: function (response) {
            var messageDiv = $('#MaintenancemodalMessage');
            messageDiv.text(response.Message);
            if (response.StatusCode === 200) {
                messageDiv.css('color', 'green');
            } else {
                messageDiv.css('color', 'red');
            }
            messageDiv.show();
        },
        error: function (xhr, status, error) {
            var messageDiv = $('#MaintenancemodalMessage');
            messageDiv.text('An unexpected error occurred!');
            messageDiv.css('color', 'red');
            messageDiv.show();
        }
    });
});






$(document).ready(function () {
    $('.Add-Schedule-Maintanance').click(function () {
        var formData = new FormData();
        var location = $('input[name="schedulemaintenance-location"]:checked').val();
        formData.append('Location', location);

        $('#multiSelectDropdown option:selected').each(function () {
            formData.append('EmployeeIDs', $(this).val());
            formData.append('EmployeeNames', $(this).data('empname'));
            formData.append('EmployeeEmails', $(this).data('empemailid'));
        });

        var AgentName = $('#ScheduleMaintenance-Agent option:selected').text();
        formData.append('AgentName', AgentName);
        var agentID = $('#ScheduleMaintenance-Agent').val();
        formData.append('AgentID', agentID);
        var date = $('#ScheduleMaintenance-Date').val();
        formData.append('Date', date);
        var timeIn = $('#datetimepicker1').val();
        var timeOut = $('#datetimepicker2').val();
        formData.append('TimeIn', timeIn);
        formData.append('TimeOut', timeOut);

        // AJAX call to the controller
        $.ajax({
            url: '/Maintanance/AddMaintenanceSchedule',
            type: 'POST',
            processData: false,
            contentType: false,
            data: formData,
            beforeSend: function () {
                $('.show-progress').show();
            },
            success: function (response) {
                var messageSpan = $('#maintenanceMessage');
                if (response.StatusCode == 200) {

                    messageSpan.text(response.Message).removeClass('text-danger').addClass('text-success').show();

                    // Optional: Hide the message after a few seconds
                    setTimeout(function () {
                        messageSpan.fadeOut();
                    }, 5000);

                    $('#addmaintenanceinfo-popup').find('input, select').val('');
                    $('#multiSelectDropdown option:selected').prop('selected', false);
                }
                else {
                    messageSpan.text(response.Message).removeClass('text-success').addClass('text-danger').show();

                    setTimeout(function () {
                        messageSpan.fadeOut();
                    }, 5000);

                    $('#addmaintenanceinfo-popup').find('input, select').val('');
                    $('#multiSelectDropdown option:selected').prop('selected', false);
                }

                $('.show-progress').hide();
            },
            error: function (error) {
                var messageSpan = $('#maintenanceMessage');
                // Show error message
                messageSpan.text("An error occurred while scheduling maintenance.").removeClass('text-success').addClass('text-danger').show();
                $('.show-progress').hide();
            }
        });
    });
});

$('#addmaintenanceinfo-popup').on('show.bs.modal', function () {
    $('#multiSelectDropdown').val(null).trigger('change');
    $('#selectedNames').text('');
    $('input[name="schedulemaintenance-location"]').prop('checked', false);
    $('#schedulemaintenance-radio1').prop('checked', true);
});


$('.maintenance-filter-view').on('click', function () {
    var year = $('#maintenance-filter-year-dropdown').val();
    var month = $('#maintenance-filter-month-dropdown').val();
    var location = $('#maintenance-filter-location-dropdown').val();
    $.ajax({
        url: '/maintanance/maintananceinfo',
        type: 'GET',
        data: { year: year, month: month, location: location },
        success: function (result) {
            $('#adminmaintenanceinfotable').html($(result).find('#adminmaintenanceinfotable').html());
            $(document).trigger('ajaxComplete');
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
});



$(document).on('click', '.maintenance-filter-export', function () {
    var year = $('#maintenance-filter-year-dropdown').val();
    var month = $('#maintenance-filter-month-dropdown').val();
    var location = $('#maintenance-filter-location-dropdown').val();

    $.ajax({
        url: '/maintanance/exporttoexcelmaintenance',
        type: 'GET',
        data: { year: year, month: month, location: location },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data, status, xhr) {
            var filename = xhr.getResponseHeader('Content-Disposition').split('filename=')[1].trim();
            var blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = filename;
            link.click();
        },
        error: function (xhr, status, error) {
            console.log("Error: " + error);
        }
    });
});






