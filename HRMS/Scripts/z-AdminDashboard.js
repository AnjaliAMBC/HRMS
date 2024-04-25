function HighlightAdminActiveLink(element) {
    $('#admindash-menu li a.active').removeClass('active');
    $(element).addClass('active');
}

//Self service link js
$('.admin-dashboard').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $('.admin-dashboard-container').show();
    $('.admin-empmanagement-container').hide();
    $('.admin-attendance-container').hide();
    $('.admin-leave-container').hide();
    $('.admin-ticketing-container').hide();

    $(".hiddenadmindashboard").html("");

    
});


//Dashboard link js
$('.admin-empmanagement').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $.ajax({
        url: '/admindashboard/employeemanagement',
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            $(".hiddenempmanagement").html(response);
            var formContent = $(".hiddenempmanagement").find(".admin-empmanagement-view").html();
            $(".admin-empmanagement-container").html(formContent);

            $('.admin-empmanagement-container').show();
            $('.admin-dashboard-container').hide();
            $('.admin-attendance-container').hide();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenempmanagement").html("");
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});

//Attedence link js
$('.admin-attendance').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $('.admin-attendance-container').show();
    $('.admin-empmanagement-container').hide();
    $('.admin-dashboard-container').hide();
    $('.admin-leave-container').hide();
    $('.admin-ticketing-container').hide();

    $(".hiddenadminattendance").html("");
});

//Leave Tracker
$('.admin-leave').click(function (event) {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $('.admin-leave-container').show();
    $('.admin-attendance-container').hide();
    $('.admin-empmanagement-container').hide();
    $('.admin-dashboard-container').hide();
    $('.admin-ticketing-container').hide();

    $(".hiddenadminleave").html("");
});


//My Request link
$('.admin-ticketing').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));

    $('.admin-ticketing-container').show();
    $('.admin-leave-container').hide();
    $('.admin-attendance-container').hide();
    $('.admin-empmanagement-container').hide();
    $('.admin-dashboard-container').hide();  

    $(".hiddenadminticketing").html("");
});