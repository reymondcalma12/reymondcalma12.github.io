let receiverId = "";


if (receiverId != "") {
    GetMessages(receiverId);
}

$(document).ready(function () {

    let connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    let curentUser = $("#currentUserId").val();

    setInterval(function () {
        updateUserStatus();
    }, 1000);



    //$("#updateProfile").on("click", function () {

    //    event.preventDefault();

    //    var profile = $("#profileString").val();
    //    var name = $("#Name").val();
    //    var email = $("#Email").val();
    //    var number = $("#Number").val();

    //    $.ajax({
    //        type: 'GET',
    //        url: "/Account/UpdateProfile",
    //        data: { profil: profile, nam: name, emai: email, numbe: number },
    //        dataType: "json",
    //        success: function (result) {
    //            alert(result);
    //        },
    //        error: function (req, status, error) {
    //            console.log(status);
    //        }
    //    });

    //});



    $("#profileLink").on("click", function () {
            $.ajax({
                type: 'GET',
                url: "/Account/ViewUserProfile",
                data: { id: receiverId },
                dataType: "json",
                success: function (result) {
                    console.log(result);
                    $("#usersName").text(result.name);
                    $("#usersUserName").text(result.userName);
                    $("#usersNumber").text(result.phoneNumber);
                    $("#edit").hide();
                },
                error: function (req, status, error) {
                    console.log(status);
                }
            });

    });

    $(".searchBtn").on("click", function () {

       var text = $("#searchText").val();

        if (text != "") {
            $.ajax({
                type: 'GET',
                url: "/Account/ViewUserProfile",
                data: { id: receiverId },
                dataType: "json",
                success: function (result) {
                    console.log(result);
                },
                error: function (req, status, error) {
                    console.log(status);
                }
            });
        }
        else {
            alert("Plese Input First");
        }


    });

    connection.on("GetUsersWithMessages", function (newMessages) {

        $(".messageWithUser").empty();
        newMessages.forEach(function (message) {
          
            let userLink = $("<div>", {
                html: `<h5>${message.senderName}</h5><p><strong>${message.message.text}</strong></p>`,
                class: "messageList",
                click: function () {
                    receiverId = message.senderId;
                    $("#userName").text(message.senderName);
                    $("#profileLink").text("View Profile");
                    connection.invoke("GetMessages", message.senderId);
                }
            });
            $(".messageWithUser").append(userLink);
        });
    });

    connection.on("ReceiveAllUsers", function (users) {
        $("#usersList").empty();
        users.forEach(function (user) {

            let dotColor = user.online ? 'green' : 'red';

            let userLink = $("<a>", {
                href: "#",
                html: `<span style='color: ${dotColor}; font-size:20px;'>\u2022</span>&nbsp&nbsp&nbsp${user.name}`,
                class: "user-link user-link-" + user.id,
                click: function () {
                    receiverId = user.id;
                    $("#userName").text(user.name);
                    $("#profileLink").text("View Profile");
                    connection.invoke("GetMessages", user.id);
                }
            });

            setTimeout(function () {
                user.online = false;
                dotColor = 'red';
                userLink.html(`<span style='color: ${dotColor}; font-size:20px;'>\u2022</span>&nbsp&nbsp&nbsp${user.name}`);
            }, 10000);


            $("#usersList").append(userLink);
        });
    });

    connection.on("NotifyMessage", function (messages) {

        const today = new Date();
        const timeString = today.toTimeString();

        const timeComponents = timeString.split(' ')[0].split(':');
        const currentHour = parseInt(timeComponents[0], 10);
        const currentMinutes = parseInt(timeComponents[1], 10);

        messages.forEach(function (msg) {

            const messageDate = new Date(msg.date);
            const messageHourMinute = messageDate.getHours() * 60 + messageDate.getMinutes();

            console.log(msg.date);
            console.log(currentHour);
            console.log(currentMinutes);

            if (currentHourMinute == messageHourMinute) {
                console.log("The message was sent at the same time (hour and minute) as the current time!");
            } else {
                console.log("The message was not sent at the same time (hour and minute) as the current time.");
            }

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

        event.preventDefault();

        var messageValue = $("#messageValue").val();

        if (receiverId != "") {

            if (messageValue != "") {
                connection.invoke("SendMessage", receiverId, messageValue)
                    .catch(function (err) {
                        return console.error(err.toString());
                    });
                $("#messageValue").val("");

                GetMessages(receiverId);
            }
            else {
                alert("PLease Input a Message!");
            }

        }
        else {
            alert("PLease Select User!");
        }
    });

    function updateUserStatus() {
        GetAllUsers();
    }

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

    function GetNewMesage() {
        connection.invoke("GetNewMessages")
            .catch(function (err) {
                return console.error(err.toString());
            });
    }

    function GetMessagesWithUsers() {
        connection.invoke("GetUsersWithMessages")
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
        GetNewMesage();
        GetMessagesWithUsers();
    })
        .catch(function (err) {
            console.error(err.toString());
        });

});
