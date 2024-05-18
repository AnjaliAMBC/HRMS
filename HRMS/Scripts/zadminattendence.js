
function HighlightAdminActiveLink(element) {
    $('#admindash-menu li').removeClass('sidebaractive');
    $('#admindash-menu li a.active').removeClass('active');
    $(element).addClass('active ');
    $(element).parent().addClass('sidebaractive');
}

$(document).on('change', '#attedencemonth', function (event) {
    updateDays();
});

function updateDays(DateSelected) {
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

    AdminAttendenceDataMap(date);

}
function AdminAttendenceDataMap(DateSelected) {
    $.ajax({
        url: '/adminattendance/attendance',
        type: 'GET',
        dataType: 'html',
        data: { selectedDate: DateSelected },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $(".admin-empmanagement-container").html("");
            $('.admin-attendance-container').html("");
            $('.admin-emppadd-container').html("");
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

$(document).off('click', '.admin-attendance').on('click', '.admin-attendance', function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));
    AdminAttendenceDataMap();
});







//Data table 
//var table = $('#adminaddendancetable').DataTable({
//    data: data,
//    "paging": true,
//    "searching": true,
//    "pageLength": 8, // Set the default number of rows to display
//    "lengthChange": false,
//    "info": true,
//    "order": [],
//    "columnDefs": [
//        {
//            "targets": 0,
//            "orderable": false,
//            "class": "tdempid"

//        },
//        {
//            "targets": 1,
//            "orderable": false,
//            "class": "tdempname",
//            "render": function (data, type, full, meta) {
//                var imageHtml = '';
//                if (data.image) {
//                    var imageURl = "/assets/empImages/" + data.image;
//                    imageHtml = '<img src="' + imageURl + '" alt="Profile Image" style="width: 45px; height: 45px; border-radius: 50%; margin-right: 10px;">';
//                } else {
//                    var firstNameInitial = data.name.charAt(0);
//                    var lastNameInitial = data.name.split(' ').slice(-1)[0].charAt(0);
//                    var shortName = firstNameInitial + lastNameInitial;
//                    imageHtml = '<div class="list-image" style="width: 45px; height: 45px; border-radius: 50%; margin-right: 10px; background-color: #C4C4C4; color: white; display: flex; justify-content: center; align-items: center;">' + shortName + '</div>';
//                }
//                return '<div style="display: flex; align-items: center;">' + imageHtml +
//                    '<span style="margin-top: 0px; margin-right: 10px;">' + data.name + '<br><span style="color: #3E78CF;">' + data.email + '</span></span>' + '</div>';
//            }

//        },
//        {
//            "targets": 2,
//            "orderable": false,
//            "class": "tdemplocation",
//        },

//        {
//            "targets": 3,
//            "orderable": false,
//            "class": "tdempdate",

//        },
//        {
//            "targets": 4,
//            "orderable": false,
//            "class": "tdempcheckin",
//        },
//        {
//            "targets": 5,
//            "orderable": false,
//            "class": "tdempcheckout",
//        },
//        {
//            "targets": 6,
//            "orderable": false,
//            "class": "tdemphours",
//        },
//        {
//            "targets": 7,
//            "orderable": false,
//            "class": "tdempstatus",
//            "render": function (data, type, full, meta) {
//                return '<button type="button" class="btn btn-primary edit-btn" title="status">Status</button>';
//            }

//        },
//        {
//            "targets": 8,
//            "orderable": false,
//            "render": function (data, type, full, meta) {
//                return '<span class="edit-btn" title="Edit"><i class="fas fa-pencil-alt"></i></span>';
//            }
//        },
//    ],

//});

function submitDateRange() {
    var fromDate = $('#fromDate').val();
    var toDate = $('#toDate').val();

    // Validate the dates
    if (!fromDate || !toDate) {
        alert('Please select both From and To dates.');
        return;
    }

    if (new Date(fromDate) > new Date(toDate)) {
        alert('The From date cannot be later than the To date.');
        return;
    }

    // Make AJAX call to the server with the date range
    $.ajax({
        url: '/adminattendance/exportattendence',  // Replace with your actual endpoint
        type: 'POST',
        data: JSON.stringify({ fromDate: fromDate, toDate: toDate }),      
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

//$(document).ready(function () {
//    $('#exportIcon').click(function () {
//        $('.dropdown-menu1').toggleClass();
//    });
//});