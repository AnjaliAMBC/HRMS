
function HighlightAdminActiveLink(element) {
    $('#admindash-menu li').removeClass('sidebaractive');
    $('#admindash-menu li a.active').removeClass('active');
    $(element).addClass('active ');
    $(element).parent().addClass('sidebaractive');
}


$(document).on('change', '#attedencemonth', function (event) {
    updateDays();
});


function updateDays() {
    var selectedMonth = new Date($('#attedencemonth').val());
    var daysInMonth = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 0).getDate();
    var today = new Date().getDate();

    $('#daysContainer').empty(); // Clear previous days

    for (var i = 1; i <= daysInMonth; i++) {
        var dayNumber = i.toString().padStart(2, '0'); // Add leading zero if necessary
        var activeClass = (i === today && selectedMonth.getMonth() === new Date().getMonth()) ? 'active' : '';

        $('#daysContainer').append(`
            <div class="day ${activeClass}" onclick="highlightDate(this)">
                ${dayNumber}
            </div>
        `);
    }
}

function highlightDate(element) {
    $('.day.active').removeClass('active'); // Remove active class from all days
    $(element).addClass('active'); // Add active class to the clicked day
}


$(document).off('click', '.admin-attendance').on('click', '.admin-attendance', function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));
    $.ajax({
        url: '/adminattendance/attendance',
        type: 'GET',
        dataType: 'html',
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

            // Set the initial value of the input field to the current month
            var currentDate = new Date();
            var currentMonth = currentDate.getFullYear() + '-' + ('0' + (currentDate.getMonth() + 1)).slice(-2);
            $('#attedencemonth').val(currentMonth).trigger('change'); // Trigger change event to ensure days are updated

            updateDays(); // Initially populate the days

        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
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

//function submitDateRange() {
//    var fromDate = $('#fromDate').val();
//    var toDate = $('#toDate').val();
//    if (fromDate && toDate) {
//        alert('From Date: ' + fromDate + '\nTo Date: ' + toDate);
//    } else {
//        alert('Please select both dates.');
//    }
//}

//$(document).ready(function () {
//    $('#exportIcon').click(function () {
//        $('.dropdown-menu1').toggleClass();
//    });
//});