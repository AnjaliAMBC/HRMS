
    $('.fa-pencil-alt').on('click', function () {
        var row = $(this).closest('tr');
        var empId = row.find('.empname').data('empid');
        var empName = row.find('.empname').text().trim();
        var date = row.find('.date').text().trim();
        var checkIn = row.find('.checkin').text().trim();
        var checkOut = row.find('.checkout').text().trim();
        var status = row.find('.status').text().trim();

        // Fill the popup with data
        $('.leaveupdate_name').val(empName).data('empid', empId);
        $('.leaveupdate_date').val(date);
        $('.leaveupdate_checkin').val(checkIn);
        $('.leaveupdate_checkout').val(checkOut);
        $('#eleaveupdate_status').val(status);

        // Show the popup
        $('.leaveupdate_body').show();
    });

    $('.leaveupdate_btn').on('click', function () {
        var empId = $('.leaveupdate_name').data('empid');
        var empName = $('.leaveupdate_name').val();
        var date = $('.leaveupdate_date').val();
        var checkIn = $('.leaveupdate_checkin').val();
        var checkOut = $('.leaveupdate_checkout').val();
        var status = $('#eleaveupdate_status').val();

        $.ajax({
            url: '@Url.Action("UpdateEmployeeCheckIn", "AdminAttendance")',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                EmpId: empId,
                EmpName: empName,
                Date: date,
                CheckIn: checkIn,
                CheckOut: checkOut,
                Status: status
            }),
            success: function (response) {
                if (response.success) {
                    alert('Employee data updated successfully!');
                    // Optionally refresh the page or update the table row with new data
                    location.reload();
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
    });

    // Hide the popup when clicking outside of it
    $(document).mouseup(function (e) {
        var container = $(".leaveupdate_view");
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            $('.leaveupdate_body').hide();
        }
    });
