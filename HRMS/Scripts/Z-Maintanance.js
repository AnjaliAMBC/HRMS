$(document).ready(function () {
        $('.delete-Subscription').on('click', function () {
        // Get selected location
        var location = $('input[name="schedulemaintenance-location"]:checked').val();

        // Get selected employees
        var selectedEmployees = [];
        $('#multiSelectDropdown option:selected').each(function () {
            selectedEmployees.push($(this).val());
        });

        // Get other field values
        var agent = $('#ScheduleMaintenance-Agent').val();
        var date = $('#ScheduleMaintenance-Date').val();
        var timeIn = $('#datetimepicker1').val();
        var timeOut = $('#datetimepicker2').val();

        // Prepare the data to be sent
        var data = {
            Location: location,
            SelectedEmployees: selectedEmployees,
            Agent: agent,
            Date: date,
            TimeIn: timeIn,
            TimeOut: timeOut
        };

        // Send the data using AJAX
        $.ajax({
            type: 'POST',
            url: '/YourControllerName/AddMaintenanceSchedule',  // Change to your actual controller
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(data),
            success: function (response) {
                if (response.success) {
                    // Success: close the modal and show a success message
                    $('#addmaintenanceinfo-popup').modal('hide');
                    alert(response.message);
                } else {
                    // Handle error
                    alert('Error: Could not add schedule');
                }
            },
            error: function (xhr, status, error) {
                // Handle AJAX error
                console.log(error);
                alert('AJAX Error: Could not add schedule');
            }
        });
    });
});
