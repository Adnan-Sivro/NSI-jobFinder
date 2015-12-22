$(document).ready(function () {
    $('.applyForJobBtn').click(function (button) {
        var email = button.toElement.value;
        $.post("SaveContactEmailToViewData", { contactEmail: email });
       // $("#applyForJobModal").show();
    })
});