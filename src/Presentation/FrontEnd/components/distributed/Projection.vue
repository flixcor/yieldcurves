<template>
  <dynamic-component
    ref="componentRef"
    :component="comp"
    v-on="$listeners"
  />
</template>

<script>

import { EventRequest } from '../../utils/greet_grpc_web_pb.js'
import models, { EventStoreClient, InstrumentCreated, MarketCurveCreated } from '../../utils/greet_pb.js'
import DynamicComponent from './DynamicComponent.vue'

const Url = 'http://localhost:5123'

const instrumentCreated = 'InstrumentCreated'
const marketCurveCreated = 'MarketCurveCreated'

const buildRequest = ({ eventTypes, commitPosition, preparePosition, subscribe }) => {
  const request = new EventRequest()
  request.setPreparePosition(commitPosition)
  request.setCommitPosition(preparePosition)
  request.setSubscribe(subscribe)

  eventTypes.forEach((eventType) => {
    request.addEventTypes(eventType)
  })

  return request
}

const getEventPayload = (eventReply) => {
  const payload = eventReply.getPayload()
  const typeName = eventReply.getEventType()

  const pascalName = typeName[0].toUpperCase() + typeName.substr(1)

  const type = models[pascalName]

  return type.deserializeBinary(payload)
}

const connect = (params, action) => {
  const client = getClient()
  console.log(params)
  const request = buildRequest(params)

  console.log(request.toObject())

  const stream = client.getEvents(request, {})
  stream.on('data', (response) => {
    const payload = getEventPayload(response).toObject()
    const type = response.getEventType()

    action({
      payload,
      type
    })
  })
}

const getClient = () => new EventStoreClient(Url)

export default {
  components: {
    DynamicComponent
  },
  props: {
    subscribe: {
      type: Boolean,
      default: true
    },
    eventTypes: {
      type: Array,
      required: true
    },
    scriptUrl: {
      type: String,
      required: true
    }
  },
  data () {
    return {
      preparePosition: 0,
      commitPosition: 0,
      comp: ''
    }
  },
  mounted () {
    const func = this.onEvent
    this.comp = this.scriptUrl
    const client = new EventStoreClient('http://localhost:5123')

    const request = new EventRequest()
    request.setPreparePosition(0)
    request.setCommitPosition(0)
    request.setSubscribe(true)
    request.addEventTypes(instrumentCreated)
    request.addEventTypes(marketCurveCreated)

    const stream = client.getEvents(request, {})

    stream.on('data', (response) => {
      console.log(response.toObject())
      const payload = response.getPayload()
      const type = response.getEventType()

      let deserializer

      console.log(type)

      switch (type) {
        case instrumentCreated:
          deserializer = InstrumentCreated.deserializeBinary
          break
        case marketCurveCreated:
          deserializer = MarketCurveCreated.deserializeBinary
          break
        default:
          throw Error
      }

      const event = deserializer(payload).toObject()

      func({ type, event })
    })
  },
  methods: {
    connect () {
      const request = {
        eventTypes: this.eventTypes,
        preparePosition: this.preparePosition,
        commitPosition: this.commitPosition,
        subscribe: true
      }

      connect(request, this.onEvent)
    },
    onEvent ({ event, type }) {
      const camel = type[0].toLowerCase() + type.substr(1)
      const ref = this.$refs.componentRef

      if (typeof ref.passAlong === 'function') {
        ref.passAlong(camel, event)
      }
    }
  }
}
</script>
