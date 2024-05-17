
$(document).on('click', '.btn-addshift', function () {
    event.preventDefault();
    HighlightAdminActiveLink($(this));

    $.ajax({
        url: '/adminattendance/addshift',
        type: 'GET',
        dataType: 'html',
        success: function (response) {

            $(".hiddenadmindashboard").html("");
            $(".admin-attendance-container").html("");

            $(".hiddenadmindashboard").html(response);
            var formContent = $(".hiddenadmindashboard").find(".admin-addshift-view").html();
            $(".admin-attendance-container").html(formContent);

            $('.admin-attendance-container').show();
            $('.admin-empmanagement-container').hide();
            $('.admin-emppadd-container').hide();
            $('.admin-dashboard-container').hide();
            $('.admin-leave-container').hide();
            $('.admin-ticketing-container').hide();
            $(".hiddenadmindashboard").html("");
        },
        error: function (xhr, status, error) {
            var err = eval("(" + xhr.responseText + ")");
        }
    });
});




//document.getElementById("departmentOption").onchange = function () {
//    document.getElementById("departmentDropdown").style.display = "block";
//    document.getElementById("employeeDropdown").style.display = "none";
//};


//document.getElementById("employeeOption").onchange = function () {
//    document.getElementById("departmentDropdown").style.display = "none";
//    document.getElementById("employeeDropdown").style.display = "block";
//};


//document.getElementById("saveButton").onclick = function () {
//};
