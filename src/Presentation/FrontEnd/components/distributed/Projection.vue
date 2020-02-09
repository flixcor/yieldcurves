<script>
const Url = 'https://localhost:44395'

export default {
  props: {
    subscribe: {
      type: Boolean,
      default: true
    },
    eventTypes: {
      type: Array,
      required: true
    },
    initialEvents: {
      type: Array,
      default: () => []
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
    this.initialEvents.forEach(({ type, position, payload }) => {
      this.onEvent({ type, position, payload })
    })
  },
  mounted () {
    this.connect()
  },
  beforeDestroy () {
    this.disconnect()
  },
  methods: {
    onEvent ({ type, position, payload }) {
      if (this.position < position) {
        this.position = position
        this.$emit(type, payload)
      }
    },
    connect (self = this) {
      self.disconnect()

      const typesString = self.eventTypes.join('&eventTypes=')

      self.connection = new EventSource(`${Url}/subscribe?eventTypes=${typesString}&checkpoint=${self.position}`)

      for (const type of self.eventTypes) {
        const onEvent = e => self.onEvent({ type, position: parseInt(e.lastEventId), payload: JSON.parse(e.data) })
        self.connection.addEventListener(type, onEvent)
      }

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
  },
  render () {
    return this.$scopedSlots.default({})
  }
}
</script>
