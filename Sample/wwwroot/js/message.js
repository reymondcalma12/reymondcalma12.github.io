$(document).ready(function () {

    let receiverId;
    let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

    let curentUser = $("#currentUserId").val();



    connection.on("ReceiveAllUsers", function (users) {
        $('#usersList').empty();
        users.forEach(function (user) {

            let userLink = $("<a>", {
                href: "#",
                text: user.name,
                class: "user-link",
                click: function () {
                    GetMessages(user.id);
                    receiverId = user.id;
                    $("#userName").text(user.name);
                }
            });
            $("#usersList").append(userLink);
        });
    });

    connection.on("ReceiveMessages", function (messages) {

        var chatContainer = $('.chat-container');

        if (messages.length > 0) {

            chatContainer.empty();
            messages.forEach(function (msg) {

                var messageClass = msg.senderId === curentUser ? 'sent' : 'received';

                var dateObj = new Date(msg.date);

                var today = new Date();
                var yesterday = new Date(today.getTime() - (24 * 60 * 60 * 1000));

                let messageTime;

                if (dateObj.toDateString() === today.toDateString()) {
                    messageTime = dateObj.toLocaleString('en-US', {
                        hour: 'numeric',
                        minute: 'numeric',
                        hour12: true
                    });
                } else if (dateObj.toDateString() === yesterday.toDateString()) {
                    messageTime = 'Yesterday ' + dateObj.toLocaleString('en-US', {
                        hour: 'numeric',
                        minute: 'numeric',
                        hour12: true
                    });
                } else {
                    messageTime = dateObj.toLocaleString('en-US', {
                        year: 'numeric',
                        month: 'short',
                        day: 'numeric',
                        hour: 'numeric',
                        minute: 'numeric',
                        hour12: true
                    });
                }
                var messageHtml = `
                <div class="chat-message ${messageClass}">
                    <div class="message-content">${msg.text}</div>
                    <div class="message-time">${messageTime}</div>
                </div>
            `;
                chatContainer.append(messageHtml);
                scrollChatContainer();
            });
         
        }
        else {
            chatContainer.empty();
            var message = `<div class="NoMessageResult">
                                    <h3>No Messages for this Person</h3>
                                  </div>`;
            chatContainer.append(message);
        }

    });

    $(".buttonSend").on("click", function () {

        var messageValue = $("#messageValue").val();

        if (messageValue != "") {
            GetMessages(receiverId);
            connection.invoke("SendMessage", receiverId, messageValue)
                .catch(function (err) {
                    return console.error(err.toString());
                });
            $("#messageValue").val("");
        }
        else {
            alert("PLease Input a Message!");
        }


    });

    function GetAllUsers() {
        connection.invoke("GetAllUsers")
            .catch(function (err) {
                return console.error(err.toString());
            });
    }

    function GetMessages(userId) {
        connection.invoke("GetMessages", userId)
            .catch(function (err) {
                return console.error(err.toString());
            });
    }

    function scrollChatContainer() {
        var chatContainer = $('.chat-container');
        if (chatContainer.is(':visible')) {
            chatContainer.scrollTop(chatContainer[0].scrollHeight);
        }
    }

    connection.start().then(function () {
        GetAllUsers();
    }).catch(function (err) {
        return console.error(err.toString());
    });

});