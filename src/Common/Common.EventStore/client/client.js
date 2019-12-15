import { EventRequest } from './greet_grpc_web_pb.js'
import { EventStoreClient, InstrumentCreated, MarketCurveCreated } from './greet_pb.js'

const instrumentCreated = 'InstrumentCreated'
const marketCurveCreated = 'MarketCurveCreated'




var client = new EventStoreClient('http://localhost:5123')

var request = new EventRequest()
request.setPreparePosition(0)
request.setCommitPosition(0)
request.setSubscribe(false)
request.addEventTypes(instrumentCreated)
request.addEventTypes(marketCurveCreated)

const stream = client.getEvents(request, {})
stream.on('data', response => {
  console.log(response.toObject())
  const payload = response.getPayload()
  const typeName = response.getEventType()

  var deserializer

  console.log(typeName)

  switch (typeName) {
    case instrumentCreated:
      deserializer = InstrumentCreated.deserializeBinary
      break
    case marketCurveCreated:
      deserializer = MarketCurveCreated.deserializeBinary
      break
    default:
      throw 'raar'
  }

  const ding = deserializer(payload)
  console.log(ding.toObject())
})
