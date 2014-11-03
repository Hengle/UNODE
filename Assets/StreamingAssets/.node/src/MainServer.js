var ws = require('websocket.io');
var server = ws.listen(8080,function () {
    console.log("Websocket Server start");
  }
);

var connections = [];
server.on('connection', function(client) {
    console.log('connection start');
 
    // クライアントからのメッセージ受信イベントを処理
    client.on('message', function(request) {
        var msg = JSON.parse(request);
        console.log("From Unity:"+request + "MODE:"+msg.mode);
        switch (msg.mode) {
            case 1:
                var text = "{\"mode\":1,"+"\"ver\":\"v0.10.28\"}";
                console.log("Nodejs -> Unity:"+text);
                client.send(text);
                break;
            case 2:
                var text = "{\"mode\":2}";
                console.log("Nodejs -> Unity:"+text);
                client.send(text)
                break;
        }
        
    });
 
    // クライアントが切断したときの処理
    client.on('disconnect', function(){
        console.log('connection disconnect');
    });
 
    // 通信がクローズしたときの処理
    client.on('close', function(){
        console.log('connection close');
    });
 
    // エラーが発生した場合
    client.on('error', function(err){
        console.log(err);
        console.log(err.stack);
    });
});


/*
server.on("connection",function(socket) {
    console.log("message " + ":(" +socket.remoteAddress +' : '+ socket.remotePort+")");
    socket.send("Connect!");
    connections.push(socket);
    socket.on("message",function(data) {
        console.log("message " + data);
        socket.send(data);
      }
    );
  }
);

function broadcast(message,socket) {
    connections.forEach(function (con, i) {
        if (con != socket) {
            con.send(message);
        }
    });
};
*/