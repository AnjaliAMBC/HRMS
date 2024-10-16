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
