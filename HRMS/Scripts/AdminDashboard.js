function HighlightActiveLink(element) {
    $('#admindash-menu li a.active').removeClass('active');
    $(element).addClass('active');
}

//Self service link js
$('.admin-dashboard').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));

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
    HighlightActiveLink($(this));

    $('.admin-empmanagement-container').show();
    $('.admin-dashboard-container').hide();   
    $('.admin-attendance-container').hide();
    $('.admin-leave-container').hide();
    $('.admin-ticketing-container').hide();

    $(".hiddenempmanagement").html("");
});

//Attedence link js
$('.admin-attendance').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));

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
    HighlightActiveLink($(this));

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