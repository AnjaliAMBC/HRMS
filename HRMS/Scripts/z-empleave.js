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
                cell.classList.add("btn-apply-leave1"); // Add a class for jQuery

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
    } else {
        historyBtn.textContent = "Leave History";
        icon.classList.remove("fa-calendar-alt");
        icon.classList.add("fa-clock-rotate-left");
        cardDiv.style.display = 'block';
        leaveHistory.style.display = 'none';
    }
}
//history table

$(function () {
    $('#leaveHistoryTable').DataTable({
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
        }
    });
});

$(document).on('click', '.leave-edit-history', function (event) {
    $('#emp-leaveoptions').toggle();
});
$(document).on('click', function (event) {
    if (!$(event.target).closest('.leave-edit-history, #emp-leaveoptions').length) {
        $('#emp-leaveoptions').hide();
    }
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
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});

//on page load
generateLeaveCalendar(new Date().getMonth(), new Date().getFullYear());
LeaveCarousel();
