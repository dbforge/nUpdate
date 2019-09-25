var rpc = require('node-json-rpc');
var p = require("path");
const fs = require('fs');

var options = {
    // int port of rpc server, default 5080 for http or 5433 for https
    port: 5080,
    // string domain name or ip of rpc server, default '127.0.0.1'
    host: '127.0.0.1',
    // string with default path, default '/'
    path: '/',
    // boolean false to turn rpc checks off, default true
    strict: true
};
 
// Create a server object with options
var server = new rpc.Server(options);
 
server.addMethod('DeleteFile', function (para, callback) {
    var error, result;
  
    // Add 2 or more parameters together
    if (para.length === 1) {
        fs.unlink(para[0], (err) => {
            error = { code: -32603, message: err.message }
        })
    } else {
        error = { code: -32602, message: "Invalid params" };
    }
 
    callback(error, result);
});

var deleteFolderRecursive = function(path) {
    if (fs.existsSync(path)) {
        fs.readdirSync(path).forEach(function(file) {
            var curPath = p.join(path, file);
            if (fs.lstatSync(curPath).isDirectory()) { // recurse
                deleteFolderRecursive(curPath);
            } else { // delete file
                fs.unlinkSync(curPath);
            }
        });
        fs.rmdirSync(path);
    }
}

server.addMethod('DeleteDirectory', function (para, callback) {
    var error, result;
  
    // Add 2 or more parameters together
    if (para.length === 1) {
        try {
            deleteFolderRecursive(para[0]);
        } catch (err) {
            error = { code: -32603, message: err.message }
        }
    } else {
        error = { code: -32602, message: "Invalid params" };
    }
 
    callback(error, result);
});

// TODO: Add other methods
 
// Start the server
server.start(function (error) {
    // Did server start succeed ?
    if (error) throw error;
    else console.log('Server running ...');
});