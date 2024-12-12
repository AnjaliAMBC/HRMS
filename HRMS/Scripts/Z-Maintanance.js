
$(document).on('click', '.maintenance-info-view', function (event) {
    event.preventDefault();
    var maintenacesno = $(this).attr("data-maintenancesno");
    window.location.href = '/maintanance/maintananceapproval?sno=' + maintenacesno;
});



$(document).on('click', '.maintanace-emp-history', function () {
    event.preventDefault();
    var maintenaceempid = $(this).attr("data-maintenanceempid");
    window.location.href = "/Maintanance/EmpMaintananceHistory?empid=" + maintenaceempid;
})



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
    var rescheduleDate = $('#RescheduleMaintenance-Reschedule').val();
    var agentID = $('#RescheduleMaintenance-AgentName').val();
    var agentName = $('#RescheduleMaintenance-AgentName option:selected').text();
    var notes = $('#RescheduleMaintenance-Notes').val();
    var isValid = true;

    $('#RescheduleMaintenance-Reschedule').removeClass('is-invalid');
    $('#rescheduleDateError').remove();

    if (!rescheduleDate) {
        isValid = false;

        $('#RescheduleMaintenance-Reschedule').addClass('is-invalid');
        $('#RescheduleMaintenance-Reschedule').after('<span id="rescheduleDateError" class="text-danger">Reschedule Date is required</span>');
    }

    if (isValid) {
        var maintenanceData = {
            Sno: maintenancesno,
            RescheduleDate: rescheduleDate,
            AgentID: agentID,
            AgentName: agentName,
            Notes: notes
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
    }
});






$(document).ready(function () {
    $('.Add-Schedule-Maintanance').click(function () {
        var isValid = true;
        var formData = new FormData();

        $('.error-message').remove();
        $('.form-control, .form-check-input').removeClass('is-invalid');

        var location = $('input[name="schedulemaintenance-location"]:checked').val();
        if (!location) {
            isValid = false;
            $('input[name="schedulemaintenance-location"]').parent().after('<span class="error-message text-danger">Please select a location.</span>');
            $('input[name="schedulemaintenance-location"]').addClass('is-invalid');
        } else {
            formData.append('Location', location);
        }

        if ($('#multiSelectDropdown option:selected').length === 0) {
            isValid = false;
            $('#multiSelectDropdown').after('<span class="error-message text-danger">Please select at least one employee.</span>');
            $('#multiSelectDropdown').addClass('is-invalid');
        } else {
            $('#multiSelectDropdown option:selected').each(function () {
                formData.append('EmployeeIDs', $(this).val());
                formData.append('EmployeeNames', $(this).data('empname'));
                formData.append('EmployeeEmails', $(this).data('empemailid'));
            });
        }

        var agentID = $('#ScheduleMaintenance-Agent').val();
        if (!agentID) {
            isValid = false;
            $('#ScheduleMaintenance-Agent').after('<span class="error-message text-danger">Please select an agent.</span>');
            $('#ScheduleMaintenance-Agent').addClass('is-invalid');
        } else {
            formData.append('AgentID', agentID);
            var AgentName = $('#ScheduleMaintenance-Agent option:selected').text();
            formData.append('AgentName', AgentName);
        }

        var date = $('#ScheduleMaintenance-Date').val();
        if (!date) {
            isValid = false;
            $('#ScheduleMaintenance-Date').after('<span class="error-message text-danger">Please select a date.</span>');
            $('#ScheduleMaintenance-Date').addClass('is-invalid');
        } else {
            formData.append('Date', date);
        }

        var timeIn = $('#datetimepicker1').val();
        var timeOut = $('#datetimepicker2').val();

        if (!timeIn || !timeOut || new Date(`1970-01-01T${timeOut}`) <= new Date(`1970-01-01T${timeIn}`)) {
            isValid = false;
            if (!timeIn) {
                $('#datetimepicker1').after('<span class="error-message text-danger">Please enter a valid Time In.</span>');
                $('#datetimepicker1').addClass('is-invalid');
            }
            if (!timeOut) {
                $('#datetimepicker2').after('<span class="error-message text-danger">Please enter a valid Time Out.</span>');
                $('#datetimepicker2').addClass('is-invalid');
            } else {
                $('#datetimepicker2').after('<span class="error-message text-danger">Time Out should be greater than Time In.</span>');
                $('#datetimepicker2').addClass('is-invalid');
            }
        } else {
            formData.append('TimeIn', timeIn);
            formData.append('TimeOut', timeOut);
        }

        if (!isValid) return false;

        if (isValid) {
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

                        setTimeout(function () {
                            messageSpan.fadeOut();
                        }, 5000);

                        $('#addmaintenanceinfo-popup').find('input, select').val('');
                        $('#multiSelectDropdown option:selected').prop('selected', false);
                    } else {
                        messageSpan.text(response.Message).removeClass('text-success').addClass('text-danger').show();

                        setTimeout(function () {
                            messageSpan.fadeOut();
                        }, 5000);
                    }

                    $('.show-progress').hide();
                },
                error: function (error) {
                    var messageSpan = $('#maintenanceMessage');
                    messageSpan.text("An error occurred while scheduling maintenance.").removeClass('text-success').addClass('text-danger').show();
                    $('.show-progress').hide();
                }
            });
        }
    });
});


$('#datetimepicker1, #datetimepicker2').on('input', function () {
    var timeIn = $('#datetimepicker1').val();
    var timeOut = $('#datetimepicker2').val();

    if (timeIn && timeOut && new Date(`1970-01-01T${timeOut}`) > new Date(`1970-01-01T${timeIn}`)) {
        $('#datetimepicker1, #datetimepicker2').removeClass('is-invalid');
        $('.error-message').remove(); // Remove the error message
    }
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


$(document).on('click', '#exportmaintenancehistory', function () {
    var year = $('#maintenancehistory-status-dropdown').val();
    var empid = $('.maintenancehistory-id-select').attr("data-selectedempid");
    $.ajax({
        url: '/maintanance/exporttoexcelmaintenancebyemphistory',
        type: 'GET',
        data: { year: year, empid: empid },
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

$(document).on('click', '.refresh-scheduletablelist', function () {
    window.location.href = "/maintanance/maintananceInfo";
    return false;
});

$(document).on('click', '.emp-maintenanceacknowledge-submit', function () {
    var formData = new FormData();

    // Collecting values from the relevant fields (make sure the IDs correspond to your actual inputs)
    var maintenanceackDate = $('#MaintenanceAcknowledge-Date').val().trim();
    var problemCategory = $('#MaintenanceAcknowledge-ProblemCategory').val();
    var issueFacing = $('#MaintenanceAcknowledge-IssueFacing').val().trim();
    var newAssetRequirement = $('#MaintenanceAcknowledge-AssetReplacement').val().trim();
    var acknowledge = $('#MaintenanceAcknowledge-Completed').val(); // "Yes" or "No"
    var sno = $(this).data('maintenancesno'); // Get the Sno from the clicked link

    // Append data to the FormData object
    formData.append('AcknowledgeDate', maintenanceackDate);
    formData.append('ProblemCategory', problemCategory);
    formData.append('IssueFacing', issueFacing);
    formData.append('NewAssetRequirement', newAssetRequirement);
    formData.append('Acknowledge', acknowledge);
    formData.append('Sno', sno); // Include the Sno

    // Send the data using AJAX
    $.ajax({
        url: '/EmployeeDashboard/AcknowledgeMaintenance',
        type: 'POST',
        data: formData,
        contentType: false, // Important for FormData
        processData: false, // Important for FormData
        success: function (response) {
            // Clear previous message classes
            $('#responseModalLabel').removeClass('success-message error-message');

            // Check if the response indicates success or error
            if (response.success) {
                $('#responseModalLabel').text('Success');
                $('#responseModalBody').text(response.message).addClass('success-message');;
            } else {
                $('#responseModalLabel').text('Error');
                $('#responseModalBody').text(response.message || 'An error occurred while submitting the data.').addClass('error-message');
            }
            $('#responseModal').modal('show');
        },
        error: function (xhr, status, error) {
            console.error(error);
            $('#responseModalLabel').text('Error').addClass('error-message');
            $('#responseModalBody').text('An error occurred while submitting the data.');
            $('#responseModal').modal('show');
        }
    });

});




$(document).on('change', '#emp-mm-year', function () {
    var selectedYear = $(this).val();
    var currentEmployeeID = $('.loggedinempid').text();

    $.ajax({
        url: '/employeedashboard/getmaintenancebyyear',
        type: 'GET',
        data: { year: selectedYear, empID: currentEmployeeID },
        success: function (data) {
            // Update the maintenance records section with the new data
            $('.MaintenanceTableRows-div').html('');
            $('.MaintenanceTableRows-div').html(data);
        },
        error: function (xhr, status, error) {
            console.error(error);
            alert('An error occurred while filtering the maintenance data.');
        }
    });
});


$(document).on('click', '.mainaknowledge-close', function () {
    window.location.href = "/employeedashboard/selfservice";
});
