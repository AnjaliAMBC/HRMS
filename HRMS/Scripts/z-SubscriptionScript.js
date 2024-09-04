$(document).ready(function () {
    $('#addsubscriptioninfo').change(function () {
        if ($(this).val() === 'Addmanually') {
            window.location.href = '/Subscription/AddSubscription';
        }
    });
});