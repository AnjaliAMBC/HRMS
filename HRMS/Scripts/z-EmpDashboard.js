function HighlightEmpActiveLink(element) {
    $('#empdash-menu li').removeClass('sidebaractive');
    $('#empdash-menu li a.active').removeClass('active');
    $(element).addClass('active');
    $(element).parent().addClass('sidebaractive');
}

//Self service link js

$('.emp-selfservice').click(function (event) {
    event.preventDefault();
    //HighlightEmpActiveLink($(this));
    $.ajax({
        url: '/employeedashboard/selfservice',
        type: 'GET',
        dataType: 'html',
        success: function (response) {
            $(".hiddenempdashboard").html("");
            $(".emp-dashboard-data").html("");
            $(".selfservice-dashboard-data").html("");
            $(".attedance-dashboard-data").html("");
            $(".leave-dashboard-data").html("");
            $(".myrequest-dashboard-data").html("");


            $(".hiddenempdashboard").html(response);
            var formContent = $(".hiddenempdashboard").find(".selfservice-container").html();
            $(".selfservice-dashboard-data").html(formContent);

            $('.selfservice-dashboard-data').show();
            $('.emp-dashboard-data').hide();
            $('.attedance-dashboard-data').hide();
            $('.leave-dashboard-data').hide();
            $('.myrequest-dashboard-data').hide();

            $(".hiddenempdashboard").html("");

        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});


////Dashboard link js
//$(document).on('click', '.emp-dashboard', function (event) {
//    event.preventDefault();
//    HighlightEmpActiveLink($(this));
//    $.ajax({
//        url: '/empdash/index',
//        type: 'GET',
//        dataType: 'html',
//        success: function (response) {
//            $(".hiddenempdashboard").html("");
//            $(".emp-dashboard-data").html("");
//            $(".selfservice-dashboard-data").html("");
//            $(".attedance-dashboard-data").html("");
//            $(".leave-dashboard-data").html("");
//            $(".myrequest-dashboard-data").html("");


//            $(".hiddenempdashboard").html(response);
//            var formContent = $(".hiddenempdashboard").find(".employeedash").html();
//            $(".emp-dashboard-data").html(formContent);

//            $('.emp-dashboard-data').show();
//            $('.selfservice-dashboard-data').hide();
//            $('.attedance-dashboard-data').hide();
//            $('.leave-dashboard-data').hide();
//            $('.myrequest-dashboard-data').hide();

//            $(".hiddenempdashboard").html("");
//        },
//        error: function (xhr, status, error) {
//            var err = eval("(" + xhr.responseText + ")");
//        }
//    });
//});
//Attedence link js

$('.emp-attendence').click(function (event) {
    event.preventDefault();
    HighlightEmpActiveLink($(this));
    $.ajax({
        url: '/EmpAttendance/Index',
        type: 'POST',
        dataType: 'html',
        success: function (response) {
            $(".hiddenempdashboard").html("");
            $(".emp-dashboard-data").html("");
            $(".selfservice-dashboard-data").html("");
            $(".attedance-dashboard-data").html("");
            $(".leave-dashboard-data").html("");
            $(".myrequest-dashboard-data").html("");


            $(".hiddenempdashboard").html(response);
            var formContent = $(".hiddenempdashboard").find(".empattendence-container").html();
            $(".attedance-dashboard-data").html(formContent);
            $('.attedance-dashboard-data').show();
            $('.emp-dashboard-data').hide();
            $('.selfservice-dashboard-data').hide();
            $('.leave-dashboard-data').hide();
            $('.myrequest-dashboard-data').hide();

            $(".hiddenempdashboard").html("");
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
    $.ajax({
        url: '/empleave/index',
        type: 'POST',
        dataType: 'html',
        success: function (response) {
    $(".hiddenempdashboard").html("");
    $(".emp-dashboard-data").html("");
    $(".selfservice-dashboard-data").html("");
    $(".attedance-dashboard-data").html("");
    $(".leave-dashboard-data").html("");
    $(".myrequest-dashboard-data").html("");
     $(".hiddenempdashboard").html(response);
    var formContent = $(".hiddenempdashboard").find(".empleave-container").html();
     $(".leave-dashboard-data").html(formContent);
    $('.leave-dashboard-data').show();
    $('.attedance-dashboard-data').hide();
    $('.emp-dashboard-data').hide();
    $('.selfservice-dashboard-data').hide();
    $('.myrequest-dashboard-data').hide();
    $(".hiddenempdashboard").html("");
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});


//My Request link
$('.emp-myrequest').click(function (event) {
    event.preventDefault();
    HighlightEmpActiveLink($(this));

    $(".hiddenempdashboard").html("");
    $(".emp-dashboard-data").html("");
    $(".selfservice-dashboard-data").html("");
    $(".attedance-dashboard-data").html("");
    $(".leave-dashboard-data").html("");
    $(".myrequest-dashboard-data").html("");

    $('.myrequest-dashboard-data').show();
    $('.leave-dashboard-data').hide();
    //$('.attedance-dashboard-data').hide();
    $('.emp-dashboard-data').hide();
    $('.selfservice-dashboard-data').hide();

    $(".hiddenmyrequest").html("");
});


//var isSubmitting = false;

//$(document).off('click', '.emp-imgs').on('click', '.emp-imgs', function (event) {
//    //if (isSubmitting) {
//    //    return;
//    //}
//    //isSubmitting = true;
//    $('#imageInput').click();
//});

$(document).ready(function () {
    // Add click event to the edit icon inside emp-imgs
    $(document).on('click', '.img-edit-icon', function (event) {
        event.stopPropagation();
        $('#imageInput').click();
    });

    // Handle file input change
    $(document).on('change', '#imageInput', function (event) {
        event.preventDefault();
        console.log('Image input changed'); // Debug information

        var file = this.files[0];
        // Check if the file is a JPEG image
        if (file.type !== 'image/jpeg') {
            alert('Please upload a JPEG image.');
            return;
        }

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
                    var timestamp = new Date().getTime();
                    newImageUrl += '?' + timestamp;

                    // Self service page image update
                    $('.emp-profileimage').attr('src', newImageUrl);
                    $('.emp-profileimage').show();
                    $('.default-image').hide();

                    // Navbar image update
                    $('.nav-image-profile').attr('src', newImageUrl);
                    $('.nav-image-profile').show();
                    $('.default-image-navbar').hide();
                } else {
                    console.error('Error uploading file:', response.JsonResponse.Message);
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
});



//function sendAnniversaryWishes() {
//    // Find the active carousel item
//    var activeItem = document.querySelector('.carousel-inner .carousel-item.active');

//    // Extract employee details from the active item
//    var employeeName = activeItem.querySelector('h5').textContent;
//    var employeeEmail = activeItem.querySelector('.email').textContent;
//    var yearsCompleted = activeItem.querySelector('.text-primary').textContent;

//    var currentEmpName = $('.currentempname').text();
//    var subject = "Happy Work Anniversary!";
//    var body = `Dear ${employeeName},\n\nWishing you a wonderful work anniversary. ${yearsCompleted}. Thank you for your hard work and dedication!\n\nBest Regards,\n${currentEmpName}`;

//    fetch('/empdash/sendanniversarywishes', {
//        method: 'POST',
//        headers: {
//            'Content-Type': 'application/json'
//        },
//        body: JSON.stringify({
//            ToEmail: employeeEmail,
//            Subject: subject,
//            Body: body,
//        })
//    })
//        .then(response => {
//            if (response.ok) {
//                alert('Email sent successfully!');
//            } else {
//                throw new Error('Error sending email');
//            }
//        })
//        .catch((error) => {
//            console.error('Error:', error);
//            alert('Error sending email: ' + error.message);
//        });
//}

//function sendWishes() {
//    // Find the selected radio button
//    var selectedEmployee = $('input[name="selectedEmployee"]:checked');
//    if (selectedEmployee.length > 0) {
//        // Get the employee email from the empemail class
//        var employeeEmail = selectedEmployee.closest('.media').find('.empemail').text().trim();

//        // Find the closest parent element with class 'media'
//        var mediaElement = selectedEmployee.closest('.media');
//        if (mediaElement.length > 0) {
//            // Find the element with class 'empname' within the 'media' element
//            var empNameElement = mediaElement.find('.empname');
//            if (empNameElement.length > 0) {
//                // Get the employee name from the 'textContent' property
//                var employeeName = empNameElement.text().trim();
//                var currentEmpName = $('.currentempname').text();

//                var subject = "Happy Birthday!";
//                var body = `Dear ${employeeName},\n\nWishing you a wonderful birthday! May your special day be filled with joy and happiness.\n\nBest Regards,\n${currentEmpName}`;

//                $.ajax({
//                    url: '/empdash/sendbirthdaywishes',
//                    type: 'POST',
//                    contentType: 'application/json',
//                    data: JSON.stringify({
//                        ToEmail: employeeEmail,
//                        Subject: subject,
//                        Body: body
//                    }),
//                    success: function (response) {
//                        alert('Email sent successfully!');
//                    },
//                    error: function (xhr, status, error) {
//                        console.error('Error:', error);
//                        alert('Error sending email: ' + error);
//                    }
//                });
//            } else {
//                console.error('Element with class "empname" not found within the "media" element.');
//                alert('Error: Employee name not found.');
//            }
//        } else {
//            console.error('Parent element with class "media" not found.');
//            alert('Error: Employee details not found.');
//        }
//    } else {
//        alert('Please select an employee to send wishes.');
//    }
//}






