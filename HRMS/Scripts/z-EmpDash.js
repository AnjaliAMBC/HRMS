
$(document).on('click', '.btn-checkin', function (event) {
    event.preventDefault();

    var checkInId = $(".btn-checkin").attr("data-checkinid");

    if ($('.btn-checkin').text() == "Check-In") {
        $.ajax({
            url: '/empdash/checkin',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.JsonResponse.StatusCode == 200) {
                    $(".btn-checkin").text("Check Out");
                    $(".btn-checkin").attr("data-checkinid", response.CheckInInfo.login_id);

                } else {
                    console.error('Error While CheckIn', error);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error uploading file:', error);
            },
            complete: function () {
            }
        });
    }
    else {
        $.ajax({
            url: '/empdash/checkout',
            type: 'POST',
            dataType: 'json',
            data: { checkInID: checkInId },
            success: function (response) {
                if (response.JsonResponse.StatusCode == 200) {
                    $(".btn-checkin").hide();

                } else {
                    console.error('Error While CheckIn', error);
                }
            },
            error: function (xhr, status, error) {
                console.error('Error uploading file:', error);
            },
            complete: function () {
            }
        });
    }

});

// Function to format date as "Date MonthName Year"
function formatDate(date) {
    return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' });
}


function GetAttendenceInfo(startDate, endDate) {
    $.ajax({
        url: "/empattendance/index",
        type: 'POST',
        dataType: 'html',
        data: {
            startDate: startDate,
            endDate: endDate
        },
        success: function (response) {
            $(".hiddenempdashboard").html("");
            $(".emp-dashboard-data").html("");
            $(".selfservice-dashboard-data").html("");
            $(".attedance-dashboard-data").html("");
            $(".leave-dashboard-data").html("");
            $(".myrequest-dashboard-data").html("");


            $(".hiddenempdashboard").html(response);
            var formContent = $(".hiddenempdashboard").find(".empattendence-container").html();
            $(".attedance-dashboard-data").html(formContent);
            $('.attedance-dashboard-data').show();
            $('.emp-dashboard-data').hide();
            $('.selfservice-dashboard-data').hide();
            $('.leave-dashboard-data').hide();
            $('.myrequest-dashboard-data').hide();

            $(".hiddenempdashboard").html("");
        },
        error: function (xhr, status, error) {
            console.error('Error uploading file:', error);
        },
        complete: function () {
        }
    });
}


$(function () {
    // Initial dates
    //var startDate = new Date("April 01, 2024");
    //var endDate = new Date("April 08, 2024");


    // Get today's date
    var currentDate = new Date();

    // Calculate the start of the week (Sunday)
    var startDate = new Date(currentDate);
    startDate.setDate(currentDate.getDate() - currentDate.getDay() + 1);

    // Calculate the end of the week (Saturday)
    var endDate = new Date(currentDate);
    endDate.setDate(currentDate.getDate() + (6 - currentDate.getDay() + 1));

    // Update UI
    $("#week-start").text(startDate.toDateString());
    $("#week-end").text(endDate.toDateString());

    // Left button click
    $(document).on('click', '.left-btn', function (event) {
        startDate.setDate(startDate.getDate() - 7);
        endDate.setDate(endDate.getDate() - 7);
        updateWeekDates(startDate, endDate);
    });

    // Right button click
    $(document).on('click', '.right-btn', function (event) {
        startDate.setDate(startDate.getDate() + 7);
        endDate.setDate(endDate.getDate() + 7);
        updateWeekDates(startDate, endDate);
    });

    // Function to update week dates
    function updateWeekDates(start, end) {
        $("#week-start").text(formatDate(start));
        $("#week-end").text(formatDate(end));
       
        var startDate = $("#week-start").text();
        var endDate = $("#week-end").text();
     
        GetAttendenceInfo(startDate, endDate);

    }
});

// Function to format date as "Date MonthName Year"
function formatDateenUS(inputDate) {
    // Convert input string to Date object
    var date = new Date(inputDate);
    return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' });
}

$(document).on('click', '.attendance-find', function (event) {
    event.preventDefault();
    // Read values from 'from' and 'to' date inputs
    var fromDate = $("#attendancefrom").val();
    var toDate = $("#attendanceto").val();

    // Format dates as "dd MMMM yyyy"
    var formattedFromDate = formatDateenUS(fromDate);
    var formattedToDate = formatDateenUS(toDate);

    GetAttendenceInfo(formattedFromDate, formattedToDate);
});





//var lastAction = "checkIn";
//var lastDate = null;

//function toggleCheck() {
//    if (lastAction === "checkIn") {
//        // Perform check-in
//        var currentDate = new Date();
//        var hours = currentDate.getHours();
//        var minutes = currentDate.getMinutes().toString().padStart(2, '0');
//        var seconds = currentDate.getSeconds().toString().padStart(2, '0');

//        var ampm = hours >= 12 ? 'PM' : 'AM';
//        hours = hours % 12;
//        hours = hours ? hours : 12; // Handle midnight (0 hours)

//        var currentTime = hours.toString().padStart(2, '0') + ":" + minutes + ":" + seconds;
//        document.getElementById("checkInTime").textContent = currentTime;
//        lastAction = "checkOut";
//        document.querySelector("button").textContent = "Check-Out";
//    } else {
//        // Perform check-out
//        var currentDate = new Date();
//        var hours = currentDate.getHours();
//        var minutes = currentDate.getMinutes().toString().padStart(2, '0');
//        var seconds = currentDate.getSeconds().toString().padStart(2, '0');

//        var ampm = hours >= 12 ? 'PM' : 'AM';
//        hours = hours % 12;
//        hours = hours ? hours : 12; // Handle midnight (0 hours)

//        var currentTime = hours.toString().padStart(2, '0') + ":" + minutes + ":" + seconds;
//        document.getElementById("checkOutTime").textContent = currentTime;
//        lastAction = "checkIn";
//        document.querySelector("button").textContent = "Check-In";
//    }
//}


//function updateRunningTime() {
//    var currentDate = new Date();
//    var hours = currentDate.getHours();
//    var minutes = currentDate.getMinutes().toString().padStart(2, '0');
//    var seconds = currentDate.getSeconds().toString().padStart(2, '0');

//    var ampm = hours >= 12 ? 'PM' : 'AM';
//    hours = hours % 12;
//    hours = hours ? hours : 12; // Handle midnight (0 hours)

//    var currentTime = hours.toString().padStart(2, '0') + ":" + minutes + ":" + seconds;

//    document.getElementById("currentDate").textContent = currentDate.toLocaleDateString('en-IN', { timeZone: 'Asia/Kolkata', year: 'numeric', month: 'long', day: 'numeric' });
//    document.getElementById("currentTime").textContent = currentTime;
//    document.getElementById("day").textContent = currentDate.toLocaleDateString('en-IN', { timeZone: 'Asia/Kolkata', weekday: 'long' }).toUpperCase() + ",";
//}


//// Update time every second
//setInterval(updateRunningTime, 1000);

//        let currentMonth;
//        let currentYear;

//        // Define festivals
//        const festivals = {
//            '1-1': 'New Year',
//            '12-25': 'Christmas',
//            '10-2': 'Gandhi Jayanti',
//            // Add more festivals here as needed
//        };

//        function updateCalendar() {
//            generateCalendar(currentMonth, currentYear);
//        }

//        function generateCalendar(month, year) {
//            const today = new Date();
//            const firstDayOfMonth = new Date(year, month, 1);
//            const daysInMonth = new Date(year, month + 1, 0).getDate();
//            const startingDay = firstDayOfMonth.getDay();

//            const calendarBody = document.getElementById("calendarBody");
//            calendarBody.innerHTML = "";

//            document.getElementById("monthYear").innerText = new Date(year, month).toLocaleDateString('default', { month: 'long', year: 'numeric' });

//            let date = 1;
//            for (let i = 0; i < 6; i++) {
//                const row = document.createElement("tr");

//                for (let j = 0; j < 7; j++) {
//                    if (i === 0 && j < startingDay) {
//                        const cell = document.createElement("td");
//                        row.appendChild(cell);
//                    } else if (date > daysInMonth) {
//                        break;
//                    } else {
//                        const cell = document.createElement("td");
//                        cell.textContent = date;
//                        const festivalKey = `${month + 1}-${date}`;
//                        if (date === today.getDate() && year === today.getFullYear() && month === today.getMonth()) {
//                            cell.classList.add("highlight-current");
//                        } else if (festivals[festivalKey]) {
//                            cell.dataset.festival = festivals[festivalKey];
//                            cell.classList.add("highlight-festival");
//                            const festivalTooltip = document.createElement("div");
//                            festivalTooltip.classList.add("festival-tooltip");
//                            festivalTooltip.textContent = festivals[festivalKey];
//                            cell.appendChild(festivalTooltip);
//                        }
//                        row.appendChild(cell);
//                        date++;
//                    }
//                }

//                calendarBody.appendChild(row);
//            }
//        }

//        function prevMonth() {
//            currentMonth--;
//            if (currentMonth < 0) {
//                currentMonth = 11;
//                currentYear--;
//            }
//            updateCalendar();
//        }

//        function nextMonth() {
//            currentMonth++;
//            if (currentMonth > 11) {
//                currentMonth = 0;
//                currentYear++;
//            }
//            updateCalendar();
//        }

//        // Initialize current month and year
//        const currentDate = new Date();
//        currentMonth = currentDate.getMonth();
//        currentYear = currentDate.getFullYear();
//        generateCalendar(currentMonth, currentYear);