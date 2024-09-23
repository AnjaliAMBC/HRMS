
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
    var assetid = $(this).attr("data-assetid");
    window.location.href = '/asset/addasset?assetid=' + assetid;
});
