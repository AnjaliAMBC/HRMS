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
