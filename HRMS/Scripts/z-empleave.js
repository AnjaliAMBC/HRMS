﻿function LeaveCarousel() {
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

function generateLeaveHolidays(holidays) {
    const leaveholidays = {};

    holidays.forEach(holiday => {
        const date = new Date(holiday.holiday_date);
        const key = `${date.getMonth() + 1}-${date.getDate()}`; // 'MM-DD' format
        leaveholidays[key] = holiday.holiday_name;
    });

    return leaveholidays;
}

function fetchLeaveHolidays(selectedempid) {
    updateLeaveCalendar(selectedempid);
}

function updateLeaveCalendar(selectedempid) {
    generateLeaveCalendar(currentLeaveMonth, currentLeaveYear, selectedempid);
}

// Utility function to format date as YYYY-MM-DD
function formatDateyyyyMMdd(date) {
    var yyyy = date.getFullYear();
    var mm = String(date.getMonth() + 1).padStart(2, '0'); // Months are zero-based
    var dd = String(date.getDate()).padStart(2, '0');
    return yyyy + '-' + mm + '-' + dd;
}

function generateLeaveCalendar(month, year, selectedempid) {

    var isAdminLeavePage = false;
    var linktoleavecalender = "btn-apply-leave1";

    if ($('div.admin-leaveHistory-container').length || $('div.admin-leave-container').length) {
        isAdminLeavePage = true;
        linktoleavecalender = "btn-admin-apply-leave1";
    }

    var empID = isAdminLeavePage ? "" : $('.loggedinempid').text();

    if (selectedempid != undefined && selectedempid != "") {
        isAdminLeavePage = false;
        empID = selectedempid;
        linktoleavecalender = "btn-admin-emp-apply-leave";
    }

    if ($('div.LeaveEmpHistory-calender').length) {
        isAdminLeavePage = false;
        linktoleavecalender = "btn-admin-emp-apply-leave1";
    }



    if (isAdminLeavePage == true) {
        $.ajax({
            url: '/adminleave/admincalenderleavemanagement',
            type: 'POST',
            dataType: 'json',
            data: { month: month, year: year, empID: empID },
            success: function (response) {
                const leaveData = $.parseJSON(response);
                const leaveRequests = {};
                leaveData.forEach(leave => {
                    const leaveDate = new Date(leave.LatestLeave.createddate).toISOString().split('T')[0];
                    if (!leaveRequests[leaveDate]) {
                        leaveRequests[leaveDate] = [];
                    }
                    leaveRequests[leaveDate].push(leave);
                });

                $.ajax({
                    url: '/adminleave/getholidaysbasedonlocation',
                    type: 'POST',
                    dataType: 'json',
                    data: { empid: $('.loggedinempid').text(), month: month, year: year },
                    success: function (holidayresponse) {
                        console.log(holidayresponse);

                        const holidays = $.parseJSON(holidayresponse);
                        const leaveholidays = generateLeaveHolidays(holidays);

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
                                    const fullDate = new Date(year, month, date);
                                    const formattedDate = fullDate.toISOString().split('T')[0]; // YYYY-MM-DD format

                                    const formattedMonthDate = fullDate.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' });

                                    cell.textContent = date;
                                    cell.setAttribute("data-date", formattedMonthDate); // Set the data-date attribute
                                    cell.classList.add(linktoleavecalender);

                                    const holidayKey = `${month + 1}-${date}`;
                                    if (leaveRequests[formattedDate]) {
                                        const leaveDiv = document.createElement("div");
                                        leaveDiv.classList.add("empleave-leave-request");
                                        leaveDiv.style.backgroundColor = "#e7f3fe"; // Add background color to leaveDiv

                                        leaveRequests[formattedDate].forEach((leave, index) => {
                                            if (index < 2) { // Show up to 2 images directly
                                                const img = document.createElement("img");
                                                img.src = `/Assets/EmpImages/${leave.LatestLeave.employee_id}.jpeg`; // Adjust the image path as needed
                                                img.alt = leave.employee_name;
                                                img.title = leave.employee_name;
                                                leaveDiv.appendChild(img);
                                            } else if (index === 2) { // Show the count of additional leave requests
                                                const moreDiv = document.createElement("div");
                                                moreDiv.classList.add("more-count");
                                                moreDiv.textContent = `+ ${leaveRequests[formattedDate].length - 2}`;
                                                leaveDiv.appendChild(moreDiv);
                                            }
                                        });

                                        cell.appendChild(leaveDiv);
                                        cell.classList.add("highlight-leave");
                                        cell.style.backgroundColor = "#e7f3fe"; // Add background color to parent td
                                    }
                                    if (date === today.getDate() && year === today.getFullYear() && month === today.getMonth()) {
                                        cell.classList.add("highlight-currentleave");
                                    } else if (leaveholidays[holidayKey] != undefined && leaveholidays[holidayKey] != "") {
                                        cell.classList.add("highlight-festival-leave");
                                        cell.innerHTML += `<div><i class="fa fa-circle"></i> ${leaveholidays[holidayKey]}</div>`;
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
                });
            },
            error: function (xhr, status, error) {
                var err = eval("(" + xhr.responseText + ")");
            }
        });

    }
    else {
        var currentDate = new Date();

        var firstDay = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
        var firstDayFormatted = formatDateyyyyMMdd(firstDay);

        var lastDay = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
        var lastDayFormatted = formatDateyyyyMMdd(lastDay);


        if (year && month && month >= 1 && month <= 12) {
            firstDayFormatted = formatDateyyyyMMdd(new Date(year, month, 1));
            lastDayFormatted = formatDateyyyyMMdd(new Date(year, month + 1, 0));
        }

        /*const leaveTypeColors = {
            "Earned Leave": "#A5E2E9", 
            "Emergency Leave": "#C4BEF4", 
            "Sick Leave": "#ACFF97", 
            "Bereavement Leave": "#FFC297", 
            "Hourly Permission": "#BBCDF3", 
            "Marriage Leave": "#A8E4E2",
            "Maternity Leave": "#FEA8B5",
            "Paternity Leave": "#F4D8C0",
            "Comp Off": "#F8CADC"
        };*/

        const leaveTypeColors = {
            "Earned Leave": "#1cDDDc",
            "Emergency Leave": "#A5D8EB",
            "Sick Leave": "#FA9F9E",
            "Bereavement Leave": "#B4C1C7",
            "Hourly Permission": "#C4BEF4",
            "Marriage Leave": "#BDD7EE",
            "Maternity Leave": "#AEF0FF",
            "Paternity Leave": "#FEBDD1",
            "Comp Off": "#9FB6FD"
        };

        $.ajax({
            url: '/empleave/GetAllEmpLeavesInfoBasedonDate',
            type: 'POST',
            dataType: 'json',
            data: { startdate: firstDayFormatted, enddate: lastDayFormatted, leaverequestname: "", empID: empID },
            success: function (response) {
                const leaveData = $.parseJSON(response);
                const leaveRequests = {};
                leaveData.forEach(leave => {
                    const leaveDate = new Date(leave.leavedate).toISOString().split('T')[0];
                    if (!leaveRequests[leaveDate]) {
                        leaveRequests[leaveDate] = [];
                    }
                    leaveRequests[leaveDate].push(leave);
                });

                $.ajax({
                    url: '/adminleave/getholidaysbasedonlocation',
                    type: 'POST',
                    dataType: 'json',
                    data: { empid: $('.loggedinempid').text(), month: month, year: year },
                    success: function (holidayresponse) {
                        console.log(holidayresponse);

                        const holidays = $.parseJSON(holidayresponse);
                        const leaveholidays = generateLeaveHolidays(holidays);

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
                                    const fullDate = new Date(year, month, date);
                                    const formattedDate = fullDate.toISOString().split('T')[0]; // YYYY-MM-DD format

                                    const formattedMonthDate = fullDate.toLocaleDateString('en-GB', { day: '2-digit', month: 'long', year: 'numeric' });

                                    cell.textContent = date;
                                    cell.setAttribute("data-date", formattedMonthDate); // Set the data-date attribute
                                    cell.classList.add(linktoleavecalender);

                                    const holidayKey = `${month + 1}-${date}`;
                                    if (date === today.getDate() && year === today.getFullYear() && month === today.getMonth()) {
                                        cell.classList.add("highlight-currentleave");
                                    } else if (leaveholidays[holidayKey] != undefined && leaveholidays[holidayKey] != "") {
                                        cell.classList.add("highlight-festival-leave");
                                        cell.innerHTML += `<div><i class="fa fa-circle"></i>${leaveholidays[holidayKey]}</div>`;
                                    }

                                    if (leaveRequests[formattedDate]) {
                                        const leaveDiv = document.createElement("div");
                                        leaveDiv.classList.add("empleave-leave-request");
                                        leaveDiv.style.display = "flex";
                                        leaveDiv.style.gap = "2px";
                                        leaveDiv.style.height = "100%"; // Ensure the leaveDiv fills the entire cell
                                        leaveDiv.style.alignItems = "center"; // Center the text vertically

                                        leaveRequests[formattedDate].forEach(leave => {
                                            const leaveBar = document.createElement("div");
                                            leaveBar.style.backgroundColor = leaveTypeColors[leave.leavesource] || "#e7f3fe"; // Use default color if type not found
                                            leaveBar.style.height = "100%"; // Ensure the leaveBar fills the entire height of leaveDiv
                                            leaveBar.style.flex = "1";
                                            leaveBar.textContent = leave.leavesource; // Display leave type in the bar
                                            leaveBar.style.color = "#000"; // Set text color
                                            leaveBar.style.fontSize = "10px"; // Adjust text size
                                            leaveBar.style.display = "flex";
                                            leaveBar.style.alignItems = "center";
                                            leaveBar.style.justifyContent = "center"; // Center the text horizontally
                                            leaveDiv.appendChild(leaveBar);
                                        });

                                        cell.appendChild(leaveDiv);
                                        cell.classList.add("highlight-leave");
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
                });
            }
        });
    }
}

function prevLeaveMonth() {
    currentLeaveMonth--;
    if (currentLeaveMonth < 0) {
        currentLeaveMonth = 11;
        currentLeaveYear--;
    }

    if ($('.LeaveEmpHistory-calender').length) {
        var empId = $('.LeaveEmp-dropdowntoggle').attr('data-empid');
        updateLeaveCalendar(empId);
    } else {
        updateLeaveCalendar();
    }


}

function nextLeaveMonth() {
    currentLeaveMonth++;
    if (currentLeaveMonth > 11) {
        currentLeaveMonth = 0;
        currentLeaveYear++;
    }
    if ($('.LeaveEmpHistory-calender').length) {
        var empId = $('.LeaveEmp-dropdowntoggle').attr('data-empid');
        updateLeaveCalendar(empId);
    } else {
        updateLeaveCalendar();
    }

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
    return `${formattedDate}`;
}

function formatJSONDateDay(jsonDate) {
    const date = new Date(parseInt(jsonDate.replace(/\/Date\((.*?)\)\//, '$1')));
    const dateOptions = { day: 'numeric', month: 'long', year: 'numeric' };
    const dayOptions = { weekday: 'long' };
    const day = date.toLocaleDateString('en-GB', dayOptions);
    return `${day}`;
}

//function GetEmpLeaveHistory() {
//    $.ajax({
//        url: '/empLeave/EmpLeaveHistory',
//        type: 'POST',
//        dataType: 'json',
//        data: { empId: $('.loggedinempid').text(), year: 2024 },
//        success: function (response) {
//            const tableData = response.map(item => {
//                return [
//                    formatJSONDate(item.Fromdate) + ' - ' + formatJSONDate(item.Todate),
//                    item.TotalLeaveDays,
//                    item.LatestLeave.leavesource,
//                    `<span style="color:forestgreen">${item.LatestLeave.LeaveStatus}</span>`,
//                    `<i class="fa-solid fa-message mb-3" style="color: #6b8dda;"></i>
//                             <p class="ml-3" style="margin:0;">${item.LatestLeave.leave_reason}</p>`,
//                    `<i class="fas fa-ellipsis-h leave-edit-history" id="toggleOptions"></i>
//                             <div class="emp-leaveoptions" id="emp-leaveoptions" style="display:none">
//                                 <a class="dropdown-item emp-leave-edit" href="#" data-leavenum='${item.LatestLeave.leaveno}'>Edit</a>
//                                 <a class="dropdown-item emp-leave-cancel" href="#" data-leavenum='${item.LatestLeave.leaveno}'>Cancel</a>
//                             </div>`
//                ];
//            });

//            if ($.fn.DataTable.isDataTable('#leaveHistoryTable')) {
//                $('#leaveHistoryTable').DataTable().clear().destroy();
//            }

//            $('#leaveHistoryTable').DataTable({
//                data: tableData,
//                paging: true,
//                pageLength: 8,
//                searching: false,
//                ordering: false,
//                info: false,
//                lengthChange: false,
//                dom: 'rt<"bottom"p><"clear">',
//                language: {
//                    paginate: {
//                        next: 'Next',
//                        previous: 'Previous'
//                    }
//                },
//                columns: [
//                    { title: "Date", width: "15%" },
//                    { title: "Day(s)", width: "5%" },
//                    { title: "Leave Type", width: "15%" },
//                    { title: "Status", width: "15%" },
//                    { title: "Comment", width: "30%" },
//                    { title: "Actions" }
//                ]
//            });

//        },
//        error: function (xhr, status, error) {
//            console.error(error);
//        }
//    });
//}


function GetEmpLeaveHistory() {
    $.ajax({
        url: '/empLeave/EmpLeaveHistory',
        type: 'POST',
        dataType: 'json',
        data: {
            empId: $('.loggedinempid').text(),
            year: $('#leavehistoryyear').val()
        },
        success: function (response) {
            let tableRows = response.map(item => {
                const fromDate = formatJSONDate(item.Fromdate);
                const toDate = formatJSONDate(item.Todate);
                const fromDay = formatJSONDateDay(item.Fromdate);
                const toDay = formatJSONDateDay(item.Todate);

                var today = new Date();
                const dateDisplay = fromDate === toDate || item.LatestLeave.leavesource == "Hourly Permission"
                    ? `<p class="mb-0 fontWtMedium">${fromDate}</p><span class="mutedText">${fromDay}</span>`
                    : `<p class="mb-0 fontWtMedium">${fromDate} - ${toDate}</p><span class="mutedText">${fromDay} - ${toDay}</span>`;

                const leaveStatus = item.LatestLeave.LeaveStatus.toLowerCase();
                const statusClass = leaveStatus === 'approved' ? 'status-approved' :
                    leaveStatus === 'cancelled' || leaveStatus === 'rejected' ? 'status-cancelled' :
                        leaveStatus === 'pending' ? 'status-pending' : '';

                let leaveActions;

                var fromDateInDateFormat = new Date(fromDate);

                // Determine the leave actions to show based on the date comparison
                if (today > fromDateInDateFormat) {
                    leaveActions = (leaveStatus === 'pending') ? `
        <i class="fas fa-ellipsis-h leave-edit-history" onclick="toggleLeaveActionOptions(this)"></i>
        <div class="emp-leaveoptions" style="display:none">
            <a class="dropdown-item emp-leave-edit" onclick="empleaveedit($(this))" data-leavename='${item.LatestLeave.LeaveRequestName}'>Edit</a>
        </div>` : '';
                } else {
                    leaveActions = (leaveStatus === 'pending') ? `
        <i class="fas fa-ellipsis-h leave-edit-history" onclick="toggleLeaveActionOptions(this)"></i>
        <div class="emp-leaveoptions" style="display:none">
            <a class="dropdown-item emp-leave-edit" onclick="empleaveedit($(this))" data-leavename='${item.LatestLeave.LeaveRequestName}'>Edit</a>
            <a class="dropdown-item emp-leave-cancel" onclick="empleavecancel($(this))" data-leavename='${item.LatestLeave.LeaveRequestName}'>Cancel</a>
        </div>` : '';
                }


                return `
                    <tr class="rowBorder">
                        <td class="fontSmall dataMinWidth dateMaxWidth">
                            ${dateDisplay}
                        </td>
                        <td class="fontSmall">
                            <div class="mutedText">Day(s)</div>
                            <span class="fontWtMedium">${item.TotalLeaveDays}</span>
                        </td>
                        <td class="fontSmall dataMinWidth">
                            <div class="mutedText">Leave Type</div>
                            <span class="fontWtbold" style="font-weight:bold;">${item.LatestLeave.leavesource}</span>
                        </td>
                        <td class="fontSmall">
                            <div class="mutedText">Status</div>
                            <span class="fontWtMedium ${statusClass}">${item.LatestLeave.LeaveStatus}</span>
                        </td>
                        <td style="width: 420px;" class="fontSmall commentSec">
                            <div class="dFlex d-flex">
                                <i class="fa-solid fa-message chatIcon mt-1 mr-2"></i>
                                <p title="${item.LatestLeave.leave_reason}">${item.LatestLeave.leave_reason}</p>
                            </div>                            
                        </td>
                        <td class="fontSmall position-relative">
                            ${leaveActions}
                        </td>
                    </tr>
                `;
            }).join('');

            $('#leaveHistoryTable tbody').html(tableRows);

            if (!$.fn.DataTable.isDataTable('#leaveHistoryTable')) {
                $('#leaveHistoryTable').DataTable({
                    "paging": true,
                    "searching": true,
                    "ordering": false,
                    "info": true,
                    "autoWidth": false,
                    "lengthMenu": [[7, 14, 21, -1], [7, 14, 21, "All"]],
                    "columnDefs": [
                        { "orderable": false, "targets": 1 }, // Disable ordering on the Checkin-Check-Out column
                        { "orderable": false, "targets": 3 }  // Disable ordering on the Status column
                    ]
                });
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}


function toggleEmpCompoffActionOptions(iconElement) {
    const optionsMenu = $(iconElement).next('.emp-compoffs');
    $('.emp-compoffs').not(optionsMenu).hide(); // Hide any other open menus
    optionsMenu.toggle(); // Toggle visibility of the clicked menu
}



function GetCompOffHistory() {
    $.ajax({
        url: '/adminleave/EmployeeLeaveCompensatoryOff',
        type: 'GET',
        dataType: 'json',
        data: {
            fromDate: $('#fromDate').val(),
            todate: $('#toDate').val(),
            empID: $('.loggedinempid').text()
        },
        success: function (response) {
            var json = $.parseJSON(response);
            let tableRows = json.CompOffs.map(item => {
                const requestedDate = item.createddate ? new Date(item.createddate).toLocaleDateString('en-GB', { day: 'numeric', month: 'long', year: 'numeric' }) : "";
                const workDate = item.CampOffDate ? new Date(item.CampOffDate).toLocaleDateString('en-GB', { day: 'numeric', month: 'long', year: 'numeric' }) : "";

                const leaveStatus = item.addStatus.toLowerCase();

                const statusClass = leaveStatus === 'approved' ? 'compoff-status-approved' :
                    leaveStatus === 'rejected' ? 'compoff-status-rejected' :
                        leaveStatus === 'cancelled' ? 'compoff-status-cancelled' :
                            leaveStatus === 'pending' ? 'compoff-status-pending' : '';

                const leaveActions = (leaveStatus === 'pending') ? `
                    <i class="fas fa-ellipsis-h" onclick="toggleEmpCompoffActionOptions(this)"></i>
                    <div class="emp-compoffs" style="display:none">                       
                        <a href="" class="dropdown-item statusBtn cancelLeavecompoffBtn" data-leavename="Cancel" data-compoffnum="${item.Compoff_no}">Cancel</a>                       
                    </div>` : '';

                return `
                   <tr>
                <td style="display:none">${item.EmployeeID}</td>
                <td style="display:none">${item.EmployeeName}</td>
                <td>
                    <div class="mutedText">Requested Date</div>
                    <span class="fontWtMedium">${requestedDate}</span>
                </td>
                <td>
                     <div class="mutedText">Work Date</div>
                     <span class="fontWtMedium">${workDate}</span>
                </td>
                <td class="compoff-status">
                    <div class="mutedText">Status</div>
                    <span class="fontWtMedium ${statusClass}">${item.addStatus}</span>
                </td>                
                <td style="width: 420px;" class="fontSmall commentSec">
                    <div class="dFlex d-flex">
                        <i class="fa-solid fa-message chatIcon mt-1 mr-2"></i>
                        <p title="${item.Reason}">${item.Reason}</p>
                    </div>
                </td>
                <td class="fontSmall position-relative compoff-actions">
                    ${leaveActions}
                </td>
            </tr>
                `;
            }).join('');

            $('#empleaveCompOffTable tbody').html(tableRows);

            if (!$.fn.DataTable.isDataTable('#empleaveCompOffTable')) {
                $('#empleaveCompOffTable').DataTable({
                    "paging": true,
                    "searching": false,
                    "ordering": true,
                    "info": true,
                    "autoWidth": false,
                    "lengthMenu": [[7, 14, 21, -1], [7, 14, 21, "All"]],
                    "columnDefs": [
                        { "orderable": true, "targets": 0 },
                        { "orderable": true, "targets": 2 },
                        { "orderable": true, "targets": 3 }
                    ]
                });
            }
        },
        error: function (xhr, status, error) {
            console.error(error);
        }
    });
}



function toggleLeaveActionOptions(iconElement) {
    const optionsMenu = $(iconElement).next('.emp-leaveoptions');
    $('.emp-leaveoptions').not(optionsMenu).hide(); // Hide any other open menus
    optionsMenu.toggle(); // Toggle visibility of the clicked menu
}



function empleaveedit(currentthis) {
    var leaveName = currentthis.attr("data-leavename");

    AddEditLeaves(leaveName);

    //$.ajax({
    //    url: '/empLeave/empleaveedit',
    //    type: 'POST',
    //    dataType: 'json',
    //    data: { leavenumber: leaveName },
    //    success: function (data) {
    //        $('#leaveempname').val(data.employee_id).change();
    //        $('#leaveType').val(data.leavesource);
    //        $('#fromleaveDate').val(formatDate1(data.Fromdate));
    //        $('#toleaveDate').val(formatDate1(data.Todate));
    //        $('#reason').val(data.leave_reason);
    //        $('#BackupName').val(data.BackupResource_Name);
    //        $('#BackupNo').val(data.EmergencyContact_no);

    //        // Trigger date change event to generate day type rows
    //        $('#fromleaveDate').trigger('change');
    //        $('#toleaveDate').trigger('change');

    //        // Pre-select day types if available
    //        preselectDayTypes(data);

    //        // Show the apply leave form
    //        $('.employeeleaveapply-view').show();
    //    },
    //    error: function (xhr, status, error) {
    //        console.error("Error occurred: " + error);
    //    }
    //});


}


// Add a click event listener for the confirm button in the modal
$('#confirmCancelButton').on('click', function () {
    var leaveName = $(this).data('leavename');

    $.ajax({
        url: '/empLeave/EmpLeaveCancel',
        type: 'POST',
        dataType: 'json',
        data: { leavenumber: leaveName },
        success: function (response) {
            GetEmpLeaveHistory();
            if (response.StatusCode == 200) {
                showMessageModal(response.Message, true, "");
            } else {
                showMessageModal(response.Message, false, "closepouponly");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error occurred: " + error);
        }
    });

    // Hide the modal after the AJAX call is made
    $('#confirmCancelModal').modal('hide');
});


function empleavecancel(currentthis) {
    var leaveName = currentthis.attr("data-leavename");

    // Store leave name in a global variable or data attribute
    $('#confirmCancelButton').data('leavename', leaveName);

    // Show the confirmation modal
    $('#confirmCancelModal').modal('show');
}


//function empleavecancel(currentthis) {
//    var leaveName = currentthis.attr("data-leavename");

//    $.ajax({
//        url: '/empLeave/EmpLeaveCancel',
//        type: 'POST',
//        dataType: 'json',
//        data: { leavenumber: leaveName },
//        success: function (response) {
//            GetEmpLeaveHistory();
//            if (response.StatusCode == 200) {
//                showMessageModal(response.Message, true);
//            } else {
//                showMessageModal(response.Message, false);
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error("Error occurred: " + error);
//        }
//    });

//}

// Cancel leave event
$(document).on('click', '.emp-leave-cancel', function (event) {
    event.preventDefault();


});

// Edit leave event
$(document).on('click', '.emp-leave-edit', function (event) {
    event.preventDefault();
    var leaveNum = $(this).attr("data-leavenum");
    // Handle the edit functionality here
    // You can either open a modal or redirect to an edit page
    // For example:
    window.location.href = '/empLeave/EditLeave?leavenumber=' + leaveNum;
});

$(document).on('click', '.history_btn', function (event) {
    event.preventDefault();
    var historyBtn = document.querySelector('.history_btn');
    var leaveHistory = document.querySelector('.leave-history');
    var cardDiv = document.querySelector('.empleave-card');
    var icon = document.querySelector('.icon-leave');

    if (leaveHistory.style.display === "none") {
        historyBtn.textContent = "View Calendar";
        icon.classList.remove("fa-clock-rotate-left");
        icon.classList.add("fa-calendar");
        cardDiv.style.display = 'none';
        leaveHistory.style.display = 'block';
        GetEmpLeaveHistory();
        GetCompOffHistory();

    } else {
        historyBtn.textContent = "Leave History";
        icon.classList.remove("fa-calendar");
        icon.classList.add("fa-clock-rotate-left");
        cardDiv.style.display = 'block';
        leaveHistory.style.display = 'none';
    }
});

$(document).ready(function () {
    $('#leaveHistoryTable').on('click', '.leave-edit-history', function (event) {
        event.stopPropagation();
        $('.emp-leaveoptions').hide();
        $(this).siblings('.emp-leaveoptions').toggle();
    });

    $(document).on('click', function () {
        $('.emp-leaveoptions').hide();
    });

    $('#leaveHistoryTable').on('click', '.emp-leaveoptions', function (event) {
        event.stopPropagation();
    });
});


//Leave Tracker apply leave
$(document).on('click', '.btn-apply-leave', function (event) {
    event.preventDefault();
    /*   HighlightEmpActiveLink($(this));*/
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


$(document).on('change', '#leavehistoryyear', function () {
    GetEmpLeaveHistory();
});
