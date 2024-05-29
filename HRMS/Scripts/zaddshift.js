$(document).on('click', '.btn-addshift', function () {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $.ajax({
        url: '/adminattendance/addshift',
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
            var formContent = $(".hiddenadmindashboard").find(".admin-addshift-view").html();
            $(".admin-attendance-container").html(formContent);

            $('.admin-attendance-container').show();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});

$(document).on('change', 'input[name="shiftType"]', function () {
    if ($('#departmentRadio').is(':checked')) {
        $('#departmentDropdown').removeClass('d-none');
        $('#employeeDropdown').addClass('d-none');
    } else if ($('#employeesRadio').is(':checked')) {
        $('#employeeDropdown').removeClass('d-none');
        $('#departmentDropdown').addClass('d-none');
    }
});

$(document).on('change', '#selectDepartments', function () {
    $('.multiselect').addClass('bordered');
    initializeMultiselect(this, 'Select Departments');
});

$(document).on('change', '#selectEmployees', function () {
    $('.multiselect').addClass('bordered');
    initializeMultiselect(this, 'Select Employees');
});

function initializeMultiselect(selector, nonSelectedText) {
    $(selector).multiselect('destroy').multiselect({
        includeSelectAllOption: true,
        enableFiltering: true,
        buttonWidth: '100%',
        nonSelectedText: nonSelectedText
    });
}


$(document).on('click', '.btn-addshift-submit', function (e) {
    e.preventDefault();

    var IsDepartmentBasedUpdate = false;

    var selectedIds = [];
    if ($('#departmentRadio').is(':checked')) {
        IsDepartmentBasedUpdate = true;
        selectedIds = $('#selectDepartments').val();
    } else if ($('#employeesRadio').is(':checked')) {
        selectedIds = $('#selectEmployees').val();
    }

    var startTime = $('#startTime').val();
    var endTime = $('#endTime').val();
    var notification = $('#notification').is(':checked');

    $.ajax({
        url: '/adminattendance/addshiftinfotodb',
        method: 'POST',
        data: {
            selectedIds: selectedIds,
            startTime: startTime,
            endTime: endTime,
            notification: notification,
            IsDepartmentBasedUpdate: IsDepartmentBasedUpdate
        },
        success: function (response) {
            console.log(response);
        },
        error: function (xhr, status, error) {
            // Handle error response from server
            console.error(error);
        }
    });
});