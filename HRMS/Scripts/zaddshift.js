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
            //$(".admin-attendancemgmt-view").html("");
            //$(".admin-attendancemgmt-view").html(response)
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
    $('.shift-message').hide();

    var IsDepartmentBasedUpdate = false;
    var selectedIds = [];

    // Get the selected IDs based on the selected radio button
    if ($('#departmentRadio').is(':checked')) {
        IsDepartmentBasedUpdate = true;
        selectedIds = $('#selectDepartments').val();
    } else if ($('#employeesRadio').is(':checked')) {
        selectedIds = $('#selectEmployees').val();
    }

    var startTime = $('#startTime').val();
    var endTime = $('#endTime').val();
    var notification = $('#notification').is(':checked');

    // Remove previous validation error highlights
    $('#selectDepartments, #selectEmployees, #startTime, #endTime').removeClass('is-invalid');
    $('.validation-message').remove();

    var hasError = false;

    // Validate mandatory fields
    if (!selectedIds || selectedIds.length === 0) {
        hasError = true;
        if ($('#departmentRadio').is(':checked')) {
            $('#selectDepartments').addClass('is-invalid');
        } else {
            $('#selectEmployees').addClass('is-invalid');
        }
        $('<div class="validation-message text-danger">Please select at least one department or employee.</div>')
            .insertAfter('#selectDepartments, #selectEmployees');
    }
    if (!startTime) {
        hasError = true;
        $('#startTime').addClass('is-invalid');
        $('<div class="validation-message text-danger">Please enter the start time.</div>')
            .insertAfter('#startTime');
    }
    if (!endTime) {
        hasError = true;
        $('#endTime').addClass('is-invalid');
        $('<div class="validation-message text-danger">Please enter the end time.</div>')
            .insertAfter('#endTime');
    }

    if (hasError) {
        return;
    }

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
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            console.log(response);

            // Clear the form values upon successful response
            $('#selectDepartments').val('');
            $('#selectEmployees').val('');
            $('#startTime').val('');
            $('#endTime').val('');
            $('#notification').prop('checked', false);
            $('#departmentRadio').prop('checked', false);
            $('#employeesRadio').prop('checked', false);
            $('.multiselect-selected-text').text("--Select--");

            selectedIds = [];
            $('.multiselect-container li').removeClass('active');

            $('#selectDepartments').trigger('change');
            $('#selectEmployees').trigger('change');

            // Display the message in the modal
            var message = response.JsonResponse.Message;
            var statusCode = response.JsonResponse.StatusCode;
            var messageColor = statusCode === 200 ? 'green' : 'red';

            $('#addShiftModal .modal-body').html('<p style="color: ' + messageColor + ';">' + message + '</p>');
            $('#addShiftModal').modal('show');

            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            // Handle error response from server
            console.error(error);
            $('#addShiftModal .modal-body').html('<p style="color: red;">An unexpected error occurred. Please try again later.</p>');
            $('#addShiftModal').modal('show');
        },
        complete: function () {
            $('.show-progress').hide();
        }
    });
});


$(document).on('click', '.Add-shift-pop-close', function () {
    window.location.href = "/adminattendance/attendance";
    return false;
});


