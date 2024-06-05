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
    // Update the total leaves display
    $('#totalLeaves').text(total + "Days");
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
                            <span>Available Balance</span>
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
                            <span>Balance</span>
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
function generateDayTypeRows() {
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
                        <div class="dayleave-type-container">
                            <div class="form-check">
                                <input class="form-check-input" type="radio" name="dayType_${dateStr}" value="fullDay" checked>
                                <label class="form-check-label">Full Day</label>
                            </div>
                            <div class="form-check">
                                <input class="form-check-input" type="radio" name="dayType_${dateStr}" value="halfDay">
                                <label class="form-check-label">Half Day</label>
                            </div>
                            <select class="form-control half-day-select" name="halfType_${dateStr}" disabled>
                                <option>First Half</option>
                                <option>Second Half</option>
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
    generateDayTypeRows();
});

$(document).on('click', '.btn-apply-empleave', function (event) {
    event.preventDefault();

    let leaveType = $('#leaveType').val();
    let fromDate = $('#fromleaveDate').val();
    let toDate = $('#toleaveDate').val();
    let teamEmail = $('#teamEmail').val();
    let reason = $('#reason').val();
    let EmpID = $("#leaveempname option:selected").val();
    let EmpName = $("#leaveempname option:selected").text();
    let SubmittedBy = $('.loggedinempname').text();
    let BackupResource_Name = $('#BackupName').val();
    let EmergencyContact_no = $('#BackupNo').val();

    let isValid = true;
    let errorMessage = '';

    if (!leaveType || leaveType === "Select Leave Type") {
        isValid = false;
        errorMessage += 'Leave Type is required.\n';
    }
    if (!fromDate) {
        isValid = false;
        errorMessage += 'From Date is required.\n';
    }
    if (!toDate) {
        isValid = false;
        errorMessage += 'To Date is required.\n';
    }
    if (!teamEmail) {
        isValid = false;
        errorMessage += 'Team Email is required.\n';
    }

    let dayTypeContainer = document.getElementById('dayTypeContainer');
    let dayTypeEntries = [];
    let hasDayTypeSelection = false;

    dayTypeContainer.querySelectorAll('.apply-leave-form').forEach(row => {
        let date = row.querySelector('.apply-date-label').textContent;
        let dayType = row.querySelector('input[name^="dayType_"]:checked');

        if (dayType) {
            hasDayTypeSelection = true;
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
            errorMessage += `Day Type is required for ${date}.\n`;
        }
    });

    if (!hasDayTypeSelection) {
        isValid = false;
        errorMessage += 'At least one day type selection is required.\n';
    }

    if (!isValid) {
        alert(errorMessage);
        return;
    }

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
        BackupResource_Name: BackupResource_Name,
        EmergencyContact_no: EmergencyContact_no
    };

    $.ajax({
        url: '/empleave/ajaxapplyleave',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        success: function (response) {
            console.log('Success:', response);
            alert('Leave request submitted successfully.');
        },
        error: function (error) {
            console.error('Error:', error);
            alert('An error occurred while submitting the leave request.');
        }
    });
});


$(document).on('change', '#leaveType', function (event) {
    var selectedLeaveType = $(this).val();
    generateBalanceSection(selectedLeaveType);

    if ($(this).val() === 'Hourly Permission') { // Hourly Permission
        $('.to-date-group').hide();
        $('#dayTypeLabel').hide();
        $('#dayTypeContainer').hide();
        $('#totalLeavesContainer').hide();
        $('.time-section').show();
    } else {
        $('.to-date-group').show();
        $('.time-section').hide();
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