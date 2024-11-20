$(document).on('click', '.timesheet-Submit', function (event) {
    event.preventDefault();   
    window.location.href = "/timesheet/submittimesheet";
});


$(document).ready(function () {
    // Function to format date as "dd-mm-yyyy"
    function formatDateToShort(date) {
        var day = date.getDate();
        var month = date.getMonth() + 1; // Months are zero-indexed
        var year = date.getFullYear();
        return (day < 10 ? '0' + day : day) + '-' + (month < 10 ? '0' + month : month) + '-' + year;
    }

    // Function to format date as "dd Month yyyy"
    function formatDateToLong(date) {
        var day = date.getDate();
        var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
        var month = monthNames[date.getMonth()];
        var year = date.getFullYear();
        return (day < 10 ? '0' + day : day) + ' ' + month + ' ' + year;
    }

    // Function to calculate the week number of the year
    function getWeekNumber(date) {
        var startDate = new Date(date.getFullYear(), 0, 1); // First day of the year
        var diff = date - startDate; // Difference between current date and the start of the year
        var oneDay = 1000 * 60 * 60 * 24; // Number of milliseconds in a day
        var dayOfYear = Math.floor(diff / oneDay); // Calculate the day of the year
        return Math.ceil((dayOfYear + 1) / 7); // Return the week number
    }

    // Function to get the start and end date of the week
    function getStartEndDateOfWeek(date) {
        var start = new Date(date);
        var end = new Date(date);
        var day = start.getDay();
        var diff = start.getDate() - day + (day == 0 ? -6 : 1); // Adjust to the start of the week (Monday)
        start.setDate(diff);
        end.setDate(start.getDate() + 6); // Set the end date to 6 days after the start date
        return {
            startDate: start,
            endDate: end
        };
    }

    // Function to update the week, week number, and dates
    function updateWeek(date) {
        var weekData = getStartEndDateOfWeek(date);
        var weekStart = weekData.startDate;
        var weekEnd = weekData.endDate;
       
        $('#week-start').text(formatDateToLong(weekStart));
        $('#week-end').text(formatDateToLong(weekEnd));

        // Update the day elements below the week range with short format (e.g., "01-01-2024")
        for (var i = 0; i < 7; i++) {
            var day = new Date(weekStart);
            day.setDate(weekStart.getDate() + i); // Update the day
            $('#date-' + (i + 1)).text(formatDateToShort(day)); // Set the date for each day in short format
        }

        // Update the week number
        var weekNumber = getWeekNumber(weekStart);
        $('#week-number').text('Week ' + weekNumber);
    }

    // Set the default week (current week)
    var currentDate = new Date();
    updateWeek(currentDate);

    // Previous week click
    $('#prev-week').click(function () {
        var prevWeekStart = new Date($('#week-start').text().split('-').reverse().join('-')); // Convert string back to Date
        prevWeekStart.setDate(prevWeekStart.getDate() - 7); // Move back 7 days to get the previous week's Monday
        updateWeek(prevWeekStart);
    });

    // Next week click
    $('#next-week').click(function () {
        var nextWeekStart = new Date($('#week-start').text().split('-').reverse().join('-')); // Convert string back to Date
        nextWeekStart.setDate(nextWeekStart.getDate() + 7); // Move forward 7 days to get the next week's Monday
        updateWeek(nextWeekStart);
    });
});


let currentRow = 5; 
$(document).on('click', '#addTaskBtn', function () {
    if (currentRow < 7) {
        currentRow++;
        $('.row-' + currentRow).show(); 
    } else {
        alert('You can only add up to 2 additional tasks.');
    }
});


// validations for block 1

$(document).ready(function () {
    // Variable to store the selected date
    let selectedDate = "";
   
    $(".emp-entertimesheet-rangedates span").on("click", function () {
        $(".emp-entertimesheet-rangedates span").removeClass("active");
        $(this).addClass("active");
        selectedDate = $(this).text(); // Get the selected date
        console.log("Selected Date: " + selectedDate);
    });

    // Function to validate Row 1
    function validateRow1() {
        let isValid = true;
        const requiredFields = [
            "#entertimesheetTaskName-1-block-1",
            "#entertimesheetTaskDesc-1-block-1",
            "#entertimesheetRequester-1-block-1",
            "#entertimesheetHoursSpent-1-block-1"
        ];

        $.each(requiredFields, function (index, selector) {
            const field = $(selector);
            if (!field.val().trim()) {
                field.addClass("border-red");
                isValid = false;
            } else {
                field.removeClass("border-red");
            }
        });

        const categoryField = $("#entertimesheetCategory-1-block-1");
        if (categoryField.val() === "" || categoryField.val() === null || categoryField.val() === "0") {
            categoryField.addClass("border-red");
            isValid = false;
        } else {
            categoryField.removeClass("border-red");
        }

        const priorityField = $("#entertimesheetPriority-1-block-1");
        if (priorityField.val() === "" || priorityField.val() === null || priorityField.val() === "0") {
            priorityField.addClass("border-red");
            isValid = false;
        } else {
            priorityField.removeClass("border-red");
        }

        const statusField = $("#entertimesheetStatus-1-block-1");
        if (statusField.val() === "" || statusField.val() === null || statusField.val() === "0") {
            statusField.addClass("border-red");
            isValid = false;
        } else {
            statusField.removeClass("border-red");
        }

        return isValid;
    }

    
    function collectTimesheetData() {
        let timesheetData = []; 

        $(".emp-entertimesheet-fields").each(function (index, row) {
            let rowData = {};

            // Get values for each field in the current row
            rowData.category = $(row).find("#entertimesheetCategory-1-block-1").val();
            rowData.taskName = $(row).find("#entertimesheetTaskName-1-block-1").val();
            rowData.taskDesc = $(row).find("#entertimesheetTaskDesc-1-block-1").val();
            rowData.requester = $(row).find("#entertimesheetRequester-1-block-1").val();
            rowData.hoursSpent = $(row).find("#entertimesheetHoursSpent-1-block-1").val();
            rowData.priority = $(row).find("#entertimesheetPriority-1-block-1").val();
            rowData.status = $(row).find("#entertimesheetStatus-1-block-1").val();

            // Push the current row data into the array
            timesheetData.push(rowData);
        });

        return timesheetData;
    }

    // Click event for the Save button
    $(".emp-entertimesheet-save button").on("click", function () {
        if (validateRow1()) { 
            if (selectedDate) { 
                const timesheetData = collectTimesheetData(); 
                timesheetData.selectedDate = selectedDate;
                $.ajax({
                    url: "/Timesheet/Save", 
                    type: "POST",
                    data: { timesheetData: timesheetData }, 
                    success: function (response) {
                        if (response.success) {
                            alert("Timesheet data saved successfully.");
                        } else {
                            alert("Failed to save data. Please try again.");
                        }
                    },
                    error: function (error) {
                        console.error("Error while saving timesheet data:", error);
                        alert("Error while saving timesheet data.");
                    }
                });
            } else {
                alert("Please select a date before submitting.");
            }
        } else {
            alert("Please fill out all mandatory fields in Row 1.");
        }
    });
});


