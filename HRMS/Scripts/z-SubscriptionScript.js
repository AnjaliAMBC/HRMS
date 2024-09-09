$(document).ready(function () {
    $('#addsubscriptioninfo').change(function () {
        if ($(this).val() === 'Addmanually') {
            window.location.href = '/Subscription/AddSubscription';
        }
    });
});

function redirectToSubscriptionHistory() {
    window.location.href = '/subscription/history';
}