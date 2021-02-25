
// Everything here is in one place. This is definitely not ideal.


//const homeUri = "home/command?command="
//const homeUri = "http://localhost/home/command"
const homeUrl = window.location.href + "home/command";

const disconnectUrl = "home/disconnect"

var socket;

var scheme = document.location.protocol === "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";

connectionUrl = scheme + "://" + document.location.hostname + port;

var textArea = document.getElementById("plcData");

window.onload = createSocket();

function startClicked() {
    sendCommandToController("start");
}

function resetClicked() {
    sendCommandToController("reset");
}

function sasClicked() {
    sendCommandToController("arriving");
}

function slsClicked() {
    sendCommandToController("leaving");
}

function disconnectClicked() {
    sendDisconnectRequest();
}

function sendCommandToController(command) {
    const params = new URLSearchParams();
    params.append("command", command);

    const url = new URL(homeUrl);
    url.search = params;

    const req = new Request(url);

    console.log(params.toString());
    console.log(req);

    fetch(req).then(c => {
        console.log("Success");
    }).catch(e => {
        console.error(e);
    });
}

function sendDisconnectRequest() {
    const req = new Request(disconnectUrl);

    fetch(req).then(r => {
        console.log("Disconnected");
    }).error(e => {
        console.log(e);
    });
}

function createSocket() {
    console.log("Creating socket")
    socket = new WebSocket(connectionUrl);
    socket.onmessage = function (event) {
        //console.log("Got data: " + event.data);
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