$(document).off('click', '.btn-admin-apply-leave1').on('click', '.btn-admin-apply-leave1', function (event) {
    event.preventDefault();
    var date = $(this).attr('data-date');
    AdminLeaveDataMap(date, "");

    //$.ajax({
    //    url: '/adminleave/adminleavemanagement',
    //    type: 'GET',
    //    dataType: 'html',
    //    success: function (response) {
    //        $(".hiddenadmindashboard").html("");
    //        $('.admin-dashboard-container').html("");
    //        $(".admin-emppadd-container").html("");
    //        $('.admin-empmanagement-container').html("");
    //        $('.admin-attendance-container').html("");
    //        $('.admin-leave-container').html("");
    //        $(".hiddenadmindashboard").html(response);
    //        var formContent = $(".hiddenadmindashboard").find(".admin-leave-management-view").html();
    //        $(".admin-leave-container").html(formContent);
    //        $('.admin-leave-container').show();
    //        $('.admin-attendance-container').hide();
    //        $('.admin-empmanagement-container').hide();
    //        $('.admin-emppadd-container').hide();
    //        $('.admin-dashboard-container').hide();
    //        $('.admin-ticketing-container').hide();
    //        $(".hiddenadmindashboard").html("");

    //    },
    //    error: function (xhr, status, error) {
    //        console.error("Error deleting employee:", error);
    //    }
    //});

});



//$(document).on('change', '#myLeaveMonth', function (event) {
//    event.preventDefault();
//    var selectedMonth = new Date($('#myLeaveMonth').val());
//    var startDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth(), 1);
//    var endDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 0);
//    var formattedStartDate = formatDateToCustomString(startDate);
//    var formattedEndDate = formatDateToCustomString(endDate);
//    //AdminAttendenceDataMap(formattedStartDate, formattedEndDate);
//    updateLeaveDays(formattedStartDate);
//});


//function updateLeaveDays(DateSelected) {
//    var currentDate = new Date();
//    if (DateSelected != undefined) {
//        currentDate = new Date(DateSelected);
//    }

//    var currentMonth = currentDate.getFullYear() + '-' + ('0' + (currentDate.getMonth() + 1)).slice(-2);
//    $('#myLeaveMonth').val(currentMonth);

//    var selectedMonth = new Date($('#myLeaveMonth').val());
//    var daysInMonth = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 0).getDate();
//    var today = new Date();

//    $('#daysLeaveContainer').empty();

//    for (var i = 1; i <= daysInMonth; i++) {
//        var dayNumber = i.toString().padStart(2, '0');
//        var fullDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth(), i);
//        var formattedDate = fullDate.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' });

//        var activeClass = (i === today.getDate() && selectedMonth.getMonth() === today.getMonth() && selectedMonth.getFullYear() === today.getFullYear()) ? 'active' : '';

//        if (DateSelected != undefined) {
//            activeClass = "";
//        }

//        // Add future and weekend classes
//        var futureClass = (fullDate > today) ? 'future' : '';
//        var weekendClass = (fullDate.getDay() === 0 || fullDate.getDay() === 6) ? 'weekend' : '';

//        var clickevent = "#";
//        if (futureClass == "") {
//            clickevent = "highlightLeaveDate(this)";
//        }
//        $('#daysLeaveContainer').append(`
//            <div class="dayLeave ${activeClass} ${futureClass} ${weekendClass}" data-date="${formattedDate}" onclick="${clickevent}">
//                ${dayNumber}
//            </div>
//        `);
//    }
//}


//function highlightLeaveDate(element) {
//    $('.dayLeave.active').removeClass('active');
//    $(element).addClass('active');
//    var date = $(element).attr('data-date');

//    //AdminAttendenceDataMap(date, "");

//}







//function updateLeaveDays(DateSelected) {
//    var currentDate = new Date();
//    if (DateSelected != undefined) {
//        currentDate = new Date(DateSelected);
//    }

//    var selectedLeaveMonth = new Date($('#myLeaveMonth').val());
//    var daysInLeaveMonth = new Date(selectedLeaveMonth.getFullYear(), selectedLeaveMonth.getMonth() + 1, 0).getDate();
//    var today = new Date();

//    $('#daysLeaveContainer').empty();

//    for (var i = 1; i <= daysInLeaveMonth; i++) {
//        var dayNumber = i.toString().padStart(2, '0');
//        var fullDate = new Date(selectedLeaveMonth.getFullYear(), selectedLeaveMonth.getMonth(), i);
//        var formattedDate = fullDate.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' });

//        var activeClass = '';

//        if (DateSelected != undefined) {
//            activeClass = '';
//        } else if (selectedLeaveMonth.getMonth() === today.getMonth() && selectedLeaveMonth.getFullYear() === today.getFullYear() && i === today.getDate()) {
//            // Highlight the current date if the selected month is the current month
//            activeClass = 'active';
//        } else if (i === 1 && !(selectedLeaveMonth.getMonth() === today.getMonth() && selectedLeaveMonth.getFullYear() === today.getFullYear())) {
//            // Highlight the first day of the month if the selected month is not the current month
//            activeClass = 'active';
//        }

//        // Add future and weekend classes
//        var futureClass = (fullDate > today) ? 'future' : '';
//        var weekendClass = (fullDate.getDay() === 0 || fullDate.getDay() === 6) ? 'weekend' : '';

//        var clickevent = "#";
//        if (futureClass == "") {
//            clickevent = "highlightLeaveDate(this)";
//        }
//        $('#daysLeaveContainer').append(`
//            <div class="dayLeave ${activeClass} ${futureClass} ${weekendClass}" data-date="${formattedDate}" onclick="${clickevent}">
//                ${dayNumber}
//            </div>
//        `);
//    }
//}


function updateLeaveDays(DateSelected) {
    var currentDate = new Date();
    if (DateSelected != undefined) {
        currentDate = new Date(DateSelected);
    }

    var currentMonth = currentDate.getFullYear() + '-' + ('0' + (currentDate.getMonth() + 1)).slice(-2);
    $('#myLeaveMonth').val(currentMonth);

    var selectedMonth = new Date($('#myLeaveMonth').val());
    var daysInMonth = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 0).getDate();
    var today = new Date();

    $('#daysLeaveContainer').empty();

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
            clickevent = "highlightLeaveDate(this)";
        }
        $('#daysLeaveContainer').append(`
            <div class="dayLeave ${activeClass} ${futureClass} ${weekendClass}" data-date="${formattedDate}" onclick="${clickevent}">
                ${dayNumber}
            </div>
        `);
    }
}
//function updateLeaveDays(DateSelected) {
//    var currentDate = new Date();
//    var selectedDate = null;
//    if (DateSelected != undefined) {
//        selectedDate = new Date(DateSelected);
//        currentDate = selectedDate;
//    }

//    var selectedLeaveMonth = new Date($('#myLeaveMonth').val());
//    var daysInLeaveMonth = new Date(selectedLeaveMonth.getFullYear(), selectedLeaveMonth.getMonth() + 1, 0).getDate();
//    var today = new Date();

//    $('#daysLeaveContainer').empty();

//    for (var i = 1; i <= daysInLeaveMonth; i++) {
//        var dayNumber = i.toString().padStart(2, '0');
//        var fullDate = new Date(selectedLeaveMonth.getFullYear(), selectedLeaveMonth.getMonth(), i);
//        var formattedDate = fullDate.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' });

//        var activeClass = '';

//        if (selectedDate && fullDate.getTime() === selectedDate.getTime()) {
//            activeClass = 'active';
//        } else if (!selectedDate && selectedLeaveMonth.getMonth() === today.getMonth() && selectedLeaveMonth.getFullYear() === today.getFullYear() && i === today.getDate()) {
//            // Highlight the current date if the selected month is the current month and no date is selected
//            activeClass = 'active';
//        } else if (!selectedDate && i === 1 && !(selectedLeaveMonth.getMonth() === today.getMonth() && selectedLeaveMonth.getFullYear() === today.getFullYear())) {
//            // Highlight the first day of the month if the selected month is not the current month and no date is selected
//            activeClass = 'active';
//        }

//        // Add future and weekend classes
//        var futureClass = (fullDate > today) ? 'future' : '';
//        var weekendClass = (fullDate.getDay() === 0 || fullDate.getDay() === 6) ? 'weekend' : '';

//        var clickevent = "#";
//        if (futureClass == "") {
//            clickevent = "highlightLeaveDate(this)";
//        }
//        $('#daysLeaveContainer').append(`
//            <div class="dayLeave ${activeClass} ${futureClass} ${weekendClass}" data-date="${formattedDate}" onclick="${clickevent}">
//                ${dayNumber}
//            </div>
//        `);
//    }
//}

function AdminLeaveDataMap(DateSelected, EndDateSelected) {
    $.ajax({
        url: '/adminleave/adminleavemanagement',
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
            $('.admin-leave-management-view').html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leave-management-view").html();
            $(".admin-leave-container").html(formContent);

            $('.admin-empmanagement-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-attendance-container').hide();
            $('.admin-leave-container').show();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");

            var currentDate = new Date();
            if (DateSelected != undefined) {
                currentDate = new Date(DateSelected);
            }

            var currentMonth = currentDate.getFullYear() + '-' + ('0' + (currentDate.getMonth() + 1)).slice(-2);
            $('#myLeaveMonth').val(currentMonth);

            //updateLeaveDays(DateSelected);

            $('.dayLeave').each(function () {
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


function highlightLeaveDate(element) {
    $('.dayLeave.active').removeClass('active');
    $(element).addClass('active');
    var date = $(element).attr('data-date');
    AdminLeaveDataMap(date, "");
}

//$('#adminaddendancetable').DataTable({
//    "paging": true,
//    "searching": true,
//    "pageLength": 8,
//    "lengthChange": false,
//    "info": true,
//    "order": [],
//});

$(document).on('change', '#myLeaveMonth', function (event) {
    event.preventDefault();
    var selectedMonth = new Date($('#myLeaveMonth').val());
    var startDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth(), 1);
    var endDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 0);
    var formattedStartDate = formatDateToCustomString(startDate);
    var formattedEndDate = formatDateToCustomString(endDate);
    //AdminAttendenceDataMap(formattedStartDate, formattedEndDate);
    updateLeaveDays(formattedStartDate);
});


