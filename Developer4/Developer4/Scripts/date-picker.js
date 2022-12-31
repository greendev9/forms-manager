$(".dpGeneral").datepicker({
    showButtonPanel: true,
    closeText: 'Clear',
    onClose: function (dateText, obj) {
        if ($(window.event.srcElement).hasClass('ui-datepicker-close'))
            $(this).val('');
    }
});

// Not in use right now. I am currently setting the date picker within the actual pg.cshtml page. (Later: See if the code CAN be moved here)
//$(".dpBirthday").datepicker({
//    showButtonPanel: true,
//    closeText: 'Clear',
//    changeMonth: true,
//    changeYear: true,
//    yearRange: ((new Date).getFullYear()-100) + ":" + (new Date).getFullYear(),  // --testcode-- (Need to find out how I can pull this from the input field attribute)
//    onClose: function (dateText, obj) {                                          //              OR I can just make a dpMovein, dpOldRelevative, something like that for each date picker type
//        if ($(window.event.srcElement).hasClass('ui-datepicker-close'))          //              THIS ONE IS dpBirthday, so I can probably make separate ones for other situations (will need DB flags though)
//            $(this).val('');
//    },
//    onSelect: function (dateText, obj) {  // THIS PART HAS BEEN TESTED AND WORKS
//        var dFormat = $(this).attr('dFormat').replace("yyyy", "yy");   // Get the custom attribute used from the date input text field here
//        $(this).datepicker('option', {
//            dateFormat: dFormat
//        });
//    }
//});

$(".dpStatsRange").datepicker({
    showButtonPanel: true,
    closeText: 'Clear',
    onClose: function (dateText, obj) {
        if ($(window.event.srcElement).hasClass('ui-datepicker-close'))
            $(this).val('');
    },
    maxDate: new Date()
});

//$(".dpStatsRangeMinDate").datepicker({
//    showButtonPanel: true,
//    closeText: 'Clear',
//    onClose: function (dateText, obj) {
//        if ($(window.event.srcElement).hasClass('ui-datepicker-close'))
//            $(this).val('');
//    },
//    maxDate: new Date(),
//    minDate: new Date(document.getElementById("dpStatsRangeMinDateFld").value)
//});