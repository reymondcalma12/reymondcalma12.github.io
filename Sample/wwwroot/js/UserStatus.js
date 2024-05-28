const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

//connection.start().then(() => {
//    //window.addEventListener('beforeunload', function (e) {
//    //    e.preventDefault();
//    //    e.returnValue = '';

//    //    $.ajax({
//    //        type: 'GET',
//    //        url: "/Account/UserStatusAction",
//    //        dataType: "json",
//    //        success: function (result) {
//    //            if (result != null && result.length > 0) {
            
//    //                connection.invoke('UserOffline', result);
//    //                console.log(result)
//    //            }
//    //            else {
//    //                console.log(result);
//    //            }
//    //        },
//    //        error: function (req, status, error) {
//    //            console.log(status);
//    //        }
//    //    });


//    //});

//});