/**
 * @fileoverview gRPC-Web generated client stub for events
 * @enhanceable
 * @public
 */

// GENERATED CODE -- DO NOT EDIT!



const grpc = {};
grpc.web = require('grpc-web');

const proto = {};
proto.events = require('./greet_pb.js');

/**
 * @param {string} hostname
 * @param {?Object} credentials
 * @param {?Object} options
 * @constructor
 * @struct
 * @final
 */
proto.events.EventStoreClient =
    function(hostname, credentials, options) {
  if (!options) options = {};
  options['format'] = 'text';

  /**
   * @private @const {!grpc.web.GrpcWebClientBase} The client
   */
  this.client_ = new grpc.web.GrpcWebClientBase(options);

  /**
   * @private @const {string} The hostname
   */
  this.hostname_ = hostname;

};


/**
 * @param {string} hostname
 * @param {?Object} credentials
 * @param {?Object} options
 * @constructor
 * @struct
 * @final
 */
proto.events.EventStorePromiseClient =
    function(hostname, credentials, options) {
  if (!options) options = {};
  options['format'] = 'text';

  /**
   * @private @const {!grpc.web.GrpcWebClientBase} The client
   */
  this.client_ = new grpc.web.GrpcWebClientBase(options);

  /**
   * @private @const {string} The hostname
   */
  this.hostname_ = hostname;

};


/**
 * @const
 * @type {!grpc.web.MethodDescriptor<
 *   !proto.events.EventRequest,
 *   !proto.events.EventReply>}
 */
const methodDescriptor_EventStore_GetEvents = new grpc.web.MethodDescriptor(
  '/events.EventStore/GetEvents',
  grpc.web.MethodType.SERVER_STREAMING,
  proto.events.EventRequest,
  proto.events.EventReply,
  /**
   * @param {!proto.events.EventRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.events.EventReply.deserializeBinary
);


/**
 * @const
 * @type {!grpc.web.AbstractClientBase.MethodInfo<
 *   !proto.events.EventRequest,
 *   !proto.events.EventReply>}
 */
const methodInfo_EventStore_GetEvents = new grpc.web.AbstractClientBase.MethodInfo(
  proto.events.EventReply,
  /**
   * @param {!proto.events.EventRequest} request
   * @return {!Uint8Array}
   */
  function(request) {
    return request.serializeBinary();
  },
  proto.events.EventReply.deserializeBinary
);


/**
 * @param {!proto.events.EventRequest} request The request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @return {!grpc.web.ClientReadableStream<!proto.events.EventReply>}
 *     The XHR Node Readable Stream
 */
proto.events.EventStoreClient.prototype.getEvents =
    function(request, metadata) {
  return this.client_.serverStreaming(this.hostname_ +
      '/events.EventStore/GetEvents',
      request,
      metadata || {},
      methodDescriptor_EventStore_GetEvents);
};


/**
 * @param {!proto.events.EventRequest} request The request proto
 * @param {?Object<string, string>} metadata User defined
 *     call metadata
 * @return {!grpc.web.ClientReadableStream<!proto.events.EventReply>}
 *     The XHR Node Readable Stream
 */
proto.events.EventStorePromiseClient.prototype.getEvents =
    function(request, metadata) {
  return this.client_.serverStreaming(this.hostname_ +
      '/events.EventStore/GetEvents',
      request,
      metadata || {},
      methodDescriptor_EventStore_GetEvents);
};


module.exports = proto.events;

