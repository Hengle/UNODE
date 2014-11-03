var ws = require('websocket.io');
var server = ws.listen(8080,function () {
    console.log("Websocket Server start");
  }
);

var connections = [];
server.on('connection', function(client) {
    console.log('connection start');
 
    // �N���C�A���g����̃��b�Z�[�W��M�C�x���g������
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
 
    // �N���C�A���g���ؒf�����Ƃ��̏���
    client.on('disconnect', function(){
        console.log('connection disconnect');
    });
 
    // �ʐM���N���[�Y�����Ƃ��̏���
    client.on('close', function(){
        console.log('connection close');
    });
 
    // �G���[�����������ꍇ
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