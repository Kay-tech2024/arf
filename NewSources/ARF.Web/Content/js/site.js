if (typeof (lang) === 'undefined')
    lang = 'ar';

window.escapeRegExp = function (str) {
    return str.replace(/[.*+?^${}()|[\]\\]/g, "\\$&"); // $& means the whole matched string
};

String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(escapeRegExp(search), 'g'), replacement);
};

(function () {
    if (typeof window.Element === "undefined" || "classList" in document.documentElement) return;

    var prototype = Array.prototype,
        push = prototype.push,
        splice = prototype.splice,
        join = prototype.join;

    function DOMTokenList(el) {
        this.el = el;
        // The className needs to be trimmed and split on whitespace
        // to retrieve a list of classes.
        var classes = el.className.replace(/^\s+|\s+$/g, '').split(/\s+/);
        for (var i = 0; i < classes.length; i++) {
            push.call(this, classes[i]);
        }
    };

    DOMTokenList.prototype = {
        add: function (token) {
            if (this.contains(token)) return;
            push.call(this, token);
            this.el.className = this.toString();
        },
        contains: function (token) {
            return this.el.className.indexOf(token) !== -1;
        },
        item: function (index) {
            return this[index] || null;
        },
        remove: function (token) {
            if (!this.contains(token)) return;
            for (var i = 0; i < this.length; i++) {
                if (this[i] == token) break;
            }
            splice.call(this, i, 1);
            this.el.className = this.toString();
        },
        toString: function () {
            return join.call(this, ' ');
        },
        toggle: function (token) {
            if (!this.contains(token)) {
                this.add(token);
            } else {
                this.remove(token);
            }

            return this.contains(token);
        }
    };

    window.DOMTokenList = DOMTokenList;

    function defineElementGetter(obj, prop, getter) {
        if (Object.defineProperty) {
            Object.defineProperty(obj, prop, {
                get: getter
            });
        } else {
            obj.__defineGetter__(prop, getter);
        }
    }

    defineElementGetter(Element.prototype, 'classList', function () {
        return new DOMTokenList(this);
    });
})();

window.DateDiff = {
    inDays: function (d1, d2) {
        var t2 = d2.getTime();
        var t1 = d1.getTime();

        var diff = parseInt((t2 - t1) / (24 * 3600 * 1000));
        return diff;
    },

    inWeeks: function (d1, d2) {
        var t2 = d2.getTime();
        var t1 = d1.getTime();

        return parseInt((t2 - t1) / (24 * 3600 * 1000 * 7));
    },

    inMonths: function (d1, d2) {
        var d1Y = d1.getFullYear();
        var d2Y = d2.getFullYear();
        var d1M = d1.getMonth();
        var d2M = d2.getMonth();

        return (d2M + 12 * d2Y) - (d1M + 12 * d1Y);
    },

    inYears: function (d1, d2) {
        return d2.getFullYear() - d1.getFullYear();
    }
};

String.prototype.toProperCase = function () {
    return this.replace(/\w\S*/g, function (txt) { return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase(); });
};

String.prototype.isArabic = function () {
    var str = this.toString();
    if (str.length == 0 || str.trim().length === 0) return null;

    var rx_arabic = /[\u0600-\u06FF]/g;
    var rx_letters = /[\w]/g;

    var count1 = !rx_arabic.test(str) ? 0 : str.match(rx_arabic).length;
    var count2 = !rx_letters.test(str) ? 0 : str.match(rx_letters).length;

    if (count2 === 0) {
        if (count1 > 0)
            return true;
        return null;
    }

    return count1 >= count2;
};

if ($('.nav-tabs a').length > 0) {
    var url = document.location.toString();
    if (url.match('#') && url.split('#').length > 1) {
        var tab = $('.nav-tabs a[data-target=\'#' + url.split('#')[1] + '\']');
        if (tab.length === 0) {
            tab = $('.nav-tabs a[href=\'#' + url.split('#')[1] + '\']');

            if (tab.length === 0) {
                $('.nav-tabs a').first().tab('show');
            } else {
                $('.nav-tabs a[href=\'#' + url.split('#')[1] + '\']').tab('show');
            }
        } else {
            $('.nav-tabs a[data-target=\'#' + url.split('#')[1] + '\']').tab('show');
        }
    }
}

// Change hash for page-reload
$('.nav-tabs a').on('shown.bs.tab', function (e) {
    var targetHash = $(e.target).data('target');
    if (!targetHash)
        targetHash = $(e.target).attr('href');

    window.location.hash = targetHash;
});

var isFunction = function (functionToCheck) {
    var getType = {};
    return functionToCheck && getType.toString.call(functionToCheck) === '[object Function]';
};

var isObject = function (val) {
    if (val === null) { return false; }
    return ((typeof val === 'function') || (typeof val === 'object'));
};

var onDomInserted = function (cls, func) {
    var selector = '';
    if (isObject(cls) && typeof cls.selector === 'string' && cls.selector != null && cls.selector.trim().length > 0) {
        selector = cls.selector.trim();
    } else {
        if (typeof cls !== 'string' || cls.trim() === '' || !isFunction(func)) return;

        selector = cls.trim();

        if (selector.length === 0) return;

        if (selector.indexOf('.') < 0)
            selector = '.' + selector;
    }

    var els = $(selector);
    $.each(els, function (i) {
        func($(els[i]));
    });

    $(document).on('DOMNodeInserted', function (e) {
        if ($(e.target).is(selector)) {
            func($(e.target));
        } else {
            var elements = $(e.target).find(selector);
            if (elements.length > 0) {
                $.each(elements, function (i) {
                    func($(elements[i]));
                });
            }
        }
    });
};

var updateFieldValidation = function () {
    if ($(this).data('val') !== true) return;
    var parent = $(this).closest('.input-group');
    if (!parent || parent.length === 0)
        parent = $(this).closest('.form-group');

    var form = $(this).closest('form');

    if (form && isFunction($(this).valid)) {
        if (!$(this).valid()) {
            if (parent) {
                parent.addClass('has-error');
                parent.removeClass('has-success');
            }
        } else {
            if (parent) {
                if ($(this).val() != '')
                    parent.addClass('has-success');
                else
                    parent.removeClass('has-success');
                parent.removeClass('has-error');
            }
        }
    }
};

var updateFormValidation = function () {
    var form = $(this).closest('form');
    if (!form)
        return;

    var fields = form.find('input,select,textarea');
    for (var i = 0; i < fields.length; i++) {
        updateFieldValidation.call(fields[i]);
    }
};

$('input, select, textarea').on('input', updateFieldValidation);
$('input, select, textarea').on('blur', updateFieldValidation);
$('input[type=submit],button[type=submit]').on('click', updateFormValidation);

onDomInserted('.date-time-picker', function (e) {
    $(function () {
        if (isFunction($(e).datetimepicker)) {
            $(e).datetimepicker({
                format: 'YYYY-MM-DD hh:mm A'
            });
        }
    });
});

onDomInserted('.date-picker', function (e) {
    $(function () {
        if (isFunction($(e).datetimepicker)) {
            $(e).datetimepicker({
                format: 'YYYY-MM-DD'
            });
        } else if (isFunction($(e).datepicker)) {
            $(e).datepicker({
                format: 'yyyy-mm-dd'
            });
        }
    });
});

Date.prototype.getDateString2 = function () {
    var mm = (this.getMonth() + 1) + ''; // getMonth() is zero-based
    var dd = (this.getDate()) + '';

    return [this.getFullYear(), !mm[1] ? '0' + mm : mm, !dd[1] ? '0' + dd : dd].join('-'); // padding
};

if (typeof (HijriDate) !== 'undefined') {
    HijriDate.prototype.getDateString2 = function () {
        var mm = (this.getMonth() + 1) + ''; // getMonth() is zero-based
        var dd = (this.getDate()) + '';
        return [this.getFullYear(), !mm[1] ? '0' + mm : mm, !dd[1] ? '0' + dd : dd].join('-'); // padding
    };
}

onDomInserted('.custom-date-picker', function (e) {
    $(function () {
        e = $(e);
        var input = e.find('input');
        var isHijri = e.attr('data-hirji') === '1';
        var button = e.find('.custom-date-picker-button');
        var switchButton = e.find('.custom-date-picker-switch');
        var linkWithElm = e.attr('data-custom-date-picker-link') ? $($(e).attr('data-custom-date-picker-link')) : null;
        var isShown = false;

        if (input) {
            var cal = isHijri ? new Calendar(true, 0, false, true) : new Calendar();
            var calMode = cal.isHijriMode();
            var isEmpty = true;

            if (!isFunction(input.val)) return;
            var val = input.val();
            if (val == null) return;

            if (val.trim() !== '') {
                var date = new Date();
                try {
                    date = cal.getDate(input.val().trim());
                    isEmpty = false;
                } catch (ex) {
                }
                cal.setTime(date.getTime());
            }

            e[0].cal = cal;

            if (linkWithElm != null && linkWithElm.length === 1) {
                e[0].otherCalendarEl = linkWithElm[0];
                linkWithElm[0].otherCalendarEl = e[0];

                if (input.val().trim() === '' && e[0].otherCalendarEl.cal !== undefined) {
                    cal.setTime(e[0].otherCalendarEl.cal.getTime());
                    isEmpty = false;
                } else if (linkWithElm.find('input').val().trim() === '' && e[0].otherCalendarEl.cal !== undefined) {
                    e[0].otherCalendarEl.cal.setTime(cal.getTime());
                }
            }

            if (switchButton && switchButton.length === 1) {
                switchButton.mousedown(function () {
                    $(e[0].otherCalendarEl).show();
                    e.hide();
                });
            }

            e[0].appendChild(cal.getElement());

            if (!isEmpty) {
                if (input.val() === '')
                    input.val(cal.getDate().getDateString2());
            }

            if (window.calendarElms === undefined) {
                window.calendarElms = [];
            }

            window.calendarElms.push(e[0]);

            cal.callback = function () {
                var cal2 = e[0].otherCalendarEl !== undefined ? e[0].otherCalendarEl.cal : null;

                if (cal2 !== null) {
                    cal2.disableCallback(true);
                    cal2.setTime(cal.getTime());
                    $(e[0].otherCalendarEl).find('input').val(cal2.getDate().getDateString2());
                    cal2.hide();
                    cal2.disableCallback(false);
                }

                input.val(cal.getDate().getDateString2());

                input.click();
                input.focus();
                input.click();
                cal.hide();
            };

            var toggleCalendar = function (show) {
                if (show || !isShown) {
                    if (window.calendarElms && window.calendarElms.length > 0) {
                        for (var i = 0; i < window.calendarElms.length; i++) {
                            var el = window.calendarElms[i];
                            el.hideCalendar();
                        }
                    }

                    $('.custom-date-picker .calendar').css('display', 'none');

                    cal.show();
                    isShown = true;
                    for (var i = 0; i < window.calendarElms.length; i++) {
                        var el = window.calendarElms[i];
                        if (el === e[0]) continue;

                        el.cal.hide();
                    }
                } else {
                    cal.hide();
                    isShown = false;
                }
            };

            e[0].hideCalendar = function () {
                cal.hide();
                isShown = false;
            };

            input.mousedown(function (ev) {
                toggleCalendar(true);
                ev.stopPropagation();
            });

            input.focus(function (ev) {
                toggleCalendar(true);
                ev.stopPropagation();
            });

            if (button) {
                button.mousedown(function (ev) {
                    toggleCalendar();
                    ev.stopPropagation();
                });
            }
        }
    });
});

onDomInserted('.custom-date-picker .calendar', function (e) {
    $(e).mousedown(function (ev) {
        ev.stopPropagation();
    });

    $(e).find('.header-button').mousedown(function (ev) {
        e.closest('.custom-date-picker')[0].headerClicked = new Date();
        ev.stopPropagation();
    });
});

$(document).mousedown(function () {
    if (window.calendarElms && window.calendarElms.length > 0) {
        for (var i = 0; i < window.calendarElms.length; i++) {
            var el = window.calendarElms[i];
            el.hideCalendar();
        }
    }

    $('.custom-date-picker .calendar').css('display', 'none');
});

var parseDate = function (value) {
    if (!value || value.length === 0) return null;
    return moment(value.trim(), 'YYYY-MM-DD');
};

var parseDatetime = function (value) {
    if (!value || value.length === 0) return null;
    return moment(value.trim(), 'YYYY-MM-DD hh:mm A');
};

var expandTextarea = function ($element) {
    if (!$element || $element.length === 0) return;

    var expand = function () {
        var sheight = $element[0].scrollHeight;
        var height = $element.height();
        if (height > sheight) return;

        if (sheight < 100)
            sheight = 100;

        $element.css('overflow', 'hidden');
        $element.css('height', 0);
        $element.css('height', sheight + 'px');
    };

    $element.on('input', function () {
        expand();
    });

    expand();
};

var addRtlToInput = function ($element) {
    if (!$element || $element.length === 0) return;

    var add = function () {
        if ($element.hasClass('disabled')) {
            return;
        }
        var value = $element.val();
        var isArabic = value.isArabic();
        if (isArabic === true) {
            $element.addClass('rtl');
            $element.removeClass('ltr');
        } else if (isArabic === false) {
            $element.removeClass('rtl');
            $element.addClass('ltr');
        }
    };

    $element.on('input', function () {
        add();
    });

    add();
};

//onDomInserted({ selector: 'textarea' }, function (e) {
//    expandTextarea($(e));
//    addRtlToInput($(e));
//});

//onDomInserted({ selector: 'input' }, function (e) {
//    addRtlToInput($(e));
//});

onDomInserted({ selector: 'input.currency' }, function (e) {
    $(e).on("input", null, function () {
        var $input = $(e),
            value = $input.val(),
            num = parseFloat(value.replace(",", "")).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');

        $input.val(num);
    });
});

//Reference:
//http://www.onextrapixel.com/2012/12/10/how-to-create-a-custom-file-input-with-jquery-css3-and-php/
; (function ($) {
    var selectFileAr = 'اختر ملفًا';
    var selectFileEn = 'Select a File';

    var langArabic = lang === 'ar';

    // Browser supports HTML5 multiple file?
    var multipleSupport = typeof $('<input/>')[0].multiple !== 'undefined',
        isIE = /msie/i.test(navigator.userAgent);

    $.fn.customFile = function () {
        return this.each(function () {
            var $file = $(this).addClass('custom-file-upload-hidden'), // the original file input
                $wrap = $('<div class="file-upload-wrapper">'),
                $input = $('<input type="text" class="file-upload-input" />'),
                // Button that will be used in non-IE browsers
                $button = $('<button type="button" class="file-upload-button"> ' + (langArabic ? selectFileAr : selectFileEn) + '</button>'),
                // Hack for IE
                $label = $('<label class="file-upload-button" for="' + $file[0].id + '"> ' + (langArabic ? selectFileAr : selectFileEn) + '</label>');

            // Hide by shifting to the left so we
            // can still trigger events
            $file.css({
                position: 'absolute',
                left: '-9999px'
            });

            $wrap.insertAfter($file)
                .append($file, $input, (isIE ? $label : $button));

            // Prevent focus
            $file.attr('tabIndex', -1);
            $button.attr('tabIndex', -1);
            $label.attr('tabIndex', -1);

            $button.click(function () {
                $file.focus().click(); // Open dialog
            });

            $label.click(function () {
                $file.focus().click(); // Open dialog
            });

            $file.change(function () {
                var files = [], fileArr, filename;

                // If multiple is supported then extract
                // all filenames from the file array
                if (multipleSupport) {
                    fileArr = $file[0].files;
                    for (var i = 0, len = fileArr.length; i < len; i++) {
                        files.push(fileArr[i].name);
                    }
                    filename = files.join(', ');

                    // If not supported then just take the value
                    // and remove the path to just show the filename
                } else {
                    filename = $file.val().split('\\').pop();
                }

                $input.val(filename) // Set the value
                    .attr('title', filename) // Show filename in title tootlip
                    .focus(); // Regain focus
            });

            $input.on({
                blur: function () { $file.trigger('blur'); },
                keydown: function (e) {
                    if (e.which === 13) { // Enter
                        if (!isIE) { $file.trigger('click'); }
                    } else if (e.which === 8 || e.which === 46) { // Backspace & Del
                        // On some browsers the value is read-only
                        // with this trick we remove the old input and add
                        // a clean clone with all the original events attached
                        $file.replaceWith($file = $file.clone(true));
                        $file.trigger('change');
                        $input.val('');
                    } else if (e.which === 9) { // TAB
                        return;
                    } else { // All other keys
                        return false;
                    }
                }
            });
        });
    };

    // Old browser fallback
    if (!multipleSupport) {
        $(document).on('change', 'input.customfile', function () {
            var $this = $(this),
                // Create a unique ID so we
                // can attach the label to the input
                uniqId = 'customfile_' + (new Date()).getTime(),
                $wrap = $this.parent(),

                // Filter empty input
                $inputs = $wrap.siblings().find('.file-upload-input')
                    .filter(function () { return !this.value }),

                $file = $('<input type="file" id="' + uniqId + '" name="' + $this.attr('name') + '"/>');

            // 1ms timeout so it runs after all other events
            // that modify the value have triggered
            setTimeout(function () {
                // Add a new input
                if ($this.val()) {
                    // Check for empty fields to prevent
                    // creating new inputs when changing files
                    if (!$inputs.length) {
                        $wrap.after($file);
                        $file.customFile();
                    }
                    // Remove and reorganize inputs
                } else {
                    $inputs.parent().remove();
                    // Move the input so it's always last on the list
                    $wrap.appendTo($wrap.parent());
                    $wrap.find('input').focus();
                }
            }, 1);
        });
    }
}(jQuery));

$('.form-group input').on('input', function () {
    $(this).closest('.form-group').find('.field-validation-error').hide();
});

$('.form-group select').change(function () {
    $(this).closest('.form-group').find('.field-validation-error').hide();
});

window.notify = function (msg, type, autoHideDelay) {
    if (!msg) return;
    if (!type) type = 'info';

    type = type.toLowerCase();

    var $alert = $('<div class="alert alert-' + type + ' ' + (autoHideDelay > 0 ? 'alert-temp' : '') + '">' + msg + '</div>');

    if ($('body').hasClass('has-tabs')) {
        $('#content .nav.nav-tabs').after($alert);
    } else if ($('#content .breadcrumb').length > 0) {
        $('#content .breadcrumb').after($alert);
    } else {
        $('#content').prepend($alert);
    }

    if (autoHideDelay > 0) {
        setTimeout(function () {
            $alert.remove();
        }, autoHideDelay);
    }
};

if ($('.selectpicker').selectpicker !== undefined) {
    $('.selectpicker').selectpicker({
        liveSearch: true,
        noneSelectedText: 'اختر',
        noneResultsText: 'No results for {0}'
    });
}

// Custom-tabs

var customTabs = {};

customTabs.navigate = function (e) {
    customTabs.transformLine(e.target);
};

customTabs.transformLine = function (el) {
    customTabs.highlight.style.left = el.offsetLeft + 'px';
    customTabs.highlight.style.width = el.offsetWidth + 'px';
}

customTabs.init = function (element) {
    customTabs.transformLine(element[0].querySelector(':first-child'));
}

customTabs.link = function (element) {
    // add highlight element to DOM
    var h = document.createElement("div");
    h.classList.add('highlight');
    customTabs.highlight = element[0].appendChild(h);
    // init first tab function
    function init() {
        customTabs.transformLine(element[0].querySelector(':first-child'));
    }
    // init after digest cycle, allows dynamic tabs have rendered
    setTimeout(function () {
        customTabs.init(element);
    }, 0);
}

onDomInserted('.custom-tabs nav', function (e) {
    $(function () {
        customTabs.link(e);
    });
});

onDomInserted('.custom-tabs-link', function (e) {
    $(function () {
        $(e).click(function (e) {
            return customTabs.navigate(e);
        });
    });
});

function htmlEncode(value) {
    return $('<div/>').text(value).html();
}

function htmlDecode(value) {
    return $('<div/>').html(value).text();
}

window.elToReadOnly = function (e, addHiddenField) {
    var $e = $(e);
    var val = '';

    if (addHiddenField) {
        var name = $e.attr('name');
        val += '<input type="hidden" name="' + name + '" value="' + $e.val() + '" />';
    }

    var addFormControl = true;

    var xval = '';
    if ($e.is('textarea')) {
        if ($e.hasClass('rich-text-editor'))
            addFormControl = false;

        xval = htmlDecode($e.html());
    } else if ($e.is('select')) {
        xval = $e.find('option:selected').text();
    } else if ($e.is('input[type=radio]')) {
        if ($e.is(':checked')) {
            xval = $e.next().text();
        }

        $e.next().remove();
    } else {
        xval = $e.val();
    }

    if (xval.length > 0) {
        if (addFormControl) {
            val += '<div class="form-control disabled">' + xval + '</div>';
        } else {
            val += xval;
        }
    }

    $e.replaceWith(val);
}

onDomInserted('.readonly', function (e) {
    $(function () {
        window.elToReadOnly(e);
    })
});