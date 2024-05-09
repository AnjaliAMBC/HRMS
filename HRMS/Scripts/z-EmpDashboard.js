function HighlightEmpActiveLink(element) {
    $('#empdash-menu li').removeClass('sidebaractive');
    $('#empdash-menu li a.active').removeClass('active');
    $(element).addClass('active');
    $(element).parent().addClass('sidebaractive');
}

//Self service link js
$('.emp-selfservice').click(function (event) {
    event.preventDefault();
    HighlightEmpActiveLink($(this));
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
    HighlightEmpActiveLink($(this));

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
    HighlightEmpActiveLink($(this));
    $.ajax({
        url: '/EmpAttendance/Index',
        type: 'POST',
        dataType: 'html',
        success: function (response) {
            $(".hiddenattendance").html(response);
            var formContent = $(".hiddenattendance").find(".empattendence-container").html();
            $(".attedance-dashboard-data").html(formContent);
            $('.attedance-dashboard-data').show();
            $('.emp-dashboard-data').hide();
            $('.selfservice-dashboard-data').hide();
            $('.leave-dashboard-data').hide();
            $('.myrequest-dashboard-data').hide();

            $(".hiddenattendance").html("");
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});
//Leave Tracker
$('.emp-leavetracker').click(function (event) {
    event.preventDefault();
    HighlightEmpActiveLink($(this));

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
    HighlightEmpActiveLink($(this));

    $('.myrequest-dashboard-data').show();
    $('.leave-dashboard-data').hide();
    //$('.attedance-dashboard-data').hide();
    $('.emp-dashboard-data').hide();
    $('.selfservice-dashboard-data').hide();

    $(".hiddenmyrequest").html("");
});


var isSubmitting = false;

$(document).off('click', '.emp-imgs').on('click', '.emp-imgs', function (event) {
    if (isSubmitting) {
        return;
    }
    isSubmitting = true;
    $('#imageInput').click();
});


$(document).on('change', '#imageInput', function (event) {
    event.preventDefault();
    var file = this.files[0];
    var formData = new FormData();
    formData.append('file', file);

    $.ajax({
        url: '/employeedashboard/uploadimage',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        dataType: 'json',
        success: function (response) {
            if (response.JsonResponse.StatusCode == 200) {
                var newImageUrl = response.ImageURl;
                //Slef service page image update
                $('.emp-imageurl').attr('src', newImageUrl);
                $('.emp-imageurl').show();
                $('.default-image').hide();


                $('.nav-image-profile').show();
                $('.nav-image-profile').attr('src', newImageUrl);
                $('.default-image-navbar').hide();
            } else {
                console.error('Error uploading file:', error);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error uploading file:', error);
        },
        complete: function () {
            isSubmitting = false;
        }
    });
});





