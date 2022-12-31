$(':input[type=number]').on('mousewheel', function (e) {
    e.preventDefault();
});

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
});

// Will eventually make this look lovely for the users
function formilaeAlert(txt, alertTitle, isHex, leftAlign) {
    alertTitle = alertTitle || undefined;
    isHex = isHex || false;
    leftAlign = leftAlign || false;

    alert(txt);
}

// Get the right characters of a string
function Right(str, num) {
    return str.substring(str.length - num);  // pull out right num
}

// Get the left characters of a string
function Left(str, num) {
    return str.substring(0, num);
}

// Suported date formats (This is a list of dates that have validation support below. If another date format comes along, support for it needs to be added)
// dd/mm/yyyy
// mm/dd/yyyy
function isDate(txtDate, dateFormat, separator) {
    var aoDate,           // needed for creating array and object
        ms,               // date in milliseconds
        month, day, year; // (integer) month, day and year
    // if separator is not defined then set '/'
    if (separator === undefined) {
        separator = '/';
    }

    // split input date to month, day and year
    aoDate = txtDate.split(separator);

    // array length should be exactly 3 (no more no less)
    if (aoDate.length !== 3) {
        return false;
    }

    // define month, day and year from array (expected format is m/d/yyyy)
    // subtraction will cast variables to integer implicitly
    if (dateFormat == "mm/dd/yyyy")
    {
        month = aoDate[0] - 1; // because months in JS start from 0
        day = aoDate[1] - 0;
        year = aoDate[2] - 0;
    }
    else if (dateFormat == "dd/mm/yyyy")
    {
        month = aoDate[1] - 1; // because months in JS start from 0
        day = aoDate[0] - 0;
        year = aoDate[2] - 0;
    }

    // test year range
    if (year < 1000 || year > 3000) {
        return false;
    }
    // convert input date to milliseconds
    ms = (new Date(year, month, day)).getTime();
    // initialize Date() object from milliseconds (reuse aoDate variable)
    aoDate = new Date();
    aoDate.setTime(ms);
    // compare input date and parts from Date() object
    // if difference exists then input date is not valid
    if (aoDate.getFullYear() !== year ||
        aoDate.getMonth() !== month ||
        aoDate.getDate() !== day) {
        return false;
    }
    // date is OK, return true
    return true;
}

//This function trims a string
function Trim(s) {
    if (s != "") {
        // Remove leading spaces and carriage returns
        while ((s.substring(0, 1) == ' ') || (s.substring(0, 1) == '\n') || (s.substring(0, 1) == '\r')) {
            s = s.substring(1, s.length);
        }

        // Remove trailing spaces and carriage returns
        while ((s.substring(s.length - 1, s.length) == ' ') || (s.substring(s.length - 1, s.length) == '\n') || (s.substring(s.length - 1, s.length) == '\r')) {
            s = s.substring(0, s.length - 1);
        }
    }
    return s;
}

// Get the value of a querystring in the page url by name
// Example: mysite.com/page.aspx?id=2 
//          getQuerystringValue("id") returns "2"
function getQuerystringValue(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

/* Define function for escaping user input to be treated as 
    a literal string within a regular expression */
function escapeRegExp(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
}

/* Define functin to find and replace specified term with replacement string */
function Replace(str, term, replacement) {
    return str.replace(new RegExp(escapeRegExp(term), 'g'), replacement);
}

function isNumeric(value) {
    if (value == null || !value.toString().match(/^[-]?\d*\.?\d*$/)) return false;
    return true;
}

// IsInteger Function
function NumberIsInteger(data, allowNegative) {
    allowNegative = allowNegative || false;
    var er = /^-?[0-9]+$/;

    if (er.test(data)) {
        num = parseInt(data);
        if (allowNegative == false && num < 0) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return false;
    }
}

function isEvenNumber(num) {
    if (num % 2 == 0) {
        return true;
    }
    else {
        return false;
    }
}

// returns true/false
function InStr(haystack, needle) {
    var result = haystack.indexOf(needle);
    return result;
}

function emailValid(email) {
    const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
}

// Check if the variable passed is NaN / undefined (Returns true/false if undefined)
function isUndefined(obj) {
    return (obj === undefined);
}

function ClearRadioButtonList(fID) {
    // Jquery version... doesn't work with this program because of the brackets in "multi" radio field "name"
    //var str = 'input:radio[name=' + fID + ']';
    //$(str).prop('checked', false);

    var ele = document.getElementsByName(fID);
    for (var i = 0; i < ele.length; i++) {
        ele[i].checked = false;
    }
}

function generateGUID() {
    var d = new Date().getTime();//Timestamp
    var d2 = (performance && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16;//random number between 0 and 16
        if (d > 0) {//Use timestamp until depleted
            r = (d + r) % 16 | 0;
            d = Math.floor(d / 16);
        } else {//Use microseconds since page-load if supported
            r = (d2 + r) % 16 | 0;
            d2 = Math.floor(d2 / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

function getMonthFromMonthNo(monthNo) {
    var result = "";
    switch (monthNo) {
        case 1:
            result = "January";
            break;
        case 2:
            result = "February";
            break;
        case 3:
            result = "March";
            break;
        case 4:
            result = "April";
            break;
        case 5:
            result = "May";
            break;
        case 6:
            result = "June";
            break;
        case 7:
            result = "July";
            break;
        case 8:
            result = "August";
            break;
        case 9:
            result = "September";
            break;
        case 10:
            result = "October";
            break;
        case 11:
            result = "November";
            break;
        case 12:
            result = "December";
            break;
    }

    return result;
}

//============================================================================================================
// The following functions need jquery enabled for use
//============================================================================================================
// Search for a specific value in an array (returns true/false if it exists in the ray)
function ArrayContains(needle, hayStack) {
    return $.inArray(needle, hayStack) !== -1;
}
//============================================================================================================