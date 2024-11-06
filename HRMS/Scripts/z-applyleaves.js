// Function to calculate total leaves
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

    $('#totalLeavesNumber').text(total);
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
            <div class="ml-4 res-emp-apply-leave" style="margin-top:25px;">
                <div class="balance-section mt-4 ml-4">
                    <div class="emp-apply-leave-title">${leaveType}</div>
                    <div class="row" style="line-height:2;">
                        <div class="col-md-4 leave-apply-info-block">
                            <div class="col-md-3 res-leave-apply-left">
                                <div class="available-total">${data.Available}</div>
                            </div>
                            <div class="col-md-9 res-leave-apply-right">
                                <span>Total Balance</span>
                            </div>
                        </div>
                        <div class="col-md-4 leave-apply-info-block">
                            <div class="col-md-3 res-leave-apply-left">
                                <div class="booked-leaves">${data.Booked}</div>
                            </div>
                            <div class="col-md-9 res-leave-apply-right">
                                <span>Currently Booked</span>
                            </div>
                        </div>
                        <div class="col-md-4 leave-apply-info-block">
                            <div class="col-md-3 res-leave-apply-left">
                                <div class="available-balance">${data.Balance}</div>
                            </div>
                            <div class="col-md-9 res-leave-apply-right">
                                <span>Available Balance</span>
                            </div>                            
                        </div>
                    </div>
                </div>
            </div>`;


            $('.balance-section-wrapper').html(balanceSectionHTML);

            if ($('#isedirecord').text() == "true") {


                if (leaveType == "Hourly Permission") {
                    var editRecordBookedHours = $('.currentrecordbookedhours').attr("data-bookedhours");
                    var totalBookedHours = $('.booked-leaves').text();
                    var totalAvailableHours = $('.available-balance').text();

                    var alterBookedHurs = parseFloat(totalBookedHours) - parseFloat(editRecordBookedHours);
                    $('.booked-leaves').text(alterBookedHurs);

                    var alterTotalAvailableBalance = parseFloat(totalAvailableHours) + parseFloat(editRecordBookedHours);
                    $('.available-balance').text(alterTotalAvailableBalance);

                    $('.balance-section-wrapper').show();
                }

                else {

                    var editableRecordLeaves = $('#totalLeavesNumber').text();
                    var existingAvailableLeaveBalance = $('.available-balance').text();

                    // Convert them to numbers (assuming they are numeric)
                    var totalLeaves = parseFloat(editableRecordLeaves) || 0;
                    var availableLeaveBalance = parseFloat(existingAvailableLeaveBalance) || 0;

                    // Combine the two values
                    var combinedTotal = totalLeaves + availableLeaveBalance;
                    $('.available-balance').text(combinedTotal);


                    var bookedLeaves = $('.booked-leaves').text();
                    var totalBookedLeaves = parseFloat(bookedLeaves) || 0;

                    var combinedBookedLeaves = totalBookedLeaves - totalLeaves;
                    $('.booked-leaves').text(combinedBookedLeaves)

                    $('.balance-section-wrapper').show();
                }

            }
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
    var empId = $('#leaveempname').val();
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
                $.ajax({
                    url: '/adminleave/GetHolidaysBasedonLocation', // Adjust URL as needed
                    type: 'GET', // Adjust HTTP method as needed
                    data: { empid: empId, month: start.getMonth(), year: start.getFullYear() },
                    success: function (data) {

                        var holidays = JSON.parse(data);

                        for (var d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {

                            if ($('#leaveType').val() != "Maternity Leave") {
                                // Skip weekends (Saturday and Sunday)
                                if (d.getDay() === 0 || d.getDay() === 6) {
                                    continue;
                                }
                            }

                            var dateStr = d.toISOString().split('T')[0];

                            // Skip holidays unless it's maternity leave
                            var isHoliday = holidays.some(function (holiday) {
                                if ($('#leaveType').val() != "Maternity Leave" && holiday.holiday_date.split('T')[0] === dateStr) {
                                    return holiday.holiday_date.split('T')[0] === dateStr;
                                }
                            });

                            if (isHoliday) {
                                continue;
                            }

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

                        if ($('#isedirecord').text() == "true" && leaverequestname != undefined) {
                            generateBalanceSection();
                        }


                    },
                    error: function () {
                        alert("Error occurred while fetching available leaves.");
                    }
                });


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

        if ($('#leaveType').val() == "Hourly Permission") {
            generateBalanceSection();
        }
    }

}


$(document).on('change', '#fromleaveDate', function (event) {

    var fromDate = $(this).val();
    var $toDateInput = $('#toleaveDate');

    if (fromDate) {
        $toDateInput.prop('disabled', false);
        $toDateInput.attr('min', fromDate); // Ensure "To Date" is not before "From Date"
    } else {
        $toDateInput.prop('disabled', true);
        $toDateInput.val('');
    }

    generateDayTypeRows();
});





$(document).on('change', '#toleaveDate', function (event) {

    var fromDate = $('#fromleaveDate').val();
    var toDate = $(this).val();

    if (toDate && toDate < fromDate) {
        $(this).val('');
        $('#toleaveDate-error').text('To Date cannot be before From Date.');
    } else {
        $('#toleaveDate-error').text('');
    }

    $('#dayTypeContainer').show();
    $('#totalLeavesContainer').show();

    generateDayTypeRows();

});

// On End Time change
$('#datetimepicker2').on('change', function () {
    var startTime = $('#datetimepicker1').val(); // Start time value
    var endTime = $('#datetimepicker2').val(); // End time value

    if (startTime && endTime) {
        // Create Date objects with a fixed date for comparison
        var startDate = new Date('1970-01-01T' + startTime);
        var endDate = new Date('1970-01-01T' + endTime);

        // Calculate the time difference in minutes
        var timeDifference = (endDate - startDate) / (1000 * 60); // Time difference in minutes

        // Define valid time intervals in minutes
        var validTimeIntervals = [30, 60, 90, 120]; // 0.5 hours, 1 hour, 1.5 hours, 2 hours

        // Check if the time difference is a valid value
        if (validTimeIntervals.includes(timeDifference)) {
            // Update the total leave hours
            $('.spanleavehours').text(timeDifference / 60); // Convert minutes to hours
        } else {
            // If the time difference is not valid, clear the total hours
            $('#totalLeaveHours').text('');
            showError('datetimepicker2', 'Time difference must be 30 minutes, 1 hour, 1.5 hours, or 2 hours.');

        }
    } else {
        $('#totalLeaveHours').text('');
    }
});


function ClearApplyLeaveFormFields() {
    $('#leaveType').val($('#leaveType option:first').val());
    $('#fromleaveDate').val('');
    $('#toleaveDate').val('');
    /* $('#HourPermission').val('');*/
    $('#teamEmail').val('');
    //$('#teamEmail').val($('#defaultteamEmail').text());
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

function showError(elementId, message) {
    let errorElement = $('#' + elementId + '-error');
    errorElement.text(message);
    errorElement.show();
    $('#' + elementId).addClass('input-error');
}

$(document).on('click', '.btn-apply-empleave', function (event) {
    event.preventDefault();
    $('.error-message-showerror').hide();

    let leaveType = $('#leaveType').val();
    let fromDate = $('#fromleaveDate').val();
    let toDate = $('#toleaveDate').val();
    let hourPermission = false;
    let teamEmail = $('#teamEmail').val();
    let reason = $('#reason').val();
    let EmpID = $("#leaveempname option:selected").val();
    let EmpName = $("#leaveempname option:selected").attr("data-empname");
    let SubmittedBy = $('.loggedinempname').text();
    let backupName = $('#BackupName').val();
    let backupNo = $('#BackupNo').val();
    let location = $("#leaveempname option:selected").attr("data-emplocation");
    let officialEmail = $("#leaveempname option:selected").attr("data-empemail");
    let department = $("#leaveempname option:selected").attr("data-department");
    let designation = $("#leaveempname option:selected").attr("data-designation");

    let startTime = $('#datetimepicker1').val();
    let endTime = $('#datetimepicker2').val();

    let isValid = true;



    $('.error-message').hide(); // Hide all error messages at the start
    $('.input-error').removeClass('input-error'); // Remove error highlight at the start

    if (!leaveType || leaveType === "Select Leave Type") {
        isValid = false;
        showError('leaveType', 'Leave Type is required.');
    }

    if (leaveType === "Hourly Permission") {

        hourPermission = true;

        // 1. Validate From Date
        if (!fromDate) {
            isValid = false;
            showError('fromleaveDate', 'From Date is required.');
        }

        // 2. Validate Start Time and End Time
        if (!startTime) {
            isValid = false;
            showError('datetimepicker1', 'Start Time is required.');
        }
        if (!endTime) {
            isValid = false;
            showError('datetimepicker2', 'End Time is required.');
        }

        // 3. Ensure End Time is greater than Start Time
        if (startTime && endTime && new Date(`1970-01-01 ${endTime}`) <= new Date(`1970-01-01 ${startTime}`)) {
            isValid = false;
            showError('datetimepicker2', 'End Time must be greater than Start Time.');
        }

        // 4. Calculate Time Difference in Minutes
        if (startTime && endTime) {
            var startTimeDate = new Date(`1970-01-01 ${startTime}`);
            var endTimeDate = new Date(`1970-01-01 ${endTime}`);
            var timeDifferenceInMinutes = (endTimeDate - startTimeDate) / (1000 * 60); // in minutes

            // 5. Validate if Time Difference is Valid (30 mins, 1 hour, 1.5 hours, or 2 hours)
            var allowedTimeDifferences = [30, 60, 90, 120]; // 30 mins, 1 hour, 1.5 hours, 2 hours
            if (allowedTimeDifferences.indexOf(timeDifferenceInMinutes) === -1) {
                isValid = false;
                showError('datetimepicker2', 'Time difference must be 30 minutes, 1 hour, 1.5 hours, or 2 hours.');
            }

            // 6. Validate Against Available Balance
            var availableBalanceInHours = parseFloat($('.available-balance').text()) || 0;
            var availableBalanceInMinutes = availableBalanceInHours * 60; // Convert hours to minutes

            if (timeDifferenceInMinutes > availableBalanceInMinutes) {
                isValid = false;
                showError('datetimepicker2', 'Total time difference exceeds available balance.');
            }
        }
    }


    else {
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
                $('.error-message-showerror').html("Invalid input related to leave balance.<br><br>");
                $('.error-message-showerror').show();
                //showError('error-message-showerror', "Invalid input related to leave balance");
            }

            if (decimalAppliedLeaves > decimaltotalAvailableBalanceLeaves) {
                isValid = false;
                $('.error-message-showerror').html("You are not allowed to apply the leaves more than the available balance. Please contact administrator.<br><br>");
                $('.error-message-showerror').show();
                //showError('error-message-showerror', 'You are not allowed to apply the leaves more than the available balance. Please contact administrator.');
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
        EditRequestName: $('#editleaverequestname').text(),
        Department: department,
        Designation: designation,
        StartTime: startTime,
        EndTime: endTime
    };

    $.ajax({
        url: '/empleave/ajaxapplyleave',
        type: 'POST',
        contentType: 'application/json',
        beforeSend: function () {
            $('.show-progress').show();
        },
        data: JSON.stringify(formData),
        success: function (response) {
            console.log('Success:', response);
            if (response.jsonResponse.StatusCode == 200) {
                // Clear form fields
                ClearApplyLeaveFormFields();

                if ($('.loginempisadmin').text() != "True") {
                    $('#modalMessage').removeClass('text-danger').addClass('text-success').text(response.jsonResponse.Message);
                    $('#leavemessageModal').modal('show');
                }
                else {
                    $('#modalMessage').removeClass('text-danger').addClass('text-success').text(response.jsonResponse.Message);
                    $('#adminleavemessageModal').modal('show');
                }


            } else {
                if ($('.loginempisadmin').text() != "True") {
                    $('#modalMessage').removeClass('text-success').addClass('text-danger').text(response.jsonResponse.Message);
                    $('#leavemessageModal').modal('show');
                }
                else {
                    $('#modalMessage').removeClass('text-success').addClass('text-danger').text(response.jsonResponse.Message);
                    $('#adminleavemessageModal').modal('show');
                }

            }
        },
        error: function (error) {
            console.error('Error:', error);
            // Show error message in modal
            if ($('.loginempisadmin').text() != "True") {
                $('#modalMessage').removeClass('text-success').addClass('text-danger').text('An error occurred while submitting the leave request.');
                $('#leavemessageModal').modal('show');
            }
            else {
                $('#modalMessage').removeClass('text-success').addClass('text-danger').text('An error occurred while submitting the leave request.');
                $('#adminleavemessageModal').modal('show');
            }
        },
        complete: function () {
            $('.show-progress').hide();
        }
    });

});


$(document).off('change', '#leaveType').on('change', '#leaveType', function (event) {

    $('#fromleaveDate').val('');
    $('#toleaveDate').val('');
    $('#dayTypeContainer').empty();
    $('#dayTypeLabel').hide();
    $('#totalLeavesContainer').empty();


    $('.error-message-showerror').html("");
    $('.error-message-showerror').hide();

    $('.balance-section-wrapper').show();
    var selectedLeaveType = $(this).val();
    generateBalanceSection(selectedLeaveType);

    //$('#HourPermission').val($('#HourPermission option:first').val());

    if ($(this).val() === 'Hourly Permission') { // Hourly Permission
        $('.to-date-group').hide();
        $('#dayTypeLabel').hide();
        $('#dayTypeContainer').hide();
        $('#totalLeavesContainer').hide();
        $('.time-section').show();
        $('.currentrecordbookedhours').show();
    } else {
        $('.to-date-group').show();
        $('.time-section').hide();
        $('.currentrecordbookedhours').hide();
        $('.apply-date-section').show();
        generateDayTypeRows(); // Ensure that the day type rows are generated based on selected dates
    }


});


$(document).on('change', '#leaveempname', function (event) {
    $('#leaveType').val($('#leaveType option:first').val());
    $('.balance-section-wrapper').empty();
});

//$(document).on('change', '#HourPermission', function (event) {
//    $('#totalLeavesContainer').html('<div>Total time: <span id="totalLeaves">' + $("#HourPermission option:selected").text() + '</span></div>');
//    //$('#totalLeaves').text($(this).val());
//    $('#totalLeavesContainer').show();
//});

$(document).on('click', '.dashhoiday_description', function (e) {
    e.preventDefault();

    $('#Compreason').val("");

    // Fetch data from clicked element and its siblings or parent elements
    var holidayName = '';
    var holidayDate = '';
    var holidayNumber = $(this).data("leavenumber");
    var location = $(this).data("location");

    if ($(this).closest('.emp-dash-upcoming-holidays-block').length) {
        // If clicked element is from the dashboard holiday block
        holidayName = $(this).siblings('.emp-upcoming-left').find('.holiday-name').text();
        holidayDate = $(this).siblings('.emp-upcoming-left').find('.holiday-date').text();
    } else {
        // If clicked element is from the table row
        var $row = $(this).closest('tr');
        holidayName = $row.find('td:nth-child(1)').text(); // Holiday name from 1st column
        holidayDate = $row.find('td:nth-child(2)').text(); // Holiday date from 2nd column
    }

    $('.selectedholidayname').text(holidayName);  // Set hidden div value
    $('#Compdate').val(holidayDate);  // Set date in the input field

    // Set the hidden fields for holiday number and location
    $('.selectedholidaynumber').text(holidayNumber);
    $('.selectedholidaylocation').text(location);

    // Optionally, ensure the selected employee's name remains selected
    var empName = $('#CompemployeeName option:selected').text();
    if (empName) {
        $('#CompemployeeName').val(empName);  // Make sure the employee name remains selected if it's there
    }

    // Hide any previous messages and reset invalid field
    $('#compOffMessage').html("").hide();
    $('#CompemployeeName').removeClass("is-invalid");

    // Show the modal
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



$(document).on('click', '.btn-applyleave-cancel', function (e) {
    e.preventDefault();
    ClearApplyLeaveFormFields();
});




$(document).on('click', '.btn-leaveclose-refreshpage', function () {
    window.location.href = "/empLeave/index";
    return;
});

$(document).on('click', '.btn-leavecloseadmin-refreshpage', function () {
    window.location.href = "/adminleave/index";
    return;
});

