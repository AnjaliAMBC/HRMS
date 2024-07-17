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



$(document).off('change', '#myLeaveMonth').on('change', '#myLeaveMonth', function (event) {
    event.preventDefault();
    $('.show-progress').show();
    var selectedMonth = new Date($('#myLeaveMonth').val());
    var startDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth(), 1);
    var endDate = new Date(selectedMonth.getFullYear(), selectedMonth.getMonth() + 1, 0);
    var formattedStartDate = formatDateToCustomString(startDate);
    var formattedEndDate = formatDateToCustomString(endDate);
    AdminLeaveDataMap(formattedStartDate, "");
    $('.show-progress').hide();
    //updateLeaveDays(formattedStartDate);
});


function ApproveLeaveRequest(leaveName, element) {
    $.ajax({
        url: '/EmpLeave/ApproveLeave',
        type: 'POST',
        data: { leaveRequestName: leaveName },
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            if (response.success) {
                element.parent().html('<small style="color: green">Approved</small>');
            } else {
                alert('Failed to approve leave.');
            }
            $('.show-progress').hide();
        }.bind(this),
        complete: function () {
            $('.show-progress').hide();
        }
    });
}

$('.btn_approve').click(function () {
    var leaveName = $(this).data('leavename');
    ApproveLeaveRequest(leaveName, $(this))
});

function RejectLeaveRequest(leaveName, element) {
    $.ajax({
        url: '/EmpLeave/RejectLeave',
        type: 'POST',
        data: { leaveRequestName: leaveName },
        success: function (response) {
            if (response.success) {
                element.parent().html('<small style="color: red">Rejected</small>');
            } else {
                alert('Failed to reject leave.');
            }
        }.bind(this)
    });
}

$('.btn_reject').click(function () {
    var leaveName = $(this).data('leavename');
    RejectLeaveRequest(leaveName, $(this));
});


$(document).off('click', '.acceptLeaveBtn').on('click', '.acceptLeaveBtn', function (event) {
    event.preventDefault();
    var leaveName = $(this).data('leavename');
    var $statusBtnFlex = $(this).closest('.statusBtnFlex');
    var $statusLabel = $statusBtnFlex.siblings('.statusLabel');

    $.ajax({
        url: '/EmpLeave/ApproveLeave',
        type: 'POST',
        data: { leaveRequestName: leaveName },
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            if (response.success == true) {
                //var $statusLabel = $('<small class="statusLabel">Approved</small>');
                //$statusBtnFlex.append($statusLabel);
                //$statusBtnFlex.find('.statusBtn').hide();

                //$statusBtnFlex.find('.statusLabel').text('Approved').show();
                ////$statusBtnFlex.append($statusLabel);
                //$statusBtnFlex.find('.statusBtn').hide();
                //$statusBtnFlex.find('.changestatusapprove').hide();

                $statusBtnFlex.empty();

                // Check if admin-leaveapprovaloptions div exists, if not create it
                if ($statusBtnFlex.find('.admin-leaveapprovaloptions').length === 0) {
                    var adminLeaveApprovalOptions = `
                         <small class="statusLabel" style="color: green">Approved</small>
                        <i class="fas fa-ellipsis-h adminleave-leave-approval" onclick="toggleAdminLeaveApprovalActionOptions(this)"></i>
                        <div class="admin-leaveapprovaloptions" style="display:none">
                            <a href="" class="dropdown-item rejectLeaveBtn changestatusreject" data-leavename="${leaveName}" data-compoffnum="${leaveName}">Change Status</a>
                        </div>
                    `;
                    $statusBtnFlex.append(adminLeaveApprovalOptions);
                }

            } else {
                alert('Failed to update the status. Please try again.');
            }
            $('.show-progress').hide();
        },
        error: function () {
            alert('An error occurred. Please try again.');
        },
        complete: function () {
            $('.show-progress').hide();
        }
    });
});


$(document).off('click', '.rejectLeaveBtn').on('click', '.rejectLeaveBtn', function (event) {
    event.preventDefault();
    var leaveName = $(this).data('leavename');
    var $statusBtnFlex = $(this).closest('.statusBtnFlex');
    //var $statusLabel = $statusBtnFlex.siblings('.statusLabel');

    $.ajax({
        url: '/EmpLeave/RejectLeave',
        type: 'POST',
        data: { leaveRequestName: leaveName },
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            if (response.success == true) {
                /*  var $statusLabel = $('<small class="statusLabel">Rejected</small>');*/
                //$statusBtnFlex.find('.statusLabel').text('Rejected').show();
                ////$statusBtnFlex.append($statusLabel);
                //$statusBtnFlex.find('.statusBtn').hide();
                //$statusBtnFlex.find('.changestatusreject').hide();

                $statusBtnFlex.empty();

                if ($statusBtnFlex.find('.admin-leaveapprovaloptions').length === 0) {
                    var adminLeaveApprovalOptions = `
                        <small class="statusLabel" style="color: red">Rejected</small>
                        <i class="fas fa-ellipsis-h adminleave-leave-approval" onclick="toggleAdminLeaveApprovalActionOptions(this)"></i>
                        <div class="admin-leaveapprovaloptions" style="display:none">
                            <a href="" class="dropdown-item acceptLeaveBtn changestatusapprove" data-leavename="${leaveName}" data-compoffnum="${leaveName}">Change Status</a>
                        </div>
                    `;
                    $statusBtnFlex.append(adminLeaveApprovalOptions);
                }
            } else {
                alert('Failed to update the status. Please try again.');
            }

            $('.show-progress').hide();
        },
        error: function () {
            alert('An error occurred. Please try again.');
        },
        complete: function () {
            $('.show-progress').hide();
        }
    });
});


function toggleAdminLeaveApprovalActionOptions(iconElement) {
    const optionsMenu = $(iconElement).next('.admin-leaveapprovaloptions');
    $('.admin-leaveapprovaloptions').not(optionsMenu).hide();
    optionsMenu.toggle();
}



$(document).off('click', '.AdminIndiEmpLeave-History').on('click', '.AdminIndiEmpLeave-History', function (event) {
    $('.show-progress').show();
    window.location.href = "/adminleave/AdminEmpLeaveCalender";
    //$.ajax({
    //    url: '/adminleave/AdminEmpLeaveCalender',
    //    type: 'GET',
    //    dataType: 'html',
    //    success: function (response) {
    //        $(".hiddenadmindashboard").html("");
    //        $('.admin-dashboard-container').html("");
    //        $(".admin-emppadd-container").html("");
    //        $('.admin-empmanagement-container').html("");
    //        $('.admin-attendance-container').html("");
    //        $('.admin-leave-container').html("");
    //        $('.admin-leave-management-view').html("");
    //        $(".admin-leaveHistory-container").html("");

    //        $(".hiddenadmindashboard").html(response);
    //        var formContent = $(".hiddenadmindashboard").find(".AdminLeaveEmpCalender-Page").html();
    //        $(".admin-leaveHistory-container").html(formContent);

    //        $('.admin-empmanagement-container').hide();
    //        $('.admin-dashboard-container').hide();
    //        $('.admin-emppadd-container').hide();
    //        $('.admin-attendance-container').hide();
    //        $('.admin-leave-container').show();
    //        $('.admin-ticketing-container').hide();
    //        $('.admin-leave-container').hide();
    //        $(".hiddenadmindashboard").html("");

    //        setTimeout(function () {
    //            $('.show-progress').show();
    //            fetchLeaveHolidays();
    //            $('.show-progress').hide();
    //        }, 1000);
    //    },
    //    error: function (xhr, status, error) {
    //        var err = eval("(" + xhr.responseText + ")");
    //    }
    //});

});

$(document).on('click', '.admin-empbased-leave-calender', function (event) {

    event.preventDefault();
    $('.show-progress').show();

    var selectedEmployee = $(this).text();
    var empId = $(this).data('empid');

    $('.LeaveEmp-dropdowntoggle').text(selectedEmployee);
    $('.LeaveEmp-dropdowntoggle').attr('data-empid', empId);

    fetchLeaveHolidays(empId);
    $('.show-progress').hide();
});

//admin Leave History Calender table click 

$(document).off('click', '.Admin-leavehistory-icon').on('click', '.Admin-leavehistory-icon', function (event) {
    event.preventDefault();
    $('.show-progress').show();
    window.location.href = "/adminleave/adminleavehistory";
  
    //$.ajax({
    //    url: '/adminleave/adminleavehistory',
    //    type: 'GET',
    //    dataType: 'html',
    //    data: { startdate: $('#leavehistory-fromDate').val(), endDate: $('#leavehistory-toDate').val(), department: "", location: $('#leavehistory-Location-dropdown').val(), status: $('#leavehistory-status-dropdown').val() },
    //    success: function (response) {
    //        $(".hiddenadmindashboard").html("");
    //        $('.admin-dashboard-container').html("");
    //        $(".admin-emppadd-container").html("");
    //        $('.admin-empmanagement-container').html("");
    //        $('.admin-attendance-container').html("");
    //        $('.admin-leave-container').html("");
    //        $('.admin-leaveHistory-container').html("");
    //        $(".hiddenadmindashboard").html(response);
    //        var formContent = $(".hiddenadmindashboard").find(".admin-leaveHistory-view").html();
    //        $(".admin-leaveHistory-container").html(formContent);
    //        $('.admin-attendance-container').hide();
    //        $('.admin-empmanagement-container').hide();
    //        $('.admin-emppadd-container').hide();
    //        $('.admin-dashboard-container').hide();
    //        $('.admin-ticketing-container').hide();
    //        $('.admin-leave-container').hide();
    //        $(".hiddenadmindashboard").html("");
    //    },
    //    error: function (xhr, status, error) {
    //        console.error("Error deleting employee:", error);
    //    }
    //});
});


$(document).off('click', '.btn-import-leaves-hstory').on('click', '.btn-import-leaves-hstory', function (event) {
    event.preventDefault();

    $.ajax({
        url: '/adminleave/AdminLeavesHistoryImport',
        type: 'GET',
        dataType: 'html',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            $(".hiddenadmindashboard").html("");       
            $(".admin-leaveHistory-container").html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".adminEmp-LeaveHistoryImport-view").html();
            $(".admin-leaveHistory-container").html(formContent);
            $(".hiddenadmindashboard").html("");
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error("Error deleting employee:", error);
        }
    });
});


$(document).off('click', '.btn-importleavehistrory-submit').on('click', '.btn-importleavehistrory-submit', function (event) {
    var file = $('#leaveistory-file-upload-input')[0].files[0];
    var formData = new FormData();
    formData.append('file', file);
    $.ajax({
        url: '/adminleave/UploadLeavesHistoryExcel',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                console.log(response.data);
                $('#modalMessage').text(response.message);
                $('#messageModal').modal('show');
            } else {
                $('#modalMessage').text(response.message);
                $('#messageModal').modal('show');
            }
        },
        error: function (xhr, status, error) {
            $('#modalMessage').text('Error uploading file: ' + error);
            $('#messageModal').modal('show');
        }
    });
});


function handleLeaveFileUpload(input) {
    var uploadedFile = input.files[0];
    var uploadedFileName = uploadedFile.name;

    // Display the uploaded file name
    document.getElementById('leave-uploaded-file-text').textContent = 'File Uploaded: ' + uploadedFileName;
    document.getElementById('leave-uploaded-file-info').style.display = 'block';
}



