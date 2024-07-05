$(document).ready(() => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("https://localhost:7071/hubaction")
        .withAutomaticReconnect([1000, 1000, 2000])
        .build();

    async function start() {
        try {
            await connection.start();
            const connectionId = await connection.invoke("connected");
            console.log(`Connected with ID: ${connectionId}`);

            // Fetch tasks for the logged-in user
            await connection.invoke("GetTasksForUser", connectionId);
        } catch (error) {
            console.error(error);
            setTimeout(() => start(), 2000);
        }
    }

    start();

    // Receive tasks for the logged-in user
    connection.on("tasksForUser", tasks => {
        let taskList = $("#userTasks");
        taskList.empty();
        tasks.forEach(task => {
            taskList.append(`<li>${task.description} - ${task.createDate}</li>`);
        });
    });

    // Other message sending and receiving logic
    $("#btnSend").click(() => {
        let message = $("#txtMessage").val();
        connection.invoke("SendMessageAsync", message).catch(error => console.error(error));
        $("#txtMessage").val("");
    });

    connection.on("receiveMessage", message => {
        $("#messages ul").append(`<li>${message}</li>`);
        $("#messages").scrollTop($("#messages")[0].scrollHeight);
    });

    connection.on("userJoined", connectedId => {
        $("#status").html(`${connectedId} connected`);
        $("#status").css("background-color", "green");
        animateStatus();
    });

    connection.on("userLeaved", connectedId => {
        $("#status").html(`${connectedId} left`);
        $("#status").css("background-color", "red");
        animateStatus();
    });

    connection.onreconnecting(error => {
        $("#status").css("background-color", "blue");
        $("#status").css("color", "white");
        $("#status").html("Connecting...");
        animateStatus();
    });

    connection.onreconnected(connectedId => {
        $("#status").css("background-color", "green");
        $("#status").css("color", "white");
        $("#status").html("Connected.");
        animateStatus();
    });

    connection.onclose(connectedId => {
        $("#status").css("background-color", "red");
        $("#status").css("color", "white");
        $("#status").html("Connection failed");
        animateStatus();
    });

    function animateStatus() {
        $("#status").fadeIn(2000, () => {
            setTimeout(() => {
                $("#status").fadeOut(2000);
            }, 2000);
        });
    }
});
