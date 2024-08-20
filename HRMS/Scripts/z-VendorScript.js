
$(document).on('change', '#addvendor', function () {   
        var selectedValue = $(this).val();
        if (selectedValue === "Addmanually") {
            window.location.href = '/Vendor/AddVendor';
        } else if (selectedValue === "importuser") {
            window.location.href = '/Vendor/ImportVendor';
        }
    });

