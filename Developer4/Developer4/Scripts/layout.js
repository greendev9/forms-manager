    // prevent enter key to submit a form
    // To allow enter key press on a form, add allowEnterKeypress="true" and the form tag must have an id attribute set
    $(window).keydown(function (event) {
        var formId = $(document.activeElement).closest('form').attr('id');
        var canEnterKeyPress = "false";
        if (formId !== undefined) {
            canEnterKeyPress = $().attr("allowEnterKeypress");
        }

        if (event.keyCode == "13" && canEnterKeyPress == "false") {
            event.preventDefault();
            return false;
        }
});