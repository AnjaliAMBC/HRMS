﻿$(document).on('click', '.timesheet-enter', function (event) {
    event.preventDefault();
    var client = $('#viewtimesheetclient').val();
    window.location.href = "/timesheet/entertimesheet?client=" + client;
    return;
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

    function updateViewWeek(date) {
        var weekData = getStartEndDateOfWeek(date);
        var weekStart = weekData.startDate;
        var weekEnd = weekData.endDate;

        $('#view-week-start').text(formatDateToLong(weekStart));
        $('#view-week-end').text(formatDateToLong(weekEnd));
        var weekNumber = getWeekNumber(weekStart);
        $('#view-week-number').text('Week ' + weekNumber);

        //for (var i = 0; i < 7; i++) {
        //    var day = new Date(weekStart);
        //    day.setDate(weekStart.getDate() + i);
        //    $('#date-' + (i + 1)).text(formatDateToShort(day));
        //}

        var weekstart = $('#view-week-start').text();
        var weekend = $('#view-week-end').text();
        var weeknumber = weekNumber;
        var client = $('#viewtimesheetclient').val();
        var empID = $(".loggedinempid").text();
        $.ajax({
            url: "/timesheet/viewpreviousweektimesheets",
            type: "POST",
            data: { weekstart: weekstart, weekend: weekend, weeknumber: weeknumber, client: client, empID: empID },
            beforeSend: function () {
                $('.show-progress').show();
            },
            success: function (newrow) {
                $(".emp-timesheet-table-body").empty();
                $(".emp-timesheet-table-body").append(newrow);
                $('.show-progress').hide();
            },
            error: function (error) {
                console.error("Error loading partial view:", error);
                $('.show-progress').hide();
            }
        });
    }

    $(document).on('click', '#view-prev-week', function (event) {
        event.preventDefault();
        var prevWeekStart = new Date($('#view-week-start').text().split('-').reverse().join('-'));
        prevWeekStart.setDate(prevWeekStart.getDate() - 7);
        updateViewWeek(prevWeekStart);
    });

    $(document).on('click', '#view-next-week', function (event) {
        event.preventDefault();
        var nextWeekStart = new Date($('#view-week-start').text().split('-').reverse().join('-'));
        nextWeekStart.setDate(nextWeekStart.getDate() + 7);
        updateViewWeek(nextWeekStart);
    });


    $(document).on('click', '#empSubmitTimesheetSubmitButton', function (event) {
        event.preventDefault();
        var weekstartdate = $(this).data("weekstartdate");
        var weekenddate = $(this).data("weekenddate");
        var weeknumber = $(this).data("weeknumber");
        var clientName = $('#viewtimesheetclient').val();
        var empID = $(".loggedinempid").text();

        $.ajax({
            url: "/timesheet/submittimesheet",
            type: "POST",
            data: { weekstart: weekstartdate, weekend: weekenddate, weeknumber: weeknumber, client: clientName, empID: empID },
            success: function (response) {
                if (response.success) {
                    $('#modalMessage')
                        .text(response.message)
                        .removeClass('text-danger')
                        .addClass('text-primary');
                    $('.timesheet-noyes-div').hide();
                    $('.timesheet-close-div').show();
                    $('#empSubmitTimesheetModal').modal('show');
                } else {
                    $('#modalMessage')
                        .text("Error: " + response.message)
                        .removeClass('text-primary')
                        .addClass('text-danger');
                    $('.timesheet-noyes-div').hide();
                    $('.timesheet-close-div').show();
                    $('#empSubmitTimesheetModal').modal('show');
                }

            },
            error: function (error) {
                console.error("Error submitting timesheet:", error);

                $('#modalMessage')
                    .text("An unexpected error occurred. Please try again.")
                    .removeClass('text-primary')
                    .addClass('text-danger');
                $('.timesheet-noyes-div').hide();
                $('.timesheet-close-div').show();
                $('#empSubmitTimesheetModal').modal('show');
            }
        });
    });

    $(document).on('click', '.emp-timesheet-edit-btn', function (event) {
        event.preventDefault();

        var client = $('#viewtimesheetclient').val();
        var selectedDate = $(this).data("timesheetdate");
        window.location.href = "/timesheet/entertimesheet?client=" + client + "&date=" + selectedDate;
        return;

    });

    $(document).on('click', '.btn-timesheet-submit', function (event) {
        event.preventDefault();
        var weekstart = $('#view-week-start').text();
        var weekend = $('#view-week-end').text();

        var confirmationText = `Are you sure you want to submit the timesheet from ${weekstart} to ${weekend}?`;

        $('#modalMessage')
            .text(confirmationText)
            .removeClass('text-danger')
            .addClass('text-primary');

        $('.timesheet-noyes-div').show();
        $('.timesheet-close-div').hide();
    });

    $(document).on('click', '.btn-close-timesheetview', function (event) {
        event.preventDefault();
        window.location.href = "/timesheet/timesheetview";
        return;
    });

    $(document).on('click', '.btn-delete-timesheetview', function (event) {
        event.preventDefault();
        window.location.href = "/timesheet/timesheetview";
        return;
    });

    

    $(document).on('click', '#empTimesheetDeleteButton', function (event) {
        event.preventDefault();
        var timesheetID = $(this).data("deleteticketid");

        $.ajax({
            url: "/timesheet/deletetimesheet",
            type: "POST",
            data: { timesheetid: timesheetID },
            success: function (response) {
                if (response.success) {
                    $('#deletetimesheetmessage')
                        .text(response.message)
                        .removeClass('text-danger')
                        .addClass('text-primary');
                    $('.timesheetdelete-noyes-div').hide();
                    $('.timesheetdelete-close-div').show();
                    $('#empTimesheetConfirmCancelModal').modal('show');
                } else {
                    $('#deletetimesheetmessage')
                        .text("Error: " + response.message)
                        .removeClass('text-primary')
                        .addClass('text-danger');
                    $('.timesheetdelete-noyes-div').hide();
                    $('.timesheetdelete-close-div').show();
                    $('#empTimesheetConfirmCancelModal').modal('show');
                }

            },
            error: function (error) {
                console.error("Error submitting timesheet:", error);

                $('#deletetimesheetmessage')
                    .text("An unexpected error occurred. Please try again.")
                    .removeClass('text-primary')
                    .addClass('text-danger');
                $('.timesheetdelete-noyes-div').hide();
                $('.timesheetdelete-close-div').show();
                $('#empTimesheetConfirmCancelModal').modal('show');
            }
        });
    });

});

