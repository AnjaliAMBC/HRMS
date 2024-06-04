$('.adminside-leaveapply').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));
    $.ajax({
        url: '/adminleave/adminleaveapply',
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
            var formContent = $(".hiddenadmindashboard").find(".admin-leave-apply").html();
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
//avialable balance function

var availableBalances = {
    1: 10, // Earned Leave
    2: 5,  // Emergency Leave
    3: 8,  // Sick Leave
    4: 3,  // Bereavement Leave
    5: 12  // Hourly Permission
};

// Function to update available balance based on selected leave type
function updateAvailableBalance(leaveType) {
    var availableBalance = availableBalances[leaveType];
    // Update the UI with the available balance
    $('.available-balance').text(availableBalance);
}

// Attach change event listener to the leave type dropdown

$(document).on('change', '#adminleaveType', function (event) {
    var selectedLeaveType = $(this).val();
    generateBalanceSection(selectedLeaveType);
});

// Function to dynamically generate balance section HTML
function generateBalanceSection() {
    var leaveType = $('#adminleaveType').val();
    var availableBalance = availableBalances[leaveType];

    var balanceSectionHTML = `
            <div class="ml-4" style="margin-top:25px;">
                <div class="balance-section mt-4 ml-4">
                    <div class="row" style="line-height:2;">
                        <div class="col-md-4">
                            <span>Available Balance</span>
                        </div>
                        <div class="col-md-6">
                            <div class="available-balance">${availableBalance}</div>
                        </div>
                        <div class="col-md-4">
                            <span>Currently Booked</span>
                        </div>
                        <div class="col-md-6">
                            <div>3</div>
                        </div>
                        <div class="col-md-4">
                            <span>Balance</span>
                        </div>
                        <div class="col-md-6">
                            <div>3</div>
                        </div>
                    </div>
                </div>
            </div>`;

    // Replace existing balance section with updated HTML
    $('.balance-section-wrapper').html(balanceSectionHTML);
}

// Initially generate balance section
generateBalanceSection();
