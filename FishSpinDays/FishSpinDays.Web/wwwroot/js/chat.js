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
            // Use text() for HTML escaping to prevent XSS
            var userSpan = $('<span>').text(message.user);
            var textSpan = $('<b>').text(message.text);
            var chatDiv = $('<div>').append('[', userSpan, ']: ', textSpan);
            $("#messagesList").append(chatDiv);
        });

    $("#sendButton").on('click', sendMessage);
    $("#messageInput").keypress(function (e) {
        if (e.which === 13) {
            sendMessage();
        }
    });

    function sendMessage() {
        var message = $("#messageInput").val().trim();

        // Basic validation
        if (!message) {
            return;
        }

        // Length limit for chat messages
        if (message.length > 500) {
            alert('Message too long! Maximum 500 characters.');
            return;
        }

        connection.invoke("Send", message);
        $("#messageInput").val("");
    }

    connection.start().catch(function (err) {
        console.error('SignalR connection error:', err.toString());
        $('#messagesList').append('<div style="color: red;"><em>Connection error. Please refresh the page.</em></div>');
    });
});