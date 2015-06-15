function ButtonClick(button, action) {
    var form = $(button).closest("form");
    if (form.length == 0)
        return;
    var imageAction = $("#acsaction", form);
    if (imageAction.length == 0) {
        $('<input id="acsaction" type="hidden" name="Action" value="' + action + '" />').appendTo(form);
    } else {
        imageAction.val(action);
    }
    form.submit();
    return false;
}