﻿// Function to calculate total leaves
function calculateTotalLeaves() {
    var total = 0;
    $('.dayleave-type-container').each(function () {
        var dayType = $(this).find('input[type="radio"]:checked').val();
        if (dayType === 'fullDay') {
            total += 1;
        } else if (dayType === 'halfDay') {
            total += 0.5;
        }
    });
    // Update the total leaves display
    $('#totalLeaves').text(total + " Days");
}

// Function to dynamically generate balance section HTML
function generateBalanceSection() {
    var leaveType = $('#leaveType').val();
    var availableBalance = 0;
    var empId = $("#leaveempname option:selected").val();
    $.ajax({
        type: "POST",
        url: "/empleave/getavailableleaves",
        data: { empId: empId, leaveType: leaveType },
        success: function (data) {
            var balanceSectionHTML = `
            <div class="ml-4" style="margin-top:25px;">
                <div class="balance-section mt-4 ml-4">
                    <div class="row" style="line-height:2;">
                        <div class="col-md-4">
                            <span>Total Balance</span>
                        </div>
                        <div class="col-md-6">
                            <div class="available-total">${data.Available}</div>
                        </div>
                        <div class="col-md-4">
                            <span>Currently Booked</span>
                        </div>
                        <div class="col-md-6">
                            <div class="booked-leaves">${data.Booked}</div>
                        </div>
                        <div class="col-md-4">
                            <span>Available Balance</span>
                        </div>
                        <div class="col-md-6">
                            <div class="available-balance">${data.Balance}</div>
                        </div>
                    </div>
                </div>
            </div>`;

            // Replace existing balance section with updated HTML
            $('.balance-section-wrapper').html(balanceSectionHTML);

        },
        error: function () {
            alert("Error occurred while fetching available leaves.");
        }
    });
}

// Function to generate day type rows
function generateDayTypeRows(leaverequestname) {
    var fromDate = $('#fromleaveDate').val();
    var toDate = $('#toleaveDate').val();
    var dayTypeLabel = $('#dayTypeLabel');
    var dayTypeContainer = $('#dayTypeContainer');
    var totalLeavesContainer = $('#totalLeavesContainer'); // Select the total leaves container

    // Clear previous entries
    dayTypeContainer.empty();

    if (fromDate && toDate) {
        // Show the "Day Type" label
        dayTypeLabel.show();

        var start = new Date(fromDate);
        var end = new Date(toDate);

        $.ajax({
            url: '/empleave/getallempleavesinfobasedondate',
            type: 'POST',
            data: { startdate: fromDate, enddate: toDate, leaverequestname: leaverequestname },
            success: function (response) {
                console.log('Success:', response);

                var jsonData = $.parseJSON(response);

                // Parse the JSON data (assuming jsonData is your parsed JSON array)
                /*  var jsonData = response;*/
                // Generate a row for each date in the range
                for (var d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {

                    if ($('#leaveType').val() != "Maternity Leave") {
                        // Skip weekends (Saturday and Sunday)
                        if (d.getDay() === 0 || d.getDay() === 6) {
                            continue;
                        }
                    }

                    var dateStr = d.toISOString().split('T')[0];
                    var dayTypeRow = `
                    <div class="form-group-row apply-leave-form">
                        <div class="apply-date-label">${dateStr}</div>
                        <div class="vertical-line"></div>
                        <div class="dayleave-type-container">`;

                    // Find matching leave request for the current date
                    var matchingLeave = jsonData.find(function (leave) {
                        return leave.leavedate.split('T')[0] === dateStr;
                    });

                    // Determine default values for radio buttons and half day selection
                    var fullDayChecked = 'checked';
                    var halfDayChecked = '';
                    var halfDaySelectDisabled = 'disabled';

                    if (matchingLeave) {
                        if (matchingLeave.DayType === 'fullDay') {
                            fullDayChecked = 'checked';
                        } else if (matchingLeave.DayType === 'halfDay') {
                            halfDayChecked = 'checked';
                            halfDaySelectDisabled = '';
                        }
                    }

                    dayTypeRow += `
                            <div class="form-check">
                                <input class="form-check-input" type="radio" name="dayType_${dateStr}" value="fullDay" ${fullDayChecked}>
                                <label class="form-check-label">Full Day</label>
                            </div>
                            <div class="form-check">
                                <input class="form-check-input" type="radio" name="dayType_${dateStr}" value="halfDay" ${halfDayChecked}>
                                <label class="form-check-label">Half Day</label>
                            </div>
                            <select class="form-control half-day-select" name="halfType_${dateStr}" ${halfDaySelectDisabled}>
                                <option ${matchingLeave && matchingLeave.HalfDayCategory === 'First Half' ? 'selected' : ''}>First Half</option>
                                <option ${matchingLeave && matchingLeave.HalfDayCategory === 'Second Half' ? 'selected' : ''}>Second Half</option>
                            </select>
                        </div>
                    </div>`;
                    dayTypeContainer.append(dayTypeRow);
                }

                // Enable/disable half day selection
                $('input[name^="dayType_"]').change(function () {
                    var halfTypeSelect = $(this).closest('.apply-leave-form').find('select');
                    if ($(this).val() === 'halfDay') {
                        halfTypeSelect.prop('disabled', false);
                    } else {
                        halfTypeSelect.prop('disabled', true);
                    }
                    // Recalculate total leaves when day type changes
                    calculateTotalLeaves();
                });

                // Append the total leaves display to the total leaves container
                totalLeavesContainer.html('<div>Total Leaves: <span id="totalLeaves"><b>0</b></span></div>');

                // Calculate total leaves when day type rows are generated
                calculateTotalLeaves();
            },
            error: function (error) {
                console.error('Error:', error);
                alert('An error occurred while submitting the leave request.');
            }
        });


    } else {
        // Hide the "Day Type" label if dates are not selected
        dayTypeLabel.hide();
        // Clear the total leaves container if dates are not selected
        totalLeavesContainer.empty();
    }
}





$(document).on('change', '#fromleaveDate', function (event) {
    generateDayTypeRows();
});


$(document).on('change', '#toleaveDate', function (event) {
    $('#dayTypeContainer').show();
    $('#totalLeavesContainer').show();
    generateDayTypeRows();
});

$(document).on('click', '.btn-apply-empleave', function (event) {
    event.preventDefault();

    let leaveType = $('#leaveType').val();
    let fromDate = $('#fromleaveDate').val();
    let toDate = $('#toleaveDate').val();
    let hourPermission = $('#HourPermission').val();
    let teamEmail = $('#teamEmail').val();
    let reason = $('#reason').val();
    let EmpID = $("#leaveempname option:selected").val();
    let EmpName = $("#leaveempname option:selected").text();
    let SubmittedBy = $('.loggedinempname').text();
    let backupName = $('#BackupName').val();
    let backupNo = $('#BackupNo').val();
    let location = $("#leaveempname option:selected").attr("data-emplocation");
    let officialEmail = $("#leaveempname option:selected").attr("data-empemail");

    let isValid = true;

    function showError(elementId, message) {
        let errorElement = $('#' + elementId + '-error');
        errorElement.text(message);
        errorElement.show();
        $('#' + elementId).addClass('input-error');
    }

    $('.error-message').hide(); // Hide all error messages at the start
    $('.input-error').removeClass('input-error'); // Remove error highlight at the start

    if (!leaveType || leaveType === "Select Leave Type") {
        isValid = false;
        showError('leaveType', 'Leave Type is required.');
    }

    if (leaveType === "Hourly Permission") {
        if (!fromDate) {
            isValid = false;
            showError('fromleaveDate', 'From Date is required.');
        }
        if (!hourPermission || hourPermission === "Select Hours") {
            isValid = false;
            showError('HourPermission', 'Hour Permission is required for Hourly Permission leave.');
        }
    } else {
        if (!fromDate) {
            isValid = false;
            showError('fromleaveDate', 'From Date is required.');
        }
        if (!toDate) {
            isValid = false;
            showError('toleaveDate', 'To Date is required.');
        }
        if (fromDate && toDate && new Date(toDate) < new Date(fromDate)) {
            isValid = false;
            showError('toleaveDate', 'To Date should be greater than From Date.');
        }
    }

    if (!teamEmail) {
        isValid = false;
        showError('teamEmail', 'Team Email is required.');
    }

    if (!backupName) {
        isValid = false;
        showError('BackupName', 'Backup Resource Name is required.');
    }

    if (!backupNo) {
        isValid = false;
        showError('BackupNo', 'Contact Number (In case of Emergency) is required.');
    }

    let dayTypeContainer = document.getElementById('dayTypeContainer');
    let hasDayTypeSelection = false;
    let dayTypeEntries = [];

    dayTypeContainer.querySelectorAll('.apply-leave-form').forEach(row => {
        let dayType = row.querySelector('input[name^="dayType_"]:checked');

        if (dayType) {
            hasDayTypeSelection = true;
            let date = row.querySelector('.apply-date-label').textContent;
            let dayTypeValue = dayType.value;
            let halfType = '';

            if (dayTypeValue === 'halfDay') {
                halfType = row.querySelector('select[name^="halfType_"]').value;
            }

            dayTypeEntries.push({
                date: date,
                dayType: dayTypeValue,
                halfType: halfType
            });
        } else {
            isValid = false;
            showError('dayTypeContainer', `Day Type is required for ${row.querySelector('.apply-date-label').textContent}.`);
        }
    });

    if (!hasDayTypeSelection && leaveType != "Hourly Permission") {
        isValid = false;
        showError('dayTypeContainer', 'At least one day type selection is required.');
    }

    if ($('.loginempisadmin').text() != "True") {
        var leavesApplyied = $('#totalLeaves').text().split(' ');
        var appliedLeaves = "";
        if (leavesApplyied.length > 1) {
            appliedLeaves = leavesApplyied[0];
            var totalAvailableBalanceLeaves = $('.available-balance').text();

            var decimalAppliedLeaves = parseFloat(appliedLeaves);
            var decimaltotalAvailableBalanceLeaves = parseFloat(totalAvailableBalanceLeaves);

            if (isNaN(decimalAppliedLeaves) || isNaN(decimaltotalAvailableBalanceLeaves)) {
                isValid = false;
                showError('totalLeavesContainer', "Invalid input related to leave balance");
            }

            if (decimalAppliedLeaves > decimaltotalAvailableBalanceLeaves) {
                isValid = false;
                showError('totalLeavesContainer', 'You are not allowed to apply the leaves more than the available balance. Please contact administrator.');
            }
        }
    }

    if (!isValid) {
        return;
    }

    // If all validations passed, proceed with form submission
    let formData = {
        leaveType: leaveType,
        fromDate: fromDate,
        toDate: toDate,
        dayTypeEntries: dayTypeEntries,
        teamEmail: teamEmail,
        reason: reason,
        EmpID: EmpID,
        EmpName: EmpName,
        SubmittedBy: SubmittedBy,
        BackupResource_Name: backupName,
        EmergencyContact_no: backupNo,
        hourPermission: hourPermission,
        Location: location,
        OfficalEmailid: officialEmail,
        ActionType: $('.btn-apply-empleave').text(),
        EditRequestName: $('#editleaverequestname').text()
    };

    $.ajax({
        url: '/empleave/ajaxapplyleave',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            console.log('Success:', response);
            if (response.jsonResponse.StatusCode == 200) {
                $('#leaveType').val($('#leaveType option:first').val());
                $('#fromleaveDate').val('');
                $('#toleaveDate').val('');
                $('#HourPermission').val();
                $('#teamEmail').val('');
                $('#teamEmail').val($('#defaultteamEmail').text());
                $('#reason').val('');
                if ($('.loginempisadmin').text() == "True") {
                    $('#leaveempname').val($('#leaveempname option:first').val());
                }
                $('#BackupName').val('');
                $('#BackupNo').val('');
                $('#dayTypeContainer').empty();
                $('#dayTypeLabel').hide();
                $('#totalLeavesContainer').empty();
                $('#dayTypeContainer').hide();
                $('.balance-section-wrapper').empty();
                $('.time-section').hide();
            }
            alert(response.jsonResponse.Message);
        },
        error: function (error) {
            console.error('Error:', error);
            alert('An error occurred while submitting the leave request.');
        }
    });
});


$(document).on('change', '#leaveType', function (event) {

    $('.balance-section-wrapper').show();
    var selectedLeaveType = $(this).val();
    generateBalanceSection(selectedLeaveType);

    $('#HourPermission').val($('#HourPermission option:first').val());

    if ($(this).val() === 'Hourly Permission') { // Hourly Permission
        $('.to-date-group').hide();
        $('#dayTypeLabel').hide();
        $('#dayTypeContainer').hide();
        $('#totalLeavesContainer').hide();
        $('.time-section').show();
    } else {
        $('.to-date-group').show();
        $('.time-section').hide();
        $('.apply-date-section').show();
        generateDayTypeRows(); // Ensure that the day type rows are generated based on selected dates
    }

    $('#fromleaveDate').val('');
    $('#toleaveDate').val('');
    $('#dayTypeContainer').empty();
    $('#dayTypeLabel').hide();
    $('#totalLeavesContainer').empty();
});


$(document).on('change', '#leaveempname', function (event) {
    $('#leaveType').val($('#leaveType option:first').val());
    $('.balance-section-wrapper').empty();
});

$(document).on('change', '#HourPermission', function (event) {
    $('#totalLeavesContainer').html('<div>Total time: <span id="totalLeaves">' + $("#HourPermission option:selected").text() + '</span></div>');
    //$('#totalLeaves').text($(this).val());
    $('#totalLeavesContainer').show();
});


$(document).on('click', '.dashhoiday_description', function (e) {
    e.preventDefault();
    $('#Compreason').val("");
    var holidayName = $('.selectedholidayname').text($(this).find('h6').text());
    var holidayDate = $('#Compdate').val($(this).find('p').text());
    $('.selectedholidaynumber').text($(this).find('a').attr("data-leavenumber"));
    $('.selectedholidaylocation').text($(this).find('a').attr("data-location"));
    $('#compOffMessage').html("");
    $('#compOffMessage').hide();
    $('#CompemployeeName').removeClass("is-invalid");
    $("#compOffModal").modal('show');
});

//$(document).on('click', '.Emp-CompOffRequest', function (e) {
//    e.preventDefault();
//    $('#Compreason').val("");
//    var holidayName = $('.selectedholidayname').text($(this).find('h6').text());
//    var holidayDate = $('#Compdate').val($(this).find('p').text());
//    $('.selectedholidaynumber').text($(this).attr("data-leavenumber"));
//    $('.selectedholidaylocation').text($(this).attr("data-location"));
//    $('#compOffMessage').html("");
//    $('#compOffMessage').hide();
//    $('#CompemployeeName').removeClass("is-invalid");
//    $("#compOffModal").modal('show');
//});

$(document).on('click', '.compOff-History-Page', function (e) {
    e.preventDefault();
});