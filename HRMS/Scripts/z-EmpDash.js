function GetCurrentTime() {
    var now = new Date(); // Get current date and time
    var hours = now.getHours();
    var minutes = now.getMinutes();
    var seconds = now.getSeconds(); // Get current seconds
    var ampm = hours >= 12 ? 'PM' : 'AM';

    hours = hours % 12;
    hours = hours ? hours : 12; // The hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours; // Add leading zero if hours is less than 10
    minutes = minutes < 10 ? '0' + minutes : minutes; // Add leading zero if minutes is less than 10
    seconds = seconds < 10 ? '0' + seconds : seconds; // Add leading zero if seconds is less than 10

    var formattedTime = hours + ':' + minutes + ':' + seconds + ' ' + ampm;
    return formattedTime;
}


function formatDateAndTime(date) {
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2); // Months are zero-based in JavaScript
    var year = date.getFullYear();
    var hours = ("0" + date.getHours()).slice(-2);
    var minutes = ("0" + date.getMinutes()).slice(-2);
    var seconds = ("0" + date.getSeconds()).slice(-2);

    return day + "-" + month + "-" + year + " " + hours + ":" + minutes + ":" + seconds;
}

$(document).on('click', '.btn-checkin', function (event) {
    event.preventDefault();

    var checkInId = $(".btn-checkin").attr("data-checkinid");

    if ($('.btn-checkin').text() == "Check In") {
        $.ajax({
            url: '/empdash/checkin',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.JsonResponse.StatusCode == 200) {
                    $(".btn-checkin").text("Check Out");
                    $(".btn-checkin").attr("data-checkinid", response.CheckInInfo.login_id);
                    $(".btn-checkin").removeClass("dh-attendance");
                    $(".btn-checkin").addClass("checkputcolor");
                    var checkinTime = formatDateAndTime(new Date());
                    $('#checkinhoursminutes').attr('data-signedindatetime', checkinTime);
                    var currentTime = GetCurrentTime();
                    $('#checkInTime').html(currentTime);

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
                    $('.btn-checkin').text("Check In");
                    $('.btn-checkin').prop('disabled', true);

                    var checkinTime = formatDateAndTime(new Date());
                    $('#checkinhoursminutes').attr('data-signedindatetime', checkinTime);

                    $(".btn-checkin").addClass("dh-attendance");
                    $(".btn-checkin").addClass("checkputcolor");

                    var currentTime = GetCurrentTime();
                    $('#checkOutTime').html(currentTime);

                    if ($('.showcheckinbuttonpostcheckout').text() == "True") {
                        $('.btn-checkin').removeAttr('disabled');
                        $('#checkinhoursminutes').attr('data-signedindatetime', "");
                        $('#checkinhoursminutes').html("");
                        $('#checkOutTime').html("");
                        $('#checkInTime').html("");
                        $('.showcheckinbuttonpostcheckout').text('False');
                    }
                    else {
                        $('#checkinhoursminutes').attr('data-signedindatetime', "");
                    }

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

    var currentDate1 = new Date();

    var startDate = new Date(currentDate1);
    startDate.setDate(currentDate1.getDate() - currentDate1.getDay() + 1);

    var endDate = new Date(currentDate1);
    endDate.setDate(currentDate1.getDate() + (6 - currentDate1.getDay() + 1));

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

function AttedencevalidateDates() {
    var fromDate = document.getElementById('attendancefrom').value;
    var toDate = document.getElementById('attendanceto').value;
    var fromError = document.getElementById('from-error');
    var toError = document.getElementById('to-error');

    var isValid = true;

    if (fromDate === "") {
        fromError.style.display = 'block';
        isValid = false;
    } else {
        fromError.style.display = 'none';
    }

    if (toDate === "") {
        toError.style.display = 'block';
        isValid = false;
    } else {
        toError.style.display = 'none';
    }

    return isValid;
}

$(document).on('click', '.attendance-find', function (event) {
    event.preventDefault();
    var isvalid = AttedencevalidateDates();
    if (isvalid) {
        var fromDate = $("#attendancefrom").val();
        var toDate = $("#attendanceto").val();
        var formattedFromDate = formatDateenUS(fromDate);
        var formattedToDate = formatDateenUS(toDate);
        GetAttendenceInfo(formattedFromDate, formattedToDate);
    }
});


let currentDashCalenderMonth;
let currentDashCalenderYear;

function updateCalendar() {
    generateCalendar(currentDashCalenderMonth, currentDashCalenderYear);
}

function generateCalendar(month, year) {

    $.ajax({
        url: '/adminleave/getholidaysbasedonlocation',
        type: 'POST',
        dataType: 'json',
        data: { empid: $('.loggedinempid').text(), month: month, year: year },
        success: function (holidayresponse) {
            console.log(holidayresponse);

            const holidays = $.parseJSON(holidayresponse);
            const DashCalenderfestivals = generateLeaveHolidays(holidays);

            const today = new Date();
            const firstDayOfMonth = new Date(year, month, 1);
            const daysInMonth = new Date(year, month + 1, 0).getDate();
            let startingDay = firstDayOfMonth.getDay();

            // Adjust starting day so that Monday is the first day of the week
            startingDay = (startingDay + 6) % 7;

            const calendarBody = document.getElementById("calendarBody");
            if (!calendarBody) {
                //console.error("No element found with id 'calendarBody'");
                return;
            }
            calendarBody.innerHTML = "";

            const monthYear = document.getElementById("monthYear");
            if (!monthYear) {
                //console.error("No element found with id 'monthYear'");
                return;
            }
            monthYear.innerText = new Date(year, month).toLocaleDateString('default', { month: 'long', year: 'numeric' });

            let date = 1;
            for (let i = 0; i < 6; i++) {
                const row = document.createElement("tr");

                for (let j = 0; j < 7; j++) {
                    if (i === 0 && j < startingDay) {
                        const cell = document.createElement("td");
                        row.appendChild(cell);
                    } else if (date > daysInMonth) {
                        break;
                    } else {
                        const cell = document.createElement("td");
                        cell.textContent = date;

                        // Highlight Sunday (6) and Saturday (5) in red (because now Sunday is the 6th day)
                        if (j === 5 || j === 6) {
                            cell.style.color = "#8C8C8C";
                        }

                        const festivalKey = `${month + 1}-${date}`;
                        if (date === today.getDate() && year === today.getFullYear() && month === today.getMonth()) {
                            cell.classList.add("highlight-current");
                            const todayTooltip = document.createElement("div");
                            todayTooltip.classList.add("today-tooltip");
                            todayTooltip.textContent = "Today";
                            cell.appendChild(todayTooltip);
                        } else if (DashCalenderfestivals[festivalKey]) {
                            cell.dataset.festival = DashCalenderfestivals[festivalKey];
                            cell.classList.add("highlight-festival");
                            const festivalTooltip = document.createElement("div");
                            festivalTooltip.classList.add("festival-tooltip");
                            festivalTooltip.textContent = DashCalenderfestivals[festivalKey];
                            cell.appendChild(festivalTooltip);
                        }
                        row.appendChild(cell);
                        date++;
                    }
                }
                calendarBody.appendChild(row);
            }
        }
    });
}





function prevMonth() {
    currentDashCalenderMonth--;
    if (currentDashCalenderMonth < 0) {
        currentDashCalenderMonth = 11;
        currentDashCalenderYear--;
    }
    updateCalendar();
}

function nextMonth() {
    currentDashCalenderMonth++;
    if (currentDashCalenderMonth > 11) {
        currentDashCalenderMonth = 0;
        currentDashCalenderYear++;
    }
    updateCalendar();
}


currentDashCalenderMonth = new Date().getMonth();
currentDashCalenderYear = new Date().getFullYear();
generateCalendar(currentDashCalenderMonth, currentDashCalenderYear);

var lastAction = "checkIn";
var lastDate = null;

function toggleCheck() {
    if (lastAction === "checkIn") {
        // Perform check-in
        var currentDate = new Date();
        var hours = currentDate.getHours();
        var minutes = currentDate.getMinutes().toString().padStart(2, '0');
        var seconds = currentDate.getSeconds().toString().padStart(2, '0');

        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // Handle midnight (0 hours)

        var currentTime = hours.toString().padStart(2, '0') + ":" + minutes + ":" + seconds;
        document.getElementById("checkInTime").textContent = currentTime;
        lastAction = "checkOut";
        document.querySelector("button").textContent = "Check Out";
    } else {
        // Perform check-out
        var currentDate = new Date();
        var hours = currentDate.getHours();
        var minutes = currentDate.getMinutes().toString().padStart(2, '0');
        var seconds = currentDate.getSeconds().toString().padStart(2, '0');

        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // Handle midnight (0 hours)

        var currentTime = hours.toString().padStart(2, '0') + ":" + minutes + ":" + seconds;
        document.getElementById("checkOutTime").textContent = currentTime;
        lastAction = "checkIn";
        document.querySelector("button").textContent = "Check In";
    }
}


function updateDashRunningTime() {
    var dashcurrentDateElem = document.getElementById("dashcurrentDate");
    if (dashcurrentDateElem) {
        var currentDate = new Date();
        var hours = currentDate.getHours();
        var minutes = currentDate.getMinutes().toString().padStart(2, '0');
        var seconds = currentDate.getSeconds().toString().padStart(2, '0');

        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12;
        var currentTime = hours.toString().padStart(2, '0') + ":" + minutes + ":" + seconds + " " + ampm;
        document.getElementById("dashcurrentTime").textContent = currentTime;
        document.getElementById("dashcurrentDate").textContent = currentDate.toLocaleDateString('en-IN', { timeZone: 'Asia/Kolkata', weekday: 'long', month: 'long', day: 'numeric', year: 'numeric' });
        //document.getElementById("dashday").textContent = currentDate.toLocaleDateString('en-IN', { timeZone: 'Asia/Kolkata', weekday: 'long' }).toUpperCase() + ",";
    }
}

//Dash leaves scetion

//let currentLeavesIndex = 0;
//const itemsToShow = 4;
//const totalItems = $('.leave-card').length;
//const itemWidth = $('.leave-card').outerWidth(true);

//function updateLeavesCarousel() {
//    const offset = -currentLeavesIndex * itemWidth;
//    $('.leave-row').css('transform', `translateX(${offset}px)`);
//}

//$('.empdashleaves-carousel-next').click(function () {
//    if (currentLeavesIndex < totalItems - itemsToShow) {
//        currentLeavesIndex++;
//        updateLeavesCarousel();
//    }
//});

//$('.empdashleaves-carousel-prev').click(function () {
//    if (currentLeavesIndex > 0) {
//        currentLeavesIndex--;
//        updateLeavesCarousel();
//    }
//});

//updateLeavesCarousel();

//$('#recipeCarousel').carousel({
//    interval: 10000
//});
//$('.carousel .carousel-item').each(function () {
//    var minPerSlide = 4;
//    var next = $(this).next();

//    if (!next.length) {
//        next = $(this).siblings(':first');
//    }

//    for (var i = 0; i < minPerSlide; i++) {
//        if (!next.length) {
//            next = $(this).siblings(':first');
//        }

//        next.children(':first-child').clone().appendTo($(this));
//        next = next.next();
//    }
//});
//$('#recipeCarousel').on('slide.bs.carousel', function () {
//    $(this).find('.carousel-inner').css('transition', 'transform 0.5s ease-in-out');
//});

//$('#recipeCarousel').on('slid.bs.carousel', function () {
//    $(this).find('.carousel-inner').css('transition', '');
//});


function parseCustomDate(dateString) {
    var parts = dateString.split(' ');
    var datePart = parts[0].split('-'); // Split by dash
    var timePart = parts[1].split(':'); // Split by colon

    var day = parseInt(datePart[0], 10);
    var month = parseInt(datePart[1], 10) - 1; // Month is 0-based in JavaScript Date
    var year = parseInt(datePart[2], 10);
    var hours = parseInt(timePart[0], 10);
    var minutes = parseInt(timePart[1], 10);
    var seconds = parseInt(timePart[2], 10);

    return new Date(year, month, day, hours, minutes, seconds);
}

function parseCustomDate1(dateString) {
    // Identify if the date string is in 'MM/DD/YYYY HH:MM:SS AM/PM' format or 'DD-MM-YYYY HH:MM:SS' format
    var isFirstFormat = dateString.includes('/');

    if (isFirstFormat) {
        // Example dateString: '8/5/2024 10:09:28 AM'

        // Split date and time parts
        var [datePart, timePart] = dateString.split(' ');
        var [month, day, year] = datePart.split('/').map(Number);
        var [time, period] = timePart.split(' ');
        var [hours, minutes, seconds] = time.split(':').map(Number);

        // Convert 12-hour format to 24-hour format
        if (dateString.indexOf('PM') !== -1 && hours !== 12) {
            hours += 12;
        } else if (dateString.indexOf('AM') !== -1 && hours === 12) {
            hours = 0;
        }
    } else {
        // Example dateString: '05-08-2024 12:33:30'

        // Split date and time parts
        var [datePart, timePart] = dateString.split(' ');
        var [day, month, year] = datePart.split('-').map(Number);
        var [hours, minutes, seconds] = timePart.split(':').map(Number);
    }

    // Create and return the Date object
    return new Date(year, month - 1, day, hours, minutes, seconds);
}

function updateHoursTimer1() {
    var dashhoursElementElem = document.getElementById("checkinhoursminutes");
    var checkInTime = document.getElementById("checkInTime").textContent; // Get the check-in time
    var checkOutTime = document.getElementById("checkOutTime").textContent; // Get the check-out time

    // Check if both check-in and check-out times are valid and not default
    if (checkInTime !== '00:00:00' && checkOutTime !== '00:00:00') {
        return; // Do not run the script if both times are valid
    }

    if (dashhoursElementElem) {
        var signedInDateTimeStr = $('#checkinhoursminutes').attr('data-signedindatetime');
        var signedInDateTime = parseCustomDate1(signedInDateTimeStr);

        // Check if user is checked in
        if ($('.isusercheckedin').text() == "true" && $('.isusercheckedout').text() == "false") {
            signedInDateTime = parseCustomDate1(signedInDateTimeStr);
        }

        // Validate signedInDateTime
        if (signedInDateTime.toString() == 'Invalid Date' || signedInDateTimeStr.indexOf("01-01-0001") !== -1 || signedInDateTimeStr.indexOf("1/1/0001") !== -1) {
            return;
        }

        // Calculate time difference
        var currentTime = new Date();
        var timeDifference = currentTime - signedInDateTime;
        var totalSeconds = Math.floor(timeDifference / 1000);
        var hours = Math.floor(totalSeconds / 3600);
        var minutes = Math.floor((totalSeconds % 3600) / 60);
        var seconds = totalSeconds % 60;

        // Format the time
        var formattedHours = hours < 10 ? "0" + hours : hours.toString();
        var formattedMinutes = minutes < 10 ? "0" + minutes : minutes.toString();
        var formattedSeconds = seconds < 10 ? "0" + seconds : seconds.toString();
        var formattedTime = formattedHours + ":" + formattedMinutes + ":" + formattedSeconds;

        // Update the check-in hours element
        $('#checkinhoursminutes').text(formattedTime);
    }
}




//On page load
if ($('.admin-leave-container').length > 0) {
    generateCalendar(new Date().getMonth(), new Date().getFullYear());
}


updateDashRunningTime();
setInterval(updateDashRunningTime, 1000);

setInterval(updateHoursTimer1, 1000);
updateHoursTimer1();


$(document).on('click', '.emp-my-tickets-redirect', function (event) {
    window.location.href = "/empticketing/index";
    return false;
});


document.addEventListener('DOMContentLoaded', function () {
    setInterval(function () {
        updateHoursTimer1();
        setInterval(updateHoursTimer1, 1000);
    }, 1000);
});





function getLeaveSummary(employeeId, location, month, year) {
    $.ajax({
        url: '/empdash/getattedencesummary',
        type: 'GET',
        dataType: 'json',
        data: {
            employeeId: employeeId,
            location: location,
            month: month,
            year: year
        },
        success: function (data) {
            console.log('Leave Summary:', data);
            response = $.parseJSON(data);

            $('#totalLeaves').text(response.TotalLeaves);
            $('#totalPresent').text(response.TotalPresent);
            $('#totalAbsent').text(response.TotalAbsent);

            // Update chart
            updateChart(response.TotalLeaves, response.TotalPresent, response.TotalAbsent);

        },
        error: function (xhr, status, error) {
            console.error('Error fetching leave summary:', error);
            alert('Failed to fetch leave summary. Please try again.');
        }
    });
}

function updateChart(present, leaves, absent) {
    var xValues = ["Present", "Leave", "Absent"];
    var yValues = [present, leaves, absent];
    var barColors = [
        "#96CD5C",
        "#C85353",
        "#00B4F2"
    ];

    // Destroy any existing chart instance before creating a new one (prevents duplication)
    if (window.empDashAttendanceChart) {
        window.empDashAttendanceChart.destroy();
    }

    // Create the chart
    window.empDashAttendanceChart = new Chart("emp-dash-attendance", {
        type: "doughnut",
        data: {
            labels: xValues,
            datasets: [{
                backgroundColor: barColors,
                data: yValues
            }]
        },
        options: {
            legend: {
                display: true,
                position: 'bottom'
            },
            // Disable hover effects
            hover: {
                mode: null // Disable hover mode
            },
            // Disable animations if you want it to load instantly without hover effects
            animation: {
                duration: 0 // Disable animations on hover
            }
        }
    });
}




$(document).ready(function () {
    const employeeId = $('.loggedinempid').text().trim();
    const location = $('.loggedinemplocation').text().trim();

    const date = new Date();
    const month = date.getMonth() + 1;
    const year = date.getFullYear();

    getLeaveSummary(employeeId, location, month, year);
});



$(document).on('change', '#emp-dash-attedence-chart', function (event) {
    event.preventDefault();
    const employeeId = $('.loggedinempid').text().trim();
    const location = $('.loggedinemplocation').text().trim();

    const date = new Date();
    const month = $('#emp-dash-attedence-chart').val();
    const year = date.getFullYear();

    getLeaveSummary(employeeId, location, month, year);
});



