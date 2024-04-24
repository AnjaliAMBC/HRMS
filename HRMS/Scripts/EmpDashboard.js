function HighlightActiveLink(element) {
    $('#empdash-menu li a.active').removeClass('active');
    $(element).addClass('active');
}

//Self service link js
$('.emp-selfservice').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));
    $.ajax({
        url: '/employeedashboard/selfservice',
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            $(".hiddenselfservice").html(response);
            var formContent = $(".hiddenselfservice").find(".selfservice-container").html();
            $(".selfservice-dashboard-data").html(formContent);

            $('.selfservice-dashboard-data').show();
            $('.emp-dashboard-data').hide();
            $('.attedance-dashboard-data').hide();
            $('.leave-dashboard-data').hide();
            $('.myrequest-dashboard-data').hide();
            
            $(".hiddenselfservice").html("");

        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});


//Dashboard link js
$('.emp-dashboard').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));
  
    $('.emp-dashboard-data').show();
    $('.selfservice-dashboard-data').hide();
    $('.attedance-dashboard-data').hide();
    $('.leave-dashboard-data').hide();
    $('.myrequest-dashboard-data').hide();

    $(".hiddendashboard").html("");
});

//Attedence link js
$('.emp-attendence').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));

    $('.attedance-dashboard-data').show();
    $('.emp-dashboard-data').hide();
    $('.selfservice-dashboard-data').hide(); 
    $('.leave-dashboard-data').hide();
    $('.myrequest-dashboard-data').hide();

    $(".hiddenattendance").html("");
});

//Leave Tracker
$('.emp-leavetracker').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));

    $('.leave-dashboard-data').show();
    $('.attedance-dashboard-data').hide();
    $('.emp-dashboard-data').hide();
    $('.selfservice-dashboard-data').hide();   
    $('.myrequest-dashboard-data').hide();

    $(".hiddenleave").html("");
});


//My Request link
$('.emp-myrequest').click(function (event) {
    event.preventDefault();
    HighlightActiveLink($(this));

    $('.myrequest-dashboard-data').show();
    $('.leave-dashboard-data').hide();
    $('.attedance-dashboard-data').hide();
    $('.emp-dashboard-data').hide();
    $('.selfservice-dashboard-data').hide();   

    $(".hiddenmyrequest").html("");
});