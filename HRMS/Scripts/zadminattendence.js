
//function HighlightAdminActiveLink(element) {
//    $('#admindash-menu li').removeClass('sidebaractive');
//    $('#admindash-menu li a.active').removeClass('active');
//    $(element).addClass('active ');
//    $(element).parent().addClass('sidebaractive');
//}

function formatDateToCustomString(date) {
    // Extract day, month, and year
    var day = date.getDate();
    var month = date.toLocaleString('default', { month: 'long' }); // Full month name
    var year = date.getFullYear();

    // Format the date as "DD Month YYYY"
    var formattedDate = (day < 10 ? '0' : '') + day + ' ' + month + ' ' + year;

    return formattedDate;
}


$(document).on('change', '#attedencemonth', function (event) {
    //$(document).off('change', '#attedencemonth').on('change', '#attedencemonth', function (event) {
    event.preventDefault();
    //updateDays();

    var selectedMonth = new Date($('#attedencemonth').val());
    var startDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth(), 1);
    var endDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 0);
    var formattedStartDate = formatDateToCustomString(startDate);
    var formattedEndDate = formatDateToCustomString(endDate);
    AdminAttendenceDataMap(formattedStartDate, formattedEndDate);

});

updateDays();

function updateDays(DateSelected) {
    var currentDate = new Date();
    if (DateSelected != undefined) {
        currentDate = new Date(DateSelected);
    }

    var currentMonth = currentDate.getFullYear() + '-' + ('0' + (currentDate.getMonth() + 1)).slice(-2);
    $('#attedencemonth').val(currentMonth);

    var selectedMonth = new Date($('#attedencemonth').val());
    var daysInMonth = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 0).getDate();
    var today = new Date();

    $('#daysContainer').empty();

    for (var i = 1; i <= daysInMonth; i++) {
        var dayNumber = i.toString().padStart(2, '0'); // Add leading zero if necessary
        var fullDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth(), i);
        var formattedDate = fullDate.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' });

        var activeClass = (i === today.getDate() && selectedMonth.getMonth() === today.getMonth() && selectedMonth.getFullYear() === today.getFullYear()) ? 'active' : '';

        if (DateSelected != undefined) {
            activeClass = "";
        }

        // Add future and weekend classes
        var futureClass = (fullDate > today) ? 'future' : '';
        var weekendClass = (fullDate.getDay() === 0 || fullDate.getDay() === 6) ? 'weekend' : '';

        var clickevent = "#";
        if (futureClass == "") {
            clickevent = "highlightDate(this)";
        }
        $('#daysContainer').append(`
            <div class="day ${activeClass} ${futureClass} ${weekendClass}" data-date="${formattedDate}" onclick="${clickevent}">
                ${dayNumber}
            </div>
        `);
    }
}


function highlightDate(element) {
    $('.day.active').removeClass('active');
    $(element).addClass('active');
    var date = $(element).attr('data-date');

    AdminAttendenceDataMap(date, "");

}
function AdminAttendenceDataMap(DateSelected, EndDateSelected) {
    $.ajax({
        url: '/adminattendance/attendance',
        type: 'GET',
        dataType: 'html',
        data: { selectedStartDate: DateSelected, SelectedendDate: EndDateSelected },
        success: function (response) {

            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-attendancemgmt-view").html();
            $(".admin-attendance-container").html(formContent);

            $('.admin-empmanagement-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-attendance-container').show();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");

            var currentDate = new Date();
            if (DateSelected != undefined) {
                currentDate = new Date(DateSelected);
            }

            var currentMonth = currentDate.getFullYear() + '-' + ('0' + (currentDate.getMonth() + 1)).slice(-2);
            $('#attedencemonth').val(currentMonth);

            updateDays(DateSelected);

            $('.day').each(function () {

                var date = $(this).data('date');
                if (date === DateSelected) {
                    $(this).addClass('active');
                }
            });
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
}

//$(document).off('click', '.admin-attendance').on('click', '.admin-attendance', function (event) {
//    event.preventDefault();
//    HighlightAdminActiveLink($(this));
//    AdminAttendenceDataMap();
//});

function submitDateRange() {
    var fromDate = $('#fromDate').val();
    var toDate = $('#toDate').val();

    if (!fromDate || !toDate) {
        alert('Please select both From and To dates.');
        return;
    }

    if (new Date(fromDate) > new Date(toDate)) {
        alert('The From date cannot be later than the To date.');
        return;
    }

    ExportEmpAttendence(fromDate, toDate, "");
}

function ExportEmpAttendence(fromDate, toDate, empID) {
    $.ajax({
        url: '/adminattendance/exportattendence',
        type: 'POST',
        data: JSON.stringify({ fromDate: fromDate, toDate: toDate, empID: empID }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var downloadLink = document.createElement("a");
            downloadLink.href = "data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64," + response;
            downloadLink.download = "EmployeeAttendence.xlsx";
            document.body.appendChild(downloadLink);
            downloadLink.click();
            document.body.removeChild(downloadLink);
        },
        error: function (xhr, status, error) {
            alert("Error occurred while exporting data: " + error);
        }
    });
}

$(document).on('click', '#downloademp-attedenceLink', function (event) {
    var fromDate = $('#empfromDate').val();
    var toDate = $('#emptoDate').val();

    if (!fromDate || !toDate) {
        alert('Please select both From and To dates.');
        return;
    }

    if (new Date(fromDate) > new Date(toDate)) {
        alert('The From date cannot be later than the To date.');
        return;
    }

    ExportEmpAttendence(fromDate, toDate, $('.selectedEmpID').text().trim());
});

function AdminEmpIndividualAttendence(employeeStartDate, employeeEndDate, employeeId) {
    $.ajax({
        url: '/adminattendance/empattendance',
        type: 'GET',
        dataType: 'html',
        data: { selectedStartDate: employeeStartDate, selectedEndDate: employeeEndDate, selectedEmpID: employeeId },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-attendancemgmt-view").html();
            $(".admin-attendance-container").html(formContent);

            $('.admin-empmanagement-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-attendance-container').show();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
}

$(document).on('click', '.employee-info', function (event) {
    var employeeId = $(this).find('.employee-id').text();
    var employeeSignInDate = $(this).find('.emp-logindate').text();

    $.ajax({
        url: '/adminattendance/empattendance',
        type: 'GET',
        dataType: 'html',
        data: { selectedDate: employeeSignInDate, selectedEmpID: employeeId },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-attendancemgmt-view").html();
            $(".admin-attendance-container").html(formContent);

            $('.admin-empmanagement-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-attendance-container').show();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});

$(document).on('click', '#applyButton', function (event) {
    var fromDate = $('#empfromDate').val();
    var toDate = $('#emptoDate').val();
    AdminEmpIndividualAttendence(fromDate, toDate, $('.selectedEmpID').text().trim());
});

$(document).on('click', '.clearattendence-filter', function (event) {
    event.preventDefault();
    $('#fromDate').val('');
    $('#toDate').val('');
});




