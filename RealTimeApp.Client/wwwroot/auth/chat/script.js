async function start() {
    try {
        await connection.start();
        const connectionId = await connection.invoke("connected");
        console.log(`Connected with ID: ${connectionId}`);

        await connection.invoke("GetTasksForUser", connectionId);

        await fetchDatabaseData();
    } catch (error) {
        console.error(error);
        setTimeout(() => start(), 2000);
    }
}

start();

connection.on("initialTasks", tasks => {
    let taskList = $("#messages ul");
    tasks.forEach(task => {
        taskList.append(`<li>${task.description}</li>`);
    });
});

connection.on("receiveMessage", message => {
    let messageList = $("#messages ul");
    messageList.append(`<li class="new-message">${message}</li>`);
    $("#messages").scrollTop($("#messages")[0].scrollHeight);

    $(".new-message").addClass("highlight");
    setTimeout(() => {
        $(".new-message").removeClass("highlight");
    }, 5000);
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

connection.onreconnected(connectedId => {
    $("#status").css("background-color", "green");
    $("#status").css("color", "white");
    $("#status").html("Connected.");
    animateStatus();

    connection.invoke("GetTasksForUser", connectedId);
});

connection.onclose(connectedId => {
    $("#status").css("background-color", "red");
    $("#status").css("color", "white");
    $("#status").html("Connection failed");
    animateStatus();
});

$("#btnSend").click(() => {
    let message = $("#txtMessage").val().trim();

    if (message) {
        connection.invoke("SendMessageAsync", message)
            .catch(error => console.error(error));

        $("#txtMessage").val("");
    }
});

function animateStatus() {
    $("#status").fadeIn(2000, () => {
        setTimeout(() => {
            $("#status").fadeOut(2000);
        }, 2000);
    });
}

async function fetchDatabaseData() {
    try {
        let response = await fetch('/api/your-database-endpoint', {
            headers: {
                'Authorization': `Bearer ${'@accessToken'}`
            }
        });
        let data = await response.json();
        let dbDataContainer = $("#dbData");
        dbDataContainer.empty();
        data.forEach(item => {
            dbDataContainer.append(`<div class="data-item">${item}</div>`);
        });
    } catch (error) {
        console.error('Error fetching database data:', error);
    }
}
