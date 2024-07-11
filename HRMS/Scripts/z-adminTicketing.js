//Assign Agent 

$(document).on('click', '.btn-It-assignagent', function (event) {
    event.preventDefault();
    $.ajax({
        url: '/adminticketting/adminitassignagent',
        type: 'POST',
        dataType: 'html',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            $(".hiddenadmindashboard").html("");
            $(".admin-ticketing-container").html("");
            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".AdminAssignAgent-View").html();
            $(".admin-ticketing-container").html(formContent);
            $('.admin-ticketing-container').show();
            $(".hiddenadmindashboard").html("");
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error("Error deleting employee:", error);
        }
    });
});