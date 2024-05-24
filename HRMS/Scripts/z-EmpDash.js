
function GetCurrentTime() {
    var now = new Date(); // Get current date and time
    var hours = now.getHours();
    var minutes = now.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';

    hours = hours % 12;
    hours = hours ? hours : 12; // The hour '0' should be '12'
    hours = hours < 10 ? '0' + hours : hours; // Add leading zero if hours is less than 10
    minutes = minutes < 10 ? '0' + minutes : minutes;

    var formattedTime = hours + ':' + minutes + ' ' + ampm;
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

    if ($('.btn-checkin').text() == "Check-In") {
        $.ajax({
            url: '/empdash/checkin',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.JsonResponse.StatusCode == 200) {
                    $(".btn-checkin").text("Check Out");
                    $(".btn-checkin").attr("data-checkinid", response.CheckInInfo.login_id);
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
                    $('.btn-checkin').text("Check-In");
                    $('.btn-checkin').prop('disabled', true);

                    var checkinTime = formatDateAndTime(new Date());
                    $('#checkinhoursminutes').attr('data-signedindatetime', checkinTime);

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