
$(document).on('click', '.btn-empRaiseTicket', function (event) {
    event.preventDefault();
    // Redirect to the full view for raising a ticket
    window.location.href = '/empticketing/EmpTicketRaise';
});

$(document).on('click', '.emp-ticketing-listing-title', function (event) {
    event.preventDefault();

    window.location.href = '/empticketing/EmpTicketView';
});



