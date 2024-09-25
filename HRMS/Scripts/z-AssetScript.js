
$(document).on('change', '#addassetinfo', function () {
    var selectedValue = $(this).val();
    if (selectedValue === "Addmanually") {
        window.location.href = '/Asset/AddAsset';
    } else if (selectedValue === "BulkAsset") {
        window.location.href = '/Asset/AssetBulkImport';
    }
});


$(document).on('click', '.Asset-id-select', function () {
    window.location.href = '/Asset/AssetView';
});

$(document).on('click', '.asset-info-edit', function () {
    var sno = $(this).attr("data-assetid");
    window.location.href = '/asset/addasset?sno=' + sno;
});


$(document).on('click', '.asset-info-transfer', function () {
    var sno = $(this).attr("data-assetid");
    $('#assettransfer-popup').modal('show');
    $.ajax({
        url: 'asset/assettransfer',
        type: 'GET',
        data: { sno: sno },
        success: function (data) {
            $('#AssetpopupContent').html(data);
            $('#Assettransferpopup').show();
        },
        error: function (xhr, status, error) {
            alert("An error occurred: " + error);
        }
    });
});


$(document).on('click', '#AssetclosePopup').click(function () {
    $('#Assettransferpopup').hide();
});

