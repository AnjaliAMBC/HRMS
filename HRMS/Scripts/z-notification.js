$(document).on("click", ".emp-dash-modal-send", function () {
    var button = $(this);
    var modal = button.closest(".emp-dash-modal-blocks");
    var feedback = modal.find(".message-feedback");
    var commentsField = modal.find(".emp-dash-modal-message");


    var notificationData = {
        NotificationFromName: $(".loggedinempname").text().trim(),
        NotificationFromID: $(".loggedinempid").text().trim(),
        NotificationToName: button.data("notificationtoname"),
        NotificationToID: button.data("notificationtoid"),
        NotificationType: button.data("notificationtype"),
        Comments: commentsField.val().trim(),
        Status: "Sent"
    };


    if (!notificationData.Comments) {
        feedback.text("Comments are mandatory.")
            .css({ "color": "red", "display": "block" });
        commentsField.css("border-color", "red");
        return;
    }


    feedback.hide();
    commentsField.css("border-color", "");


    $.ajax({
        url: '/notification/sendnotification',
        type: "POST",
        data: JSON.stringify(notificationData),
        contentType: "application/json",
        success: function (response) {
            if (response.success) {
                feedback.text(response.message)
                    .css({ "color": "green", "display": "block" });
                commentsField.val("");
                setTimeout(function () {
                    feedback.fadeOut();
                }, 2000);
            } else {
                feedback.text(response.message)
                    .css({ "color": "red", "display": "block" });
            }
        },
        error: function () {
            feedback.text("An error occurred while sending the message.")
                .css({ "color": "red", "display": "block" });
        }
    });
});

$(document).on("click", ".notification-reply", function () {
    //$('.modal-backdrop:gt(0)').remove();
    $('.modal-backdrop').not(':first').remove();
    var sno = $(this).data('sno');
    $('#NotificationReplyPopup').attr('data-sno', sno);
    $('#NotificationReplyPopup #sender-message').val("");

    // Check if the modal is already open
    if ($('#NotificationReplyPopup').hasClass('show')) {
        // Hide the modal, then load data and reopen once hidden
        $('#NotificationReplyPopup').modal("hide").on('hidden.bs.modal', function () {
            loadNotificationReplyData(sno);  // Load data after hiding
        });
    } else {
        loadNotificationReplyData(sno); // Load data directly if modal is closed
    }
});

$(document).on("click", "#NotificationPopup .modal-header button", function () {
    $('#NotificationReplyPopup').hide();
    $('.modal-backdrop').remove();
});

function loadNotificationReplyData(sno) {
    $.ajax({
        url: '/notification/getnotificationdetails',
        type: 'GET',
        data: { sno: sno },
        success: function (data) {
            // Update the content
            $('.empNotificationReply-div').html(data);

            // Reopen the modal with updated content
            $('#NotificationReplyPopup').modal("show");
        },
        error: function (xhr, status, error) {
            console.error(error);
            alert('An error occurred while retrieving the notification details.');
        }
    });
}



$(document).on("click", ".sender-send-btn", function () {
    var sno = $('#NotificationReplyPopup').attr('data-sno'); // Get sno from the modal's data
    var replyComments = $('#NotificationReplyPopup #sender-message').val(); // Get the value from the textarea
    var replyFrom = $(this).closest('.notificationreply-sender').find('.sender-name').text(); // Get reply from name
    var replyTo = $(this).closest('.notificationreply-receiver').find('.receiver-name').text(); // Get reply to name

    if (!replyComments) {
        alert("Reply message cannot be empty.");
        return;
    }

    var formData = new FormData();
    formData.append("SNo", sno);
    formData.append("ReplyComments", replyComments);
    formData.append("ReplyFrom", replyFrom);
    formData.append("ReplyTo", replyTo);

    $.ajax({
        url: '/empdash/savenotificationreply',
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                // Show success message and fade it out
                $('#NotificationReplyPopup .replysent-msg').text("Reply sent successfully..!!").show();
                setTimeout(function () {
                    $('#NotificationReplyPopup .replysent-msg').fadeOut();
                }, 3000);

                // Optionally clear the reply message textarea
                $('#NotificationReplyPopup #sender-message').val('');
            } else {
                alert(response.message || "Failed to save reply.");
            }
        },
        error: function () {
            alert("An error occurred while saving the reply.");
        }
    });
});


