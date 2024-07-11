$(document).on('click', '.btn-empRaiseTicket', function (event) {
    event.preventDefault();
    $.ajax({
        url: '/empticketing/EmpTicketRaise',
        type: 'POST',
        dataType: 'html',
        beforeSend: function () {
            $('.show-progress').show();
        },
        success: function (response) {
            $(".hiddenempdashboard").html("");
            $(".myrequest-dashboard-data").html("");
            $(".hiddenempdashboard").html(response);
            var formContent = $(".hiddenempdashboard").find(".EmpTicket-raise-container").html();
            $(".myrequest-dashboard-data").html(formContent);
            $('.myrequest-dashboard-data').show();     
            $(".hiddenempdashboard").html("");
         
            $('.show-progress').hide();
        },
        error: function (xhr, status, error) {
            $('.show-progress').hide();
            console.error("Error deleting employee:", error);
        }
    });
});


