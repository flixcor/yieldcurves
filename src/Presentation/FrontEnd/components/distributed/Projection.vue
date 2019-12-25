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
      comp: ''
    }
  },
  mounted () {
    this.comp = this.scriptUrl
    this.connect()
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
      const evtSource = new EventSource(`${Url}?eventTypes=InstrumentCreated&position=${this.position}`)
      evtSource.addEventListener('InstrumentCreated', (e) => {
        if (self.position < e.lastEventId) {
          self.position = e.lastEventId
          self.onEvent(JSON.parse(e.data), 'InstrumentCreated')
        }
      })

      evtSource.addEventListener('error', (e) => {
        if (e.readyState === EventSource.CLOSED) {
          evtSource.close()
          self.connect()
        }
      }, false)
    }
  }
}
</script>
