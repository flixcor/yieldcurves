import {HelloRequest, HelloReply} from './greet_grpc_web_pb.js';
import {GreeterClient} from './greet_pb.js';

var client = new GreeterClient('http://localhost:5123');

var request = new HelloRequest();
request.setName('World');

const stream = client.sayHello(request, {});
stream.on('data', response => {
  console.log(response.getMessage());
})
