

function formatAttedenceTime(timeStr) {
    var [time, modifier] = timeStr.split(' ');
    var [hours, minutes] = time.split(':');

    // Convert hours to string if necessary
    if (hours === '12') {
        hours = '00';
    }

    if (modifier === 'PM' && hours !== '12') {
        hours = (parseInt(hours, 10) + 12).toString();
    } else if (modifier === 'AM' && hours === '12') {
        hours = '00';
    }

    return `${hours.padStart(2, '0')}:${minutes}`;
}

$(document).on('click', '.empattendence-edit', function () {
    var row = $(this).closest('tr');
    var empId = row.find('.employee-id').text();
    var employeeName = row.find('.employee-details').text().trim().split('\n')[0];
    var date = row.find('td:nth-child(3)').text().trim();
    var checkIn = row.find('td:nth-child(4)').text().trim();
    var checkOut = row.find('td:nth-child(5)').text().trim();
    var status = row.find('td:nth-child(7) button').text().trim();
    var loginID = $(this).attr('data-attendenceid');

    // Manually parse the date string
    var parts = date.split(' ');
    var day = parts[0];
    var month = parts[1];
    var year = parts[2];

    // Month mapping
    var monthIndex = {
        'January': '01',
        'February': '02',
        'March': '03',
        'April': '04',
        'May': '05',
        'June': '06',
        'July': '07',
        'August': '08',
        'September': '09',
        'October': '10',
        'November': '11',
        'December': '12'
    };

    // Format the date to 'yyyy-mm-dd'
    var isoDate = year + '-' + monthIndex[month] + '-' + (day.length === 1 ? '0' + day : day);
    //
    $('#leaveUpdateModal').find('.leaveupdate_empid').val(empId);
    $('#leaveUpdateModal').find('.leaveupdate_name').val(employeeName);
    $('#leaveUpdateModal').find('.leaveupdate_date').val(isoDate);
    $('#leaveUpdateModal').find('.leaveupdate_checkin').val(formatAttedenceTime(checkIn));
    $('#leaveUpdateModal').find('.leaveupdate_checkout').val(formatAttedenceTime(checkOut));
    $('#leaveUpdateModal').find('#eleaveupdate_status').val(status);
    $('#leaveUpdateModal').find('.leaveupdate_btn').attr("data-updateid", loginID);
    $('#leaveUpdateModal').modal('show');
});




$(document).on('click', '.leaveupdate_btn', function () {
    var empId = $('.leaveupdate_empid').val();
    var empName = $('.leaveupdate_name').val();
    var date = $('.leaveupdate_date').val();
    var checkIn = $('.leaveupdate_checkin').val();
    var checkOut = $('.leaveupdate_checkout').val();
    var status = $('#eleaveupdate_status').val();
    var loginID = $(this).attr("data-updateid");

    $.ajax({
        url: '/adminattendance/updateemployeecheckin',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            EmpId: empId,
            EmpName: empName,
            Date: date,
            CheckIn: checkIn,
            CheckOut: checkOut,
            Status: status,
            loginID: loginID
        }),
        success: function (response) {
            if (response.success) {
                console.log('Employee data updated successfully!');
                $('#leaveUpdateModal').modal('hide');

                var activeDiv = $('.days-container .day.active');             
                if (activeDiv.length) {
                    var dateValue = activeDiv.attr('data-date');
                    console.log(dateValue);
                    highlightDate(activeDiv);                  
                } 
              
            } else {
                alert('Error: ' + response.message);
                $('#leaveUpdateModal').modal('hide');
            }
        },
        error: function (xhr, status, error) {
            alert('Error: ' + error);
            $('#leaveUpdateModal').modal('hide');
        }
    });
});

$(document).on('click', '#leaveUpdateModal', function () {
    // Your code for #leaveUpdateModal click event
    $('body').removeClass('modal-open');
    $('.modal-backdrop').remove();
});

