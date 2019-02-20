$(function () {
    var connection =
        new signalR.HubConnectionBuilder()
            .withUrl("/chat")
            .build();
    //var connection = new signalR.HubConnectionBuilder()
    //    .withUrl("/chat", { transport: signalR.HttpTransportType.LongPolling })
    //    .build();

    connection.on("NewMessage",
        function (message) {
            var chatInfo = `<div>[${message.user}]: <b>${message.text}</b></div>`;
            $("#messagesList").append(chatInfo);
        });

    $("#sendButton").on('click', sendMessage);
    $("#messageInput").keypress(function (e) {
        if (e.which === 13) {
            sendMessage();
        }
    });

    function sendMessage() {
        var message = $("#messageInput").val();
        connection.invoke("Send", message);
        $("#messageInput").val("");
    }

    connection.start().catch(function (err) {
        return console.error(err.toString());
    });
});