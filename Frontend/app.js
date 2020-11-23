window.apiBaseUrl = 'http://localhost:7071/api';


console.log('blah');

var messages = document.getElementById("messages");


getConnectionInfo().then(info => {
    const options = {
        accessTokenFactory: () => info.accessToken
    };
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(info.url, options)
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on('newMessage', newMessage);
    connection.onclose(() => console.log('disconnected'));
    console.log('connecting...');
    connection.start()
        .then(() => console.log('connected!'))
        .then(() => {
            window.connection = connection;
        })
        .catch(console.error);

}).catch(console.error);

function newMessage(msg) {
    console.log('message', msg);

    var msgEl = document.createElement("li");
    msgEl.innerText = msg;

    messages.appendChild(msgEl)
}

function getAxiosConfig() {
    const config = {};
    return config;
}
function getConnectionInfo() {
    return axios.post(`${apiBaseUrl}/negotiate`, null, getAxiosConfig())
        .then(resp => resp.data);
}