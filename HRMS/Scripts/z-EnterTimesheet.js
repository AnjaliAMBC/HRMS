$(document).on('click', '.emp-entertimesheet-rangedates span', function (event) {
    $(".emp-entertimesheet-rangedates span").removeClass("active");
    $(".emp-entertimesheet-block4 > div").removeClass("active").hide();
    $(this).addClass("active");
    $($(this).attr("href")).addClass("active").show();
});

function getDivCountByUniqueKey(uniqueKey) {
    const divCount = $(`div[data-dayuniquekey="${uniqueKey}"]`).length;
    return divCount;
}

$(document).on('click', '#addTaskBtn', function (event) {
    event.preventDefault();
    var keyNummber = $(this).data("dayuniquekey");
    const count = getDivCountByUniqueKey(keyNummber);
    var dayDate = $(this).data("daydate");
    var newNumber = count + 1;
    var blocknumber = $(this).data("blocknumber");
    var dayindexnumber = $(this).data("dayindexnumber");
    $.ajax({
        url: "/timesheet/addnewrowbydate",
        type: "POST",
        data: { date: dayDate, rownmber: newNumber, dayindexnumber: dayindexnumber, blocknumber: blocknumber },
        success: function (newrow) {
            $("." + blocknumber + " .emp-entertimesheet-footer").before(newrow);
        },
        error: function (error) {
            console.error("Error loading partial view:", error);
            alert("Error loading timesheet data. Please try again.");
        }
    });
});


function showMessageInModal(message, type) {
    const modalMessage = $('#modalMessage');
    const modalTitle = $('#messageModalLabel');

    modalMessage.text(message);

    if (type === "success") {
        modalMessage.css("color", "green");
        modalTitle.text("Success");
    } else if (type === "error") {
        modalMessage.css("color", "red");
        modalTitle.text("Error");
    }

    $('#TimeSheetMessageModal').modal('show');
}


$(document).ready(function () {
    $(document).on('click', '.btn-save-timesheet', function () {
        var dayUniqueKey = $(this).data("dayuniquekey");
        var selectedDate = $(this).data("daydate");
        var weeknumber = $(this).data("weeknumber");
        var firstRow = $('div.emp-entertimesheet-fields[data-dayuniquekey="' + dayUniqueKey + '"]').first();
        var isValid = true;

        // Define a variable to calculate the total hours spent
        let totalHoursSpent = 0;

        // Maximum allowed hours fetched from the data attribute
        const maxAllowedHours = parseFloat($(this).data("maxallaowedhours"));

        // Validate required fields
        firstRow.find('select, input[type="text"]').not('[id*="Requester"]').each(function () {
            if ($(this).val() === "" || $(this).val() === "0") {
                $(this).addClass('is-invalid');
                isValid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });

        if (!isValid) {
            if (firstRow.find('.validation-message').length === 0) {
                /* firstRow.append('<div class="validation-message text-danger mt-2">Please fill in all required fields.</div>'); */
            }
            return; // Exit if validation fails
        }

        firstRow.find('.validation-message').remove();
        var timesheetList = [];

        $('div.emp-entertimesheet-fields[data-dayuniquekey="' + dayUniqueKey + '"]').each(function () {
            var timesheetData = $(this);

            // Parse the hours and minutes
            var hoursSpentInput = timesheetData.find('.timesheet-hoursspent').val();
            var hoursSpent = 0;

            if (hoursSpentInput) {
                var timeParts = hoursSpentInput.split('.'); // Split by '.'
                var hours = parseInt(timeParts[0]) || 0; // Get hours
                var minutes = parseInt(timeParts[1]) || 0; // Get minutes
                hoursSpent = hours + minutes / 60; // Convert minutes to fraction
            }

            totalHoursSpent += hoursSpent;

            var data = {
                Client: $('.selected-timesheet-client').text(),
                EmployeeID: $('.loggedinempid').text(),
                EmployeeName: $('.loggedinempname').text(),
                EmployeeEmail: $('.loggedinempemail').text(),
                Date: selectedDate,
                Category: timesheetData.find('.timesheet-entertimesheetCategory').val(),
                IncidentTaskName: timesheetData.find('.timesheet-entertimesheetTaskName').val(),
                IncidentTaskDescription: timesheetData.find('.timesheet-entertimesheetTaskDesc').val(),
                Requester: timesheetData.find('.timesheet-entertimesheetRequester').val(),
                HoursSpent: hoursSpentInput, // Store formatted hours spent
                Priority: timesheetData.find('.timesheet-entertimesheetPriority').val(),
                Status: timesheetData.find('.timesheet-entertimesheetStatus').val(),
                CreatedBy: timesheetData.find('.created-by').val(),
                CreatedDate: timesheetData.find('.created-date').val(),
                UpdatedBy: timesheetData.find('.updated-by').val(),
                UpdatedDate: timesheetData.find('.updated-date').val(),
                submissionstatus: "Save",
                WeekEnd: weeknumber,
                TimeSheetID: timesheetData.find('.timesheetid').text(),
            };

            timesheetList.push(data);
        });


        if (totalHoursSpent > maxAllowedHours) {
            const maxHours = Math.floor(maxAllowedHours);
            const maxMinutes = Math.round((maxAllowedHours - maxHours) * 60);

            const totalHours = Math.floor(totalHoursSpent);
            const totalMinutes = Math.round((totalHoursSpent - totalHours) * 60);

            showMessageInModal(
                `You cannot enter more than the allowed hours: ${maxHours} hours and ${maxMinutes} minutes. 
        Total hours entered: ${totalHours} hours and ${totalMinutes} minutes.`,
                "error"
            );
            return;
        }


        if (timesheetList.length > 0) {
            $.ajax({
                url: '/timesheet/savetimesheet',
                method: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(timesheetList),
                success: function (response) {
                    console.log('Timesheet data submitted successfully!', response);
                    showMessageInModal(
                        `Timesheet data saved successfully!`,
                        "success"
                    );
                },
                error: function (xhr, status, error) {
                    console.error('Error submitting timesheet data:', error);
                }
            });
        } else {
            console.warn('No timesheet data found to submit.');
        }
    });
});


$(document).on('click', '.btn-close-timesheetsave', function () {
    $('#TimeSheetMessageModal').modal('hide');
    return;
});





$(document).on('input change', '.emp-entertimesheet-fields select, .emp-entertimesheet-fields input[type="text"]', function () {
    $(this).removeClass('is-invalid');
    $(this).closest('.emp-entertimesheet-fields').find('.validation-message').remove();
});



$(document).on('input', '.timesheethoursspent', function () {
    let value = $(this).val();
    if (!/^\d*(\.\d{0,2})?$/.test(value)) {
        $(this).val(value.slice(0, -1));
    }
});

$(document).ready(function () {
    function formatDateToShort(date) {
        var day = date.getDate();
        var month = date.getMonth() + 1;
        var year = date.getFullYear();
        return (day < 10 ? '0' + day : day) + '-' + (month < 10 ? '0' + month : month) + '-' + year;
    }

    function formatDateToLong(date) {
        var day = date.getDate();
        var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
        var month = monthNames[date.getMonth()];
        var year = date.getFullYear();
        return (day < 10 ? '0' + day : day) + ' ' + month + ' ' + year;
    }

    function getWeekNumber(date) {
        var startDate = new Date(date.getFullYear(), 0, 1);
        var diff = date - startDate;
        var oneDay = 1000 * 60 * 60 * 24;
        var dayOfYear = Math.floor(diff / oneDay);
        return Math.ceil((dayOfYear + 1) / 7);
    }

    function getStartEndDateOfWeek(date) {
        var start = new Date(date);
        var end = new Date(date);
        var day = start.getDay();
        var diff = start.getDate() - day + (day == 0 ? -6 : 1);
        start.setDate(diff);
        end.setDate(start.getDate() + 6);
        return {
            startDate: start,
            endDate: end
        };
    }

    function updateWeek(date) {
        var weekData = getStartEndDateOfWeek(date);
        var weekStart = weekData.startDate;
        var weekEnd = weekData.endDate;

        $('#week-start').text(formatDateToLong(weekStart));
        $('#week-end').text(formatDateToLong(weekEnd));
        var weekNumber = getWeekNumber(weekStart);
        $('#week-number').text('Week ' + weekNumber);
        var client = $('.selected-timesheet-client').text();

        //for (var i = 0; i < 7; i++) {
        //    var day = new Date(weekStart);
        //    day.setDate(weekStart.getDate() + i);
        //    $('#date-' + (i + 1)).text(formatDateToShort(day));
        //}

        var weekstart = $('#week-start').text();
        var weekend = $('#week-end').text();
        var weeknumber = weekNumber;
        var empID = $(".loggedinempid").text();

        $.ajax({
            url: "/timesheet/previousweektimesheets",
            type: "POST",
            data: { weekstart: weekstart, weekend: weekend, weeknumber: weeknumber, client: client, empID: empID},
            success: function (newrow) {
                $("#empEntertimesheet").empty();
                $("#empEntertimesheet").append(newrow);
            },
            error: function (error) {
                console.error("Error loading partial view:", error);
                alert("Error loading timesheet data. Please try again.");
            }
        });
    }

    $(document).on('click', '#prev-week', function (event) {
        event.preventDefault();
        var prevWeekStart = new Date($('#week-start').text().split('-').reverse().join('-'));
        prevWeekStart.setDate(prevWeekStart.getDate() - 7);
        updateWeek(prevWeekStart);
    });

    $(document).on('click', '#next-week', function (event) {
        event.preventDefault();
        var nextWeekStart = new Date($('#week-start').text().split('-').reverse().join('-'));
        nextWeekStart.setDate(nextWeekStart.getDate() + 7);
        updateWeek(nextWeekStart);
    });
});


//let currentRow = 5; 
//$(document).on('click', '#addTaskBtn', function () {
//    if (currentRow < 7) {
//        currentRow++;
//        $('.row-' + currentRow).show(); 
//    } else {
//        alert('You can only add up to 2 additional tasks.');
//    }
//});


//$(document).ready(function () {
//    let selectedDate = "";

//    // Date selection handler
//    $(".emp-entertimesheet-rangedates span").on("click", function () {
//        $(".emp-entertimesheet-rangedates span").removeClass("active");
//        $(this).addClass("active");
//        selectedDate = $(this).text(); // Get the selected date

//        // Fetch the timesheet data for the selected date
//        $.ajax({
//            url: "/Timesheet/LoadPartialView", // Endpoint to load partial view
//            type: "GET",
//            data: { date: selectedDate },
//            success: function (html) {
//                $(".emp-entertimesheet-fields-container").html(html); // Update the partial view
//            },
//            error: function (error) {
//                console.error("Error loading partial view:", error);
//                alert("Error loading timesheet data. Please try again.");
//            }
//        });
//    });

//    // Validate the first row (mandatory)
//    function validateRow1() {
//        let isValid = true;
//        const requiredFields = [
//            "#entertimesheetTaskName-1-block-1",
//            "#entertimesheetTaskDesc-1-block-1",
//            "#entertimesheetRequester-1-block-1",
//            "#entertimesheetHoursSpent-1-block-1"
//        ];

//        $.each(requiredFields, function (index, selector) {
//            const field = $(selector);
//            if (!field.val().trim()) {
//                field.addClass("border-red");
//                isValid = false;
//            } else {
//                field.removeClass("border-red");
//            }
//        });

//        // Additional required selects for Row 1
//        const selectsToValidate = [
//            "#entertimesheetCategory-1-block-1",
//            "#entertimesheetPriority-1-block-1",
//            "#entertimesheetStatus-1-block-1"
//        ];

//        $.each(selectsToValidate, function (index, selector) {
//            const field = $(selector);
//            if (!field.val() || field.val() === "0") {
//                field.addClass("border-red");
//                isValid = false;
//            } else {
//                field.removeClass("border-red");
//            }
//        });

//        return isValid;
//    }

//    // Collect all timesheet data from visible rows
//    function collectTimesheetData() {
//        let timesheetData = [];

//        $(".emp-entertimesheet-fields").each(function () {
//            const rowNumber = $(this).data("row-number"); // Assumes a data attribute for row tracking

//            // Get individual fields from each row
//            let rowData = {
//                Client: $('#entertimesheetClientName').val(),
//                Category: $(this).find(`#entertimesheetCategory-${rowNumber}-block-1`).val() || "N/A",
//                IncidentTaskName: $(this).find(`#entertimesheetTaskName-${rowNumber}-block-1`).val() ? $(this).find(`#entertimesheetTaskName-${rowNumber}-block-1`).val().trim() : "N/A",
//                IncidentTaskDescription: $(this).find(`#entertimesheetTaskDesc-${rowNumber}-block-1`).val() ? $(this).find(`#entertimesheetTaskDesc-${rowNumber}-block-1`).val().trim() : "N/A",
//                Requester: $(this).find(`#entertimesheetRequester-${rowNumber}-block-1`).val() ? $(this).find(`#entertimesheetRequester-${rowNumber}-block-1`).val().trim() : "N/A",
//                HoursSpent: $(this).find(`#entertimesheetHoursSpent-${rowNumber}-block-1`).val() ? $(this).find(`#entertimesheetHoursSpent-${rowNumber}-block-1`).val().trim() : "N/A",
//                Priority: $(this).find(`#entertimesheetPriority-${rowNumber}-block-1`).val() || "N/A",
//                Status: $(this).find(`#entertimesheetStatus-${rowNumber}-block-1`).val() || "N/A"
//            };

//            // Push each rowData to the timesheetData array
//            timesheetData.push(rowData);
//        });

//        return timesheetData;
//    }

//    $(".emp-entertimesheet-save button").on("click", function () {
//        if (validateRow1()) {
//            if (selectedDate) {
//                const timesheetData = collectTimesheetData();
//                const payload = {
//                    date: selectedDate,
//                    timesheetData: JSON.stringify(timesheetData)
//                };

//                $.ajax({
//                    url: "/Timesheet/Save",
//                    type: "POST",
//                    data: payload,
//                    success: function (response) {
//                        if (response.success) {
//                            alert("Timesheet data saved successfully.");
//                            $(".emp-entertimesheet-rangedates span.active").trigger("click");
//                        } else {
//                            alert("Failed to save data. Please try again.");
//                        }
//                    },
//                    error: function (error) {
//                        console.error("Error while saving timesheet data:", error);
//                        alert("Error while saving timesheet data.");
//                    }
//                });
//            } else {
//                alert("Please select a date before submitting.");
//            }
//        } else {
//            alert("Please fill out all mandatory fields in Row 1.");
//        }
//    });
//});
