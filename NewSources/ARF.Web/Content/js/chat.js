moment.locale('ar');
var lastpersonid = '';
var personslist = '';
var msgsloaded = false;

$(function () {
    var msgs = '';
    $('ul.people').empty();
    $('.toggle-chat span').css('display', 'none');
    // Reference the auto-generated proxy for the hub.  
    var chat = $.connection.chatHub;
    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (name, message) {
        // Add the message to the page. 
        $('#discussion').append('<li><strong>' + htmlEncode(name)
            + '</strong>: ' + htmlEncode(message) + '</li>');
    };

    // Set initial focus to messages input box.  
    $('#message').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
            // Call the Send method on the hub. 
            //string subject, string message, string toUsers, string toRoles, string toColleges, bool isAnnoucement, string inReplyTo
            var userId = $('.person.active').attr('data-chat');

            chat.server.sendMesage('', $('#message').val(), userId, '', '', false, userId);

            chat.client.renderMessage('', currUserId, username, '', '', $('#message').val(), userId !== null, '', new Date());
            // Clear text box and reset focus for next comment. 
            $('#message').val('').focus();

        });
        $('.toggle-chat').click(function () {
            chat.server.getChatBuddies(lastpersonid);
        });

    });



    chat.client.renderRegister = function (count) {
        chat.server.getChatBuddies('');
    };

    chat.client.renderUnregister = function (count) {
        chat.server.getChatBuddies('');
    };


    chat.client.renderUnseenMessageCount = function (count) {
        //console.log(count);
        if (count < 1) {
            $('.toggle-chat span').css('display', 'none');
        } else {
            $('.toggle-chat span').css('display', 'block');
            $('.toggle-chat span').html(count);
        }
        //console.log(count);
    };


    chat.client.renderChatBuddies = function (data, removeAll) {
        console.log(data);
        $('.more-div').remove();

        if (removeAll)
        {
            $('ul.people').empty();
        }

        personslist = $('ul.people').html();
        var firstpersonid = '';
        var messageslist = '';
        for (var i = 0; i < data.length; i++) {
            var person = data[i];
            var lasttime = '';
            var lastmsgprev = '';
            var linestat = 'offline';
            var photo = '';
            if (!(person.LastMessageTime == 'undefined' || person.LastMessageTime == null)) {
                lasttime = moment(person.LastMessageTime).fromNow();
            }
            if (!(person.LastMessagePreview == 'undefined' || person.LastMessagePreview == null)) {
                lastmsgprev = '<p><span class="preview">' + person.LastMessagePreview + '</span></p>';
            }
            if (person.IsOnline) {
                linestat = 'online';
            }

            if (person.PhotoUrl == 'undefined' || person.PhotoUrl == null || person.PhotoUrl == '') {
                photo = "http://www.qygjxz.com/data/out/190/5691490-profile-pictures.png";
            }

            personslist = personslist + '<li class="person" data-chat="' + person.UserId + '"><span class="' + linestat + '"></span><img src="' + photo + '" alt="" /><span class="name">' + person.Name + '</span><span class="time">' + lasttime + '</span><p><span class="role" style="font-size:11px; font-style:italic; color:#9e9e9e">(' + person.Role + ')</span></p>' + lastmsgprev + '</li>';
            //var convstrt = person.ConversationStartDateTime;
            //var msgs = '';
            //for (var x = 0; x < person.messagesList.length; x++) {
            //    var msg = person.messagesList[x];
            //    msgs = msgs + '<div class="bubble ' + msg.direction + '">' + msg.messageText + '<span style="font-size:8px; font-style:italic">' + msg.sentTime + '</span></div>';
            //}
            //var msgdiv = '<div class="chat" data-chat="' + person.userId + '"><div class="conversation-start"><span>' + convstrt + '</span></div>' + msgs + '</div>';
            //messageslist = messageslist + msgdiv;
            lastpersonid = person.userId;

            if (i == 0) {
                firstpersonid = person.UserId
            }
        }
        personslist = personslist + '<div class="text-center more-div"><a class="more-buddies btn btn-success btn-xs">Give Me More Buddies!</a></div>'
        $('ul.people').empty().append(personslist);
        //$('.right').find('.messages-list').empty().append(messageslist);
        //$('.chat[data-chat=' + firstpersonid + ']').addClass('active-chat');
        //$('.person[data-chat=' + firstpersonid + ']').addClass('active');
    };

    $(document).on('click', '.more-buddies', function () {
        console.log(lastpersonid);
        chat.server.getChatBuddies(lastpersonid);
    });

    $(document).on('click', '.person', function () {
        $(this).addClass('active').siblings().removeClass('active');
        $('.right .top .name').html($(this).find('.name').html());
        msgsloaded = false;
        chat.server.getMessageHistory($(this).data('chat'), null, 10);
    });

    //user.Id, user.UserProfile.GetFullName(), "", subject, message, isAnnoucement, inReplyTo, msg.Sent
    chat.client.renderMessage = function (msgId, userId, name, photo, subject, message, isAnnouncement, inReplyTo, sent) {
        var activeUserId = $('.person.active').attr('data-chat');

        if (activeUserId == userId) {
            var $chat = $('.chat');
            var direction = 'me';
            var tdir = 'text-right';

            if (userId != currUserId) {
                direction = 'you';
                tdir = 'text-left';
            }

            var html = '<div class="bubble ' + direction + '" data-messageId="' + msgId + '">' + message + '<p class="' + tdir + '"><span style="font-size: 12px;    font-style: italic;    color: #464545;">' + moment(sent).fromNow() + '</span></div></p>'

            $chat.append(html);
        }

    };

    chat.client.renderMessageHistory = function (data) {
        console.log(msgsloaded);
        msgs = '';
        $('.cog-loader').remove();
        if (msgsloaded == false) {
            msgs = '<div class="chat">';
        }
        if (msgsloaded == true) {
            if (data.length > 6) {
                $('.load-msgs-btn').remove();
                msgs = msgs + '<div class="text-center load-msgs-btn col-xs-12"><a class="btn btn-xs btn-default load-more-msgs">Load More<i class="fa fa-cog pull-right"></i></a></div>"'
            }
        }

        for (var i = 0; i < data.length; i++) {
            var d = data[i];
            if (d.userId == currUserId) {
                direction = 'me';
                tdir = 'text-right';
            } else {
                direction = 'you';

                if ((i % 2) == 0) {
                    direction = 'me';
                } else {
                    direction = 'you';
                }

                tdir = 'text-left';
            }

            msgs = msgs + '<div class="bubble ' + direction + '" data-messageId="' + d.MessageId + '">' + d.Body + '<p class="' + tdir + '"><span style="font-size: 12px;    font-style: italic;    color: #464545;">' + moment(d.SentOn).fromNow() + '</span></div></p>';
        }
        if (msgsloaded == false) {
            msgs = msgs + '</div>';
            $('div.right').find('.messages-list').empty().append(msgs);
            $('.chat').addClass('active-chat');
        } else {
            $('.chat.active-chat').prepend(msgs);
        }


        if (msgsloaded == false) {
            var timeout = setTimeout(function () {
                $('.chat.active-chat').animate({
                    scrollTop: $('.chat.active-chat')[0].scrollHeight
                }, 750, "linear",
                    function () { } // callback method use this space how you like
                );
                clearTimeout(timeout);
                console.log("msgsloaded = " + msgsloaded)
            }, 600);
            $(document).find('.chat.active-chat').scroll(scrollingFunction);
        }

        msgsloaded = true;

        //$('.right').find('.messages-list').empty().append(messageslist);
        //$('.chat[data-chat=' + firstpersonid + ']').addClass('active-chat');
        //$('.person[data-chat=' + firstpersonid + ']').addClass('active');
    };

    $(document).on('click', '.load-msgs-btn', function () {
        chat.server.getMessageHistory($(this).data('chat'), $(this).find('.bubble').first().data('messageid'), 10);
    });


    function scrollingFunction(event) {
        if (msgsloaded == true && $(this).scrollTop() == 0) {
            $(this).prepend('<div class="text-center cog-loader"><a><i class="fa fa-3x fa-spin fa-cog"></i></a></div>');
            //$('.load-msgs-btn').remove();
            chat.server.getMessageHistory($(this).data('chat'), $(this).find('.bubble').first().data('messageid'), 10);
            //console.log('top of chat div');
        }

        if (msgsloaded == true && $(this).scrollTop() == 5) {
            $('.load-msgs-btn').remove();
        }
    }

});


// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
