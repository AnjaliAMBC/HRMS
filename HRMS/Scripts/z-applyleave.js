
function generateDayTypeRows() {
    var fromDate = $('#fromleaveDate').val();
    var toDate = $('#toleaveDate').val();
    var dayTypeLabel = $('#dayTypeLabel');
    var dayTypeContainer = $('#dayTypeContainer');

    // Clear previous entries
    dayTypeContainer.empty();

    if (fromDate && toDate) {
        // Show the "Day Type" label
        dayTypeLabel.show();

        var start = new Date(fromDate);
        var end = new Date(toDate);

        // Generate a row for each date in the range
        for (var d = new Date(start); d <= end; d.setDate(d.getDate() + 1)) {
            var day = d.getDay();
            // Skip weekends (Saturday is 6 and Sunday is 0)
            if (day === 0 || day === 6) {
                continue;
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
        });
    } else {
        // Hide the "Day Type" label if dates are not selected
        dayTypeLabel.hide();
    }
}




$(document).on('change', '#fromleaveDate', function (event) {
    generateDayTypeRows();
});


$(document).on('change', '#toleaveDate', function (event) {
    generateDayTypeRows();
});

$(document).on('click', '.btn-apply-empleave', function (event) {
    event.preventDefault();

    let leaveType = document.getElementById('leaveType').value;
    let fromDate = document.getElementById('fromleaveDate').value;
    let toDate = document.getElementById('toleaveDate').value;
    let teamEmail = document.getElementById('teamEmail').value;
    let reason = document.getElementById('reason').value;

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
        reason: reason
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