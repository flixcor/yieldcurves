<template>
  <dynamic-component
    ref="componentRef"
    :component="comp"
    v-on="$listeners"
  />
</template>

<script>

import DynamicComponent from './DynamicComponent.vue'

const Url = 'https://localhost:44395'

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
      position: 0,
      comp: '',
      connection: null
    }
  },
  created () {
    this.comp = this.scriptUrl
  },
  mounted () {
    this.connect()
  },
  beforeDestroy () {
    this.disconnect()
  },
  methods: {
    onEvent (event, type) {
      const camel = type[0].toLowerCase() + type.substr(1)
      const ref = this.$refs.componentRef

      if (typeof ref.passAlong === 'function') {
        ref.passAlong(camel, event)
      }
    },
    connect (self = this) {
      self.disconnect()

      self.connection = new EventSource(`${Url}?eventTypes=InstrumentCreated&position=${self.position}`)
      self.connection.addEventListener('InstrumentCreated', (e) => {
        const eventPosition = parseInt(e.lastEventId)
        if (self.position < eventPosition) {
          self.position = eventPosition
          self.onEvent(JSON.parse(e.data), 'InstrumentCreated')
        }
      })

      self.connection.addEventListener('error', (e) => {
        if (e.readyState === EventSource.CLOSED) {
          self.connect()
        }
      }, false)
    },
    disconnect () {
      if (this.connection != null) {
        this.connection.close()
        this.connection = null
      }
    }
  }
}
</script>
