$(document).on('click', '.adminleave-Balanve-link', function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));
    $.ajax({
        url: '/adminleave/adminleavebalance',
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leaveBalance-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            console.error("Error deleting employee:", error);
        }
    });
});


//leave balance update page 

$(document).on('click', '.employeeinfo-balance', function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));
    var employeeId = $(this).find('.employee-id').text();
    $.ajax({
        url: '/adminleave/adminleavebalanceupdate',
        type: 'GET',
        dataType: 'html',
        data: { selectedEmpID: employeeId },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leaveBalanceUpdate-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            console.error("Error deleting employee:", error);
        }
    });
});

//Admin Leave History of all employees 
$(document).on('click', '.adminleave-history', function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));
    $.ajax({
        url: '/adminleave/adminLalleavehistory',
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $('.admin-dashboard-container').html("");
            $(".admin-emppadd-container").html("");
            $('.admin-empmanagement-container').html("");
            $('.admin-attendance-container').html("");
            $('.admin-leave-container').html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-leaveHistory-view").html();
            $(".admin-leave-container").html(formContent);
            $('.admin-leave-container').show();
            $('.admin-attendance-container').hide();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            console.error("Error deleting employee:", error);
        }
    });
});
