function LeaveCarousel() {
    $('#leaveCarousel').carousel({
        interval: 10000
    });

    $('.carousel .carousel-leave-item').each(function () {
        var minPerSlide = 5;
        var next = $(this).next();

        if (!next.length) {
            next = $(this).siblings(':first');
        }

        for (var i = 0; i < minPerSlide - 1; i++) {
            if (!next.length) {
                next = $(this).siblings(':first');
            }

            next.children(':first-child').clone().appendTo($(this));
            next = next.next();
        }
    });
    $('#leaveCarousel').on('slide.bs.carousel', function () {
        $(this).find('.carousel-leave-inner').css('transition', 'transform 0.5s ease-in-out');
    });

    $('#leaveCarousel').on('slid.bs.carousel', function () {
        $(this).find('.carousel-leave-inner').css('transition', '');
    });
}


let currentLeaveMonth;
let currentLeaveYear;

const leaveholidays = {
    '4-8': 'New Year',
    '4-15': 'Pongal/Makar Sankranti',
    '4-26': 'Republic Day',
};

const leaveRequests = {
    '4-29': [
        { name: 'John Doe', image: 'https://via.placeholder.com/24', more: 3 },
    ]
};

function fetchLeaveHolidays() {
    updateLeaveCalendar();
}

function updateLeaveCalendar() {
    generateLeaveCalendar(currentLeaveMonth, currentLeaveYear);
}

//On Page load
generateLeaveCalendar(new Date().getMonth(), new Date().getFullYear());

function generateLeaveCalendar(month, year) {
    var isAdminLeavePage = false;
    var linktoleavecalender = "btn-apply-leave1";

    if ($('div.admin-leave-container').length) {
        isAdminLeavePage = true;
        linktoleavecalender = "btn-admin-apply-leave1";
    }

    const today = new Date();
    const firstDayOfMonth = new Date(year, month, 1);
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const startingDay = firstDayOfMonth.getDay() === 0 ? 6 : firstDayOfMonth.getDay() - 1;

    const calendarBody = document.getElementById("empleave-calendarBody");
    if (!calendarBody) {
        return;
    }
    calendarBody.innerHTML = "";

    const monthYear = document.getElementById("empleave-monthYear");
    if (!monthYear) {
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
                cell.classList.add(linktoleavecalender);

                const holidayKey = `${month + 1}-${date}`;
                if (date === today.getDate() && year === today.getFullYear() && month === today.getMonth()) {
                    cell.classList.add("highlight-currentleave");
                } else if (leaveholidays[holidayKey]) {
                    cell.classList.add("highlight-festival-leave");
                    cell.innerHTML += `<div>${leaveholidays[holidayKey]}</div>`;
                }

                if (leaveRequests[holidayKey]) {
                    const leaveDiv = document.createElement("div");
                    leaveDiv.classList.add("empleave-leave-request");

                    leaveRequests[holidayKey].forEach((leave, index) => {
                        const img = document.createElement("img");
                        img.src = leave.image;
                        img.alt = leave.name;
                        leaveDiv.appendChild(img);

                        if (index === leaveRequests[holidayKey].length - 1 && leave.more) {
                            const more = document.createElement("div");
                            more.textContent = `+${leave.more}`;
                            leaveDiv.appendChild(more);
                        }
                    });

                    cell.appendChild(leaveDiv);
                }

                // Add weekend background color
                if (j === 5 || j === 6) { // 5 is Saturday, 6 is Sunday
                    cell.classList.add("weekend-background");
                }

                row.appendChild(cell);
                date++;
            }
        }

        calendarBody.appendChild(row);
    }
}

// Add jQuery event listener for the click event
//$(document).on('click', '.btn-apply-leave', function (event) {
//    const date = $(this).text();
//    window.location.href = `your_page_url?date=${year}-${month + 1}-${date}`;
//});




function prevLeaveMonth() {
    currentLeaveMonth--;
    if (currentLeaveMonth < 0) {
        currentLeaveMonth = 11;
        currentLeaveYear--;
    }
    updateLeaveCalendar();
}

function nextLeaveMonth() {
    currentLeaveMonth++;
    if (currentLeaveMonth > 11) {
        currentLeaveMonth = 0;
        currentLeaveYear++;
    }
    updateLeaveCalendar();
}

const leavetoday = new Date();
currentLeaveMonth = leavetoday.getMonth();
currentLeaveYear = leavetoday.getFullYear();
fetchLeaveHolidays();


// Convert JSON date format to readable date
function formatJSONDate(jsonDate) {
    const date = new Date(parseInt(jsonDate.replace(/\/Date\((.*?)\)\//, '$1')));
    const dateOptions = { day: 'numeric', month: 'long', year: 'numeric' };
    const dayOptions = { weekday: 'short' };
    const formattedDate = date.toLocaleDateString('en-GB', dateOptions);
    const day = date.toLocaleDateString('en-GB', dayOptions);
    return `${formattedDate} <br> ${day}`;
}

function GetEmpLeaveHistory() {
    $.ajax({
        url: '/empLeave/empleavehistory',
        type: 'POST',
        dataType: 'json',
        data: { empId: $('.loggedinempid').text(), year: 2024 },
        success: function (response) {
            const tableData = response.map(item => {
                return [
                    formatJSONDate(item.leavedate), // Date
                    item.LeaveDays, // Day(s)
                    item.leavesource, // Leave Type
                    `<span style="color:forestgreen">${item.LeaveStatus}</span>`, // Status
                    `<i class="fa-solid fa-message mb-3" style="color: #6b8dda;"></i>
                     <p class="ml-3" style="margin:0;">${item.leave_reason}</p>`, // Comment
                    `<i class="fas fa-ellipsis-h leave-edit-history" id="toggleOptions"></i>
                     <div class="emp-leaveoptions" id="emp-leaveoptions" style="display:none">
                         <a class="dropdown-item emp-leave-edit" href="#" data-leavenum='${item.leaveno}'>Edit</a>
                         <a class="dropdown-item emp-leave-cancel" href="#" data-leavenum='${item.leaveno}'>Cancel</a>
                     </div>` // Actions
                ];
            });

            // Check if DataTable is already initialized and destroy it if so
            if ($.fn.DataTable.isDataTable('#leaveHistoryTable')) {
                $('#leaveHistoryTable').DataTable().clear().destroy();
            }

            $('#leaveHistoryTable').DataTable({
                data: tableData,
                paging: true,
                pageLength: 8,
                searching: false,
                ordering: false,
                info: false,
                lengthChange: false,
                dom: 'rt<"bottom"p><"clear">',
                language: {
                    paginate: {
                        next: 'Next',
                        previous: 'Previous'
                    }
                },
                columns: [
                    { title: "Date", width: "12%" },
                    { title: "Day(s)", width: "5%" },
                    { title: "Leave Type", width: "15%" },
                    { title: "Status", width: "15%" },
                    { title: "Comment", width: "30%" },
                    { title: "Actions" }
                ]
            });
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}


function toggleLeaveView() {
    var historyBtn = document.querySelector('.history_btn');
    var leaveHistory = document.querySelector('.leave-history');
    var cardDiv = document.querySelector('.empleave-card');
    var icon = document.querySelector('.fa-solid');

    if (leaveHistory.style.display === "none") {
        historyBtn.textContent = "View Calendar";
        icon.classList.remove("fa-clock-rotate-left");
        icon.classList.add("fa-calendar-alt");
        cardDiv.style.display = 'none';
        leaveHistory.style.display = 'block';
        GetEmpLeaveHistory();

    } else {
        historyBtn.textContent = "Leave History";
        icon.classList.remove("fa-calendar-alt");
        icon.classList.add("fa-clock-rotate-left");
        cardDiv.style.display = 'block';
        leaveHistory.style.display = 'none';
    }
}

$(document).on('click', '.emp-leave-cancel', function (event) {
    event.preventDefault();
    var leaveNum = $(this).attr("data-leavenum");
    $.ajax({
        url: '/empleave/empleavecancel',
        type: 'POST',
        dataType: 'json',
        data: { leavenumber: leaveNum },
        success: function (response) {
            GetEmpLeaveHistory();
            if (response.StatusCode == 200) {
                showMessageModal(response.Message, true);
            }
            else {
                showMessageModal(response.Message, false);
            }
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});

//$('#leaveHistoryTable').on('click', '.leave-edit-history', function () {
//    $(this).siblings('.emp-leaveoptions').toggle();
//});

////$(document).on('click', function (event) {
////    if (!$(event.target).closest('.leave-edit-history, #emp-leaveoptions').length) {
////        $('#emp-leaveoptions').hide();
////    }
////});
//$(document).on('click', function () {
//    $('.emp-leaveoptions').hide();
//});
$(document).ready(function () {
    $('#leaveHistoryTable').on('click', '.leave-edit-history', function (event) {
        // Prevent the click event from propagating to the document
        event.stopPropagation();
        // Hide all other .emp-leaveoptions elements
        $('.emp-leaveoptions').hide();
        // Toggle the current .emp-leaveoptions element
        $(this).siblings('.emp-leaveoptions').toggle();
    });

    // Hide .emp-leaveoptions when clicking anywhere else on the document
    $(document).on('click', function () {
        $('.emp-leaveoptions').hide();
    });

    // Prevent hiding when clicking inside .emp-leaveoptions
    $('#leaveHistoryTable').on('click', '.emp-leaveoptions', function (event) {
        event.stopPropagation();
    });
});


//Leave Tracker apply leave
$(document).on('click', '.btn-apply-leave', function (event) {
    event.preventDefault();
    HighlightEmpActiveLink($(this));
    $.ajax({
        url: '/EmpLeave/empapplyleave',
        type: 'POST',
        dataType: 'html',
        success: function (response) {
            $(".hiddenempdashboard").html("");
            $(".emp-dashboard-data").html("");
            $(".selfservice-dashboard-data").html("");
            $(".attedance-dashboard-data").html("");
            $(".leave-dashboard-data").html("");
            $(".myrequest-dashboard-data").html("");
            $(".hiddenempdashboard").html(response);
            var formContent = $(".hiddenempdashboard").find(".employeeleaveapply-view").html();
            $(".leave-dashboard-data").html(formContent);
            $('.leave-dashboard-data').show();
            $('.attedance-dashboard-data').hide();
            $('.emp-dashboard-data').hide();
            $('.selfservice-dashboard-data').hide();
            $('.myrequest-dashboard-data').hide();
            $(".hiddenempdashboard").html("");
            $('.div-leave-empname').hide();
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});

//on page load
generateLeaveCalendar(new Date().getMonth(), new Date().getFullYear());
//LeaveCarousel();
