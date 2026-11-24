var updateFieldValidation = function () {
    if ($(this).data('val') !== true) return;
    var parent = $(this).closest('.input-group');
    if (!parent || parent.length === 0)
        parent = $(this).closest('.form-group');

    var form = $(this).closest('form');

    if (form) {
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