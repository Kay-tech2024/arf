"use strict";

var options = {
    mode: 'edit',
    surveyId: '',
    isLoaded: false,
    submitUrl: '',
    loadUrl: '',
    onSubmit: null,
    onLoad: null,
    selectorNoComponents: '#divNoItems',
    selectorCanvas: '#surveyBuilder',
    selectorComponentSearchbox: '#txtSearchComponents',
    selectorComponentListItems: '#lstComponents .survey-component',
    selectorItem: ''
};

var builder = {};

builder.options = options;
builder.items = [];

var components = {
    component: '<li class="survey-item survey-{type}"><i class="zmdi zmdi-more-vert zmdi-hc-fw"></i>{content}<div class="survey-item-delete"><button class="btn bgm-deeporange btn-icon btn-xs waves-effect waves-circle waves-float"><i class="zmdi zmdi-close"></i></button></div>{options}</li>',
    question_number_container: '<div class="question-number">{question_number}</div>',
    question_tf: '{question_number_container}  <div class="question-content"><textarea class="form-control" placeholder="Please put your question here.." ></textarea></div><div class="question-choices"><ol><li><input type="text" placeholder="Option 1"></li><li><input type="text" placeholder="Option 2"></li></ol></div>',
    question_mcq: '{question_number_container} <div class="question-content"><textarea class="form-control" placeholder="Please put your question here.." ></textarea></div><div class="question-choices"><ol><li><input type="text" placeholder="Option 1"></li><li><input type="text" placeholder="Option 2"></li><li><input type="text" placeholder="Option 3"></li><li class="add-choice">Add choice..</li></ol></div>',
    question_short: '{question_number_container} <div class="question-content"><textarea class="form-control" placeholder="Please put your question here.." ></textarea></div>',
    question_long: '{question_number_container} <div class="question-content"><textarea class="form-control" placeholder="Please put your question here.." ></textarea></div>',
    question_choice: '<li><input data-choice-id="{choiceId}" type="text" placeholder="{placeholder}">{value}</li>',
    question_options_container: '<div class="question-options">{option_list}</div>',
    question_option: '<div class="{type} checkbox m-b-15"><label><input type="checkbox" value=""><i class="input-helper"></i>{label}</label></div>',
    title: '<h1 contenteditable>Put your title here</h1>',
    text: '<textarea class="form-control"></textarea>',
    richtext: '<div class=\"html-editor\"></div>',
    image: '"<div class="fileinput fileinput-new" data-provides="fileinput"><div class="fileinput-preview thumbnail" data-trigger="fileinput"></div><div><span class="btn btn-info btn-file"><span class="fileinput-new">Select image</span><span class="fileinput-exists">Change</span><input type="file" name="..."></span><a href="#" class="btn btn-danger fileinput-exists" data-dismiss="fileinput">Remove</a></div></div>',
    separator: '<hr />'
};

var components_preview = {
    component: '<li class="survey-item survey-{type}"><i class="zmdi zmdi-more-vert zmdi-hc-fw"></i>{content}<div class="survey-item-delete"><button class="btn bgm-deeporange btn-icon btn-xs waves-effect waves-circle waves-float"><i class="zmdi zmdi-close"></i></button></div>{options}</li>',
    question_number_container: '<div class="question-number">{question_number}</div>',
    question_tf: '{question_number_container}  <div class="question-content"><textarea class="form-control" placeholder="Please put your question here.." >{content}</textarea></div><div class="question-choices"><ol>{question_choices}</ol></div>',
    question_mcq: '{question_number_container} <div class="question-content"><textarea class="form-control" placeholder="Please put your question here.." >{content}</textarea></div><div class="question-choices"><ol>{question_choices}</ol></div>',
    question_short: '{question_number_container} <div class="question-content"><textarea class="form-control" placeholder="Please put your question here.." >{content}</textarea></div>',
    question_long: '{question_number_container} <div class="question-content"><textarea class="form-control" placeholder="Please put your question here.." >{content}</textarea></div>',
    question_choice: '<li><input data-choice-id="{choiceId}" type="text" placeholder="{placeholder}" value="{value}" /></li>',
    question_add_choice: '<li class="add-choice">Add choice..</li>',
    question_options_container: '<div class="question-options">{option_list}</div>',
    question_option: '<div class="{type} checkbox m-b-15"><label><input type="checkbox" value=""><i class="input-helper"></i>{label}</label></div>',
    title: '<h1 contenteditable>{content}</h1>',
    text: '<textarea class="form-control">{content}</textarea>',
    richtext: '<div class=\"html-editor\">{content}</div>',
    image: '"<div class="fileinput fileinput-new" data-provides="fileinput"><div class="fileinput-preview thumbnail" data-trigger="fileinput"></div><div><span class="btn btn-info btn-file"><span class="fileinput-new">Select image</span><span class="fileinput-exists">Change</span><input type="file" name="..."></span><a href="#" class="btn btn-danger fileinput-exists" data-dismiss="fileinput">Remove</a></div></div>',
    separator: '<hr />'
};

var getNumberOfQuestions = function (start, end) {
    if (!start) start = 0;
    if (!end) end = builder.items.length - 1;

    var currentQuestionsCount = 0;
    for (var i = start; i <= end; i++) {
        switch (builder.items[i].type) {
            case 'mcq':
            case 'mcqm':
            case 'shortq':
            case 'saq':
            case 'tfq':
            case 'longq':
            case 'laq':
                currentQuestionsCount++;
                break;
        }
    }

    return currentQuestionsCount;
};

var addNewItemToCanvas = function (type) {
    if (options.mode !== 'edit') return;
    if (!type) return;

    var currentQuestionsCount = getNumberOfQuestions();
    var order = currentQuestionsCount + 1;
    var template = components.component;
    var options_html = '';
    switch (type) {
        case 'tfq':
            template = template.replace('{content}', components.question_tf);
            options_html += components.question_option.replace('{type}', 'option-required').replace('{label}', 'Required');
            break;
        case 'mcq':
        case 'mcqm':
            template = template.replace('{content}', components.question_mcq);
            options_html += components.question_option.replace('{type}', 'option-required').replace('{label}', 'Required');
            options_html += components.question_option.replace('{type}', 'option-allow-multiple').replace('{label}', 'Allow Multiple Selection');
            break;
        case 'shortq':
        case 'saq':
            template = template.replace('{content}', components.question_short);
            options_html += components.question_option.replace('{type}', 'option-required').replace('{label}', 'Required');
            break;
        case 'longq':
        case 'laq':
            template = template.replace('{content}', components.question_long);
            options_html += components.question_option.replace('{type}', 'option-required').replace('{label}', 'Required');
            break;
        case 'title':
            template = template.replace('{content}', components.title);
            break;
        case 'text':
            template = template.replace('{content}', components.text);
            break;
        case 'richtext':
            template = template.replace('{content}', components.richtext);
            break;
        case 'separator':
            template = template.replace('{content}', components.separator);
            break;
        case 'image':
            template = template.replace('{content}', components.image);
            break;
        default:
            return;
    }

    if (options_html.length > 0) {
        template = template.replace('{options}', components.question_options_container.replace('{option_list}', options_html));
    } else {
        template = template.replace('{options}', '');
    }

    template = template.replace('{question_number_container}', components.question_number_container).replace('{question_number}', order);
    template = template.replace('{type}', type);

    $(options.selectorCanvas).append(template);

    var el = $(options.selectorCanvas + ' .survey-item');

    builder.items.push({
        isNew: true,
        isChanged: false,
        type: type,
        content: '',
        order: order,
        isRequired: false,
        allowMultiple: false,
        choice: [],
        el: el[el.length - 1]
    });
    onItemsChanged();
};

var removeItemFromCanvas = function (item) {
    if (options.mode !== 'edit') return;
    if (item === undefined) return;

    var el = null;
    var elIndex = -1;

    if (Number.isInteger(item)) {
        elIndex = item;
        el = builder.items[item];
    }
    else {
        if (item.el !== null)
            el = item.el;
        else
            el = item;

        for (var i = 0; i < builder.items.length; i++) {
            if (builder.items[i].el === el) {
                elIndex = i;

                break;
            }
        }
    }

    if (elIndex < 0) return;

    var currentQuestionsCount = getNumberOfQuestions(0, elIndex - 1);

    for (var i = elIndex + 1; i < builder.items.length; i++) {
        var questionCounter = $(builder.items[i].el).find('.question-number');
        if (questionCounter) {
            questionCounter.html(currentQuestionsCount + 1);
            currentQuestionsCount++;
        }
    }

    builder.items.splice(elIndex, 1);

    $(el).remove();

    validate();
    onItemsChanged();
};

var onItemsChanged = function (el) {
    if (!builder.items || builder.items.length === 0) {
        $(options.selectorCanvas).hide();
        $(options.selectorNoComponents).show();
    }
    else {
        $(options.selectorCanvas).show();

        var adjustment = null;
        $(options.selectorCanvas).sortable({
            handle: 'i.zmdi.zmdi-more-vert',
            // animation on drop
            onDrop: function ($item, container, _super) {
                var $clonedItem = $('<li />').css({ height: 0 });
                $item.before($clonedItem);
                $clonedItem.animate({ 'height': $item.height() });

                $item.animate($clonedItem.position(), function () {
                    $clonedItem.detach();
                    _super($item, container);
                });
            },

            // set $item relative to cursor position
            onDragStart: function ($item, container, _super) {
                var offset = $item.offset(),
                    pointer = container.rootGroup.pointer;

                adjustment = {
                    left: pointer.left - offset.left,
                    top: pointer.top - offset.top
                };

                _super($item, container);
            },
            onDrag: function ($item, position) {
                $item.css({
                    left: position.left - adjustment.left,
                    top: position.top - adjustment.top
                });
            }
        });

        $(options.selectorNoComponents).hide();
        //smoothScrollTo(builder.items[builder.items.length - 1].el);
    }
};

var findItemByEl = function (el) {
    if (!el) return;

    if (el.length > 0)
        el = el[0];

    var elIndex = null;
    for (var i = 0; i < builder.items.length; i++) {
        if (builder.items[i].el === el) {
            elIndex = i;
            break;
        }
    }

    if (elIndex < 0) {
        alert('failed :(');
        return;
    }

    return builder.items[elIndex];
};

var refreshItemView = function (item) {
    if (!item) return;
    var type = item.type.toLowerCase();

    var options_html = '';
    var template = components_preview.component;
    switch (type) {
        case 'tfq':
            template = template.replace('{content}', components_preview.question_tf);
            options_html += components.question_option.replace('{type}', 'option-required').replace('{label}', 'Required');
            break;
        case 'mcq':
        case 'mcqm':
            template = template.replace('{content}', components_preview.question_mcq);
            options_html += components.question_option.replace('{type}', 'option-required').replace('{label}', 'Required');
            options_html += components.question_option.replace('{type}', 'option-allow-multiple').replace('{label}', 'Allow Multiple Selection');
            break;
        case 'shortq':
        case 'saq':
            template = template.replace('{content}', components_preview.question_short);
            options_html += components.question_option.replace('{type}', 'option-required').replace('{label}', 'Required');
            break;
        case 'longq':
        case 'laq':
            template = template.replace('{content}', components_preview.question_long);
            options_html += components.question_option.replace('{type}', 'option-required').replace('{label}', 'Required');
            break;
        case 'title':
            template = template.replace('{content}', components_preview.title);
            break;
        case 'text':
            template = template.replace('{content}', components_preview.text);
            break;
        case 'richtext':
            template = template.replace('{content}', components_preview.richtext);
            break;
        case 'separator':
            template = template.replace('{content}', components_preview.separator);
            break;
        case 'image':
            template = template.replace('{content}', components_preview.image);
            break;
        default:
            return;
    }

    if (options_html.length > 0) {
        template = template.replace('{options}', components.question_options_container.replace('{option_list}', options_html));
    } else {
        template = template.replace('{options}', '');
    }

    template = template.replace('{question_number_container}', components.question_number_container).replace('{question_number}', item.order);
    template = template.replace('{type}', type);
    if (!item.content)
        item.content = '';
    template = template.replace('{content}', item.content);

    var choices_html = '';
    if (type === 'tfq' || type === 'mcq' || type === 'mcqm') {
        if (!item.choices || item.choices.length === 0) {
            if (type === 'mcq' || type === 'mcqm') {
                for (var i = 0; i < 3; i++) {
                    var choiceId = item.choices[i].choiceId;
                    if (!(choiceId > 0))
                        choiceId = '';

                    choices_html += components_preview.question_choice.replace('{choiceId}', choiceId).replace('{placeholder}', 'Choice ' + (i + 1)).replace('{value}', '');
                }
            } else {
                for (var i = 0; i < 2; i++) {
                    var choiceId = item.choices[i].choiceId;
                    if (!(choiceId > 0))
                        choiceId = '';

                    choices_html += components_preview.question_choice.replace('{choiceId}', choiceId).replace('{placeholder}', 'Choice ' + (i + 1)).replace('{value}', '');
                }
            }
        } else {
            for (var i = 0; i < item.choices.length; i++) {
                var choiceId = item.choices[i].choiceId;
                if (!(choiceId > 0))
                    choiceId = '';

                choices_html += components_preview.question_choice.replace('{choiceId}', choiceId).replace('{placeholder}', 'Choice ' + (i + 1)).replace('{value}', item.choices[i].content);
            }
        }

        if (type === 'mcq' || type === 'mcqm') {
            choices_html += components_preview.question_add_choice;

            if (type === 'mcqm') {
            }
        }
    }

    template = template.replace('{question_choices}', choices_html.replace('{choiceId}', ''));

    var itemIndex = -1;
    var els = null;
    if (item.el) {
        for (var i = 0; i < builder.items.length; i++) {
            if (builder.items[i].el == item.el) {
                itemIndex = i;
                break;
            }
        }

        $(item.el).replaceWith(template);
        els = $(options.selectorCanvas + ' .survey-item');
        item.el = els[itemIndex];
    } else {
        $(options.selectorCanvas).append(template);
        els = $(options.selectorCanvas + ' .survey-item');
        item.el = els[els.length - 1];
    }

    if (item.isRequired) {
        $(item.el).find('.question-options .option-required input').prop('checked', true);
    }

    if (item.allowMultiple) {
        $(item.el).find('.question-options .option-allow-multiple input').prop('checked', true);
    }

    onItemsChanged();
};

var refreshItemModel = function (item, validateQuestion, validateChoices) {
    if (!item) return;

    var valid = true;
    switch (item.type) {
        case 'mcq':
        case 'mcqm':
        case 'tfq':
            var txtContent = $(item.el).find('.question-content textarea');
            item.content = txtContent.val();
            item.isRequired = $(item.el).find('.question-options .option-required input:checked').length > 0;
            item.allowMultiple = item.type !== 'tfq' && $(item.el).find('.question-options .option-allow-multiple input:checked').length > 0;
            item.choices = [];

            var choices = $(item.el).find('.question-choices input');
            for (var j = 0; j < choices.length; j++) {
                var choiceId = $(choices[j]).attr('data-choice-id');
                if (choiceId === undefined || !(choiceId > 0))
                    choiceId = '';

                var choice = {
                    choiceId: choiceId,
                    content: $(choices[j]).val().trim()
                };

                item.choices.push(choice);

                // validation

                if (choice.content === '') {
                    if (validateChoices) {
                        $(choices[j]).parent().addClass('has-error');
                        $(choices[j]).parent().find('.help-block').remove();
                        $(choices[j]).parent().append('<div class="help-block">Choice can\'t be empty</div>');
                    }

                    valid = false;
                } else {
                    var exists = false;

                    for (var k = 0; k < j; k++) {
                        if (item.choices[k].content === choice.content) {
                            exists = true;
                            break;
                        }
                    }

                    if (validateChoices) {
                        $(choices[j]).parent().removeClass('has-error');
                        $(choices[j]).parent().find('.help-block').remove();
                    }

                    if (exists) {
                        valid = false;
                        if (validateChoices) {
                            $(choices[j]).parent().addClass('has-error');
                            $(choices[j]).parent().find('.help-block').remove();
                            $(choices[j]).parent().append('<div class="help-block">Choice can\'t be repeated</div>');
                        }
                    }
                }
            }

            break;
        case 'shortq':
        case 'saq':
            var txtContent = $(item.el).find('.question-content textarea');
            item.isRequired = $(item.el).find('.question-options .option-required input:checked').length > 0;
            item.content = txtContent.val();
            break
        case 'longq':
        case 'laq':
            var txtContent = $(item.el).find('.question-content textarea');
            item.isRequired = $(item.el).find('.question-options .option-required input:checked').length > 0;
            item.content = txtContent.val();
            break;
        default:
            return true;
    }

    // validation

    if (item.content.trim() === '') {
        if (validateQuestion) {
            $(item.el).find('.question-content').addClass('has-error');
            $(item.el).find('.question-content .help-block').remove();
            $(item.el).find('.question-content').append('<div class="help-block">Question can\'t be empty</div>');
        }

        valid = false;
    } else {
        if (validateQuestion) {
            $(item.el).find('.question-content').removeClass('has-error');
            $(item.el).find('.question-content .help-block').remove();
        }
    }

    if (!valid) {
        $('#btnSubmit').prop('disabled', true);
    } else {
        $('#btnSubmit').prop('disabled', false);
    }

    return valid;
};

var refreshModel = function (validateQuestion, validateChoices) {
    var isValid = true;
    for (var i = 0; i < builder.items.length; i++) {
        isValid = isValid && refreshItemModel(builder.items[i], validateQuestion, validateChoices);
    }

    return isValid;
};

var refreshView = function () {
    $(options.selectorCanvas).empty();
    for (var i = 0; i < builder.items.length; i++) {
        refreshItemView(builder.items[i]);
    }
};

var validate = function (validateQuestion, validateChoices) {
    if (!builder.items || builder.items.length === 0) {
        $('#btnSubmit').prop('disabled', true);
        return false;
    }
    return refreshModel(validateQuestion, validateChoices);
};

var preview = function (containerEl) {
    if (!containerEl || !builder.items || builder.items.length === 0) return;
    refreshModel();

    var html = $(options.selectorCanvas).clone();
    $(containerEl).addClass('survey-preview');
    $(containerEl).empty();
    $(containerEl).append(html);
};

var submit = function () {
    if (!validate()) {
        alert('Survey questions are not valid');
        return;
    }

    if (!options.submitUrl) {
        alert('No submitUrl specified');
        return;
    }

    var model = { surveyId: options.surveyId, questions: [] };

    var order = 0;
    for (var i = 0; i < builder.items.length; i++) {
        switch (builder.items[i].type) {
            case 'mcq':
            case 'mcqm':
            case 'shortq':
            case 'saq':
            case 'laq':
            case 'tfq':
            case 'longq':
                var q =
                {
                    questionId: builder.items[i].questionId ? builder.items[i].questionId : null,
                    isRequired: builder.items[i].isRequired,
                    allowMultiple: builder.items[i].allowMultiple,
                    type: builder.items[i].type.toUpperCase(),
                    order: order++,
                    content: builder.items[i].content,
                    choices: builder.items[i].choices ? builder.items[i].choices : null
                };

                model.questions.push(q);

                break;
        }
    }

    console.log(model);

    $.ajax({
        type: "POST",
        url: options.submitUrl,
        data: model,
        datatype: "json",
        success: function (res) {
            if (res.status === 200 && json.success) {
                for (var i = 0; i < builder.items.length; i++) {
                    switch (builder.items[i].type) {
                        case 'mcq':
                        case 'mcqm':
                        case 'shortq':
                        case 'saq':
                        case 'laq':
                        case 'tfq':
                        case 'longq':
                            builder.items[i].questionId = res.data[i];

                            break;
                    }
                }
            }

            if (isFunction(options.onSubmit)) {
                options.onSubmit();
            }
        }
    });
};

var load = function (force) {
    if ((!force && options.isLoaded) || !options.loadUrl) {
        if (isFunction(options.onLoad)) {
            options.onLoad();
        }
        return;
    }

    $.ajax({
        type: "GET",
        url: options.loadUrl + '/' + options.surveyId,
        datatype: "json",
        success: function (json) {
            if (json.status) {
                builder.items = [];
                for (var i = 0; i < json.data.questions.length; i++) {
                    var q = json.data.questions[i];
                    var item = {
                        questionId: q.questionId,
                        content: q.content,
                        type: q.type.toLowerCase(),
                        choices: q.choices,
                        isRequired: q.isRequired,
                        order: q.order
                    };

                    if (item.type === 'mcqm') {
                        item.type = 'mcq';
                        item.allowMultiple = true;
                    }

                    builder.items.push(item);
                }

                if (!builder.items || builder.items.length === 0) {
                    $(options.selectorCanvas).hide();
                    $(options.selectorNoComponents).show();
                } else {
                    builder.items.sort(function (a, b) {
                        return a.order - b.order;
                    });
                }

                refreshView();

                if (isFunction(options.onLoad)) {
                    options.onLoad();
                    options.isLoaded = true;
                }
            }
        }
    });
};

$(options.selectorComponentListItems).click(function (e) {
    var type = $(this).data('type');
    if (!type) return false;

    addNewItemToCanvas(type);

    return false;
});

$(document).on('keydown', options.selectorCanvas + ' .question-choices input', function (ev) {
    var parent = $(this).closest('.question-choices');
    var choices = parent.find('input');
    var currentIndex = -1;
    for (var i = 0; i < choices.length; i++) {
        if (choices[i] === this) {
            currentIndex = i;
            break;
        }
    }

    if (currentIndex < 0) return;

    if (ev.which === 13) {
        // enter key
        if (currentIndex === choices.length - 1) {
            $(parent).find('.add-choice').before(components.question_choice.replace('{value}', '').replace('{placeholder}', 'Option ' + (choices.length + 1)));
            $(parent).last('input').focus();
        } else {
            $(choices[currentIndex + 1]).focus();
        }

        ev.preventDefault();
        return false;
    } else if (ev.which === 8) {
        // backspace key
        if ($(this).val().trim() === '' && choices.length > 2) {
            $(this).closest('li').remove();

            if (currentIndex > 0)
                $(choices[currentIndex - 1]).focus();
            else
                $(choices[currentIndex + 1]).focus();

            var choices = parent.find('input');

            for (var i = 0; i < choices.length; i++) {
                $(choices[i]).attr('placeholder', 'Option ' + (i + 1));
            }

            ev.preventDefault();
            return false;
        }
    }
    else if (ev.which === 38) {
        // up key
        if (currentIndex > 0)
            $(choices[currentIndex - 1]).focus();
    }
    else if (ev.which === 40) {
        // down key
        if (currentIndex < choices.length - 1)
            $(choices[currentIndex + 1]).focus();
    }
});

$(document).on('click', options.selectorCanvas + ' .question-choices .add-choice', function () {
    var parent = $(this).closest('.question-choices');
    var choices = parent.find('input');
    $(this).before(components.question_choice.replace('{value}', '').replace('{placeholder}', 'Option ' + (choices.length + 1)));
    $(parent).last('input').focus();
});

$(document).on('click', options.selectorCanvas + ' .survey-item-delete', function () {
    var el = $(this).closest('.survey-item');
    if (!el || el.length === 0) return;
    var item = findItemByEl(el);
    if (item.isChanged) {
        if (confirm('Are you sure?')) {
            removeItemFromCanvas(item);
        }
    } else {
        removeItemFromCanvas(item);
    }
});

$(document).on('input', options.selectorCanvas + ' .question-content textarea', function () {
    var el = $(this).closest('li.survey-item');
    if (!el) return;
    el = el[0];
    for (var i = 0; i < builder.items.length; i++) {
        if (builder.items[i].el === el) {
            refreshItemModel(builder.items[i], true);
            return;
        }
    }
});

$(document).on('blur', options.selectorCanvas + ' .question-content textarea', function () {
    var el = $(this).closest('li.survey-item');
    if (!el) return;
    el = el[0];
    for (var i = 0; i < builder.items.length; i++) {
        if (builder.items[i].el === el) {
            refreshItemModel(builder.items[i], true);
            return;
        }
    }
});

$(document).on('input', options.selectorCanvas + ' .question-choices input', function () {
    var el = $(this).closest('li.survey-item');
    if (!el) return;
    el = el[0];
    for (var i = 0; i < builder.items.length; i++) {
        if (builder.items[i].el === el) {
            refreshItemModel(builder.items[i], false, true);
            return;
        }
    }
});

$(document).on('blur', options.selectorCanvas + ' .question-choices input', function () {
    var el = $(this).closest('li.survey-item');
    if (!el) return;
    el = el[0];
    for (var i = 0; i < builder.items.length; i++) {
        if (builder.items[i].el === el) {
            refreshItemModel(builder.items[i], false, true);
            return;
        }
    }
});

$(options.selectorComponentSearchbox).on('input', function () {
    var query = $(this).val().toLowerCase();

    var cmps = $(options.selectorComponentListItems);
    for (var i = 0; i < cmps.length; i++) {
        if (!query || query.trim().length === 0 || $(cmps[i]).text().toLowerCase().indexOf(query) >= 0)
            $(cmps[i]).fadeIn();
        else
            $(cmps[i]).fadeOut();
    }
});

builder.addNewItemToCanvas = addNewItemToCanvas;
builder.removeItemFromCanvas = removeItemFromCanvas;
builder.getNumberOfQuestions = getNumberOfQuestions;
builder.validate = validate;
builder.submit = submit;
builder.preview = preview;
builder.load = load;
window.builder = builder;