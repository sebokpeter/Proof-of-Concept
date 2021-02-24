
// Everything here is in one place. This is definitely not ideal.

const homeUri = "home/command?command="

var socket;

var scheme = document.location.protocol === "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";

connectionUrl = scheme + "://" + document.location.hostname + port;

var textArea = document.getElementById("plcData");

function startClicked() {
    const req = new Request(
        homeUri + "start"
    );

    fetch(req).then(c => {
        console.log("Success");
    }).catch(e => {
        console.log("Error: " + e)
    })
}

function resetClicked() {
    const req = new Request(
        homeUri + "reset"
    );

    fetch(req).then(c => {
        console.log("Success");
    }).catch(e => {
        console.log("Error: " + e)
    })
}

function sasClicked() {
    const req = new Request(
        homeUri + "arriving"
    );
    fetch(req).then(c => {
        console.log("Success");
    }).catch(e => {
        console.log("Error: " + e)
    })
}

function slsClicked() {
    const req = new Request(
        homeUri + "leaving"
    );
    fetch(req).then(c => {
        console.log("Success");
    }).catch(e => {
        console.log("Error: " + e)
    })
}

function disconnectClicked() {
    const req = new Request(
        "home/disconnect"
    );
    fetch(req).then(c => {
        console.log("Success");
    }).catch(e => {
        console.log("Error: " + e)
    })
}


function createSocket() {
    console.log("Creating socket")
    socket = new WebSocket(connectionUrl);
    socket.onmessage = function (event) {
        console.log("Got data: " + event.data);
        // textArea.value += event.data + "\n"; // With this we can see each message
        textArea.value = event.data;
    }
    socket.onopen = function (event) {
        console.log("Opened");
    }
    socket.onerror = function (error) {
        console.log("Error: " + error);
    }
    console.log(socket);
}