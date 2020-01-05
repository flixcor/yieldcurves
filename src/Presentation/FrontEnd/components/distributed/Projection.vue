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
    }
  },
  data () {
    return {
      position: 0,
      comp: '',
      connection: null
    }
  },
  mounted () {
    this.connect()
  },
  beforeDestroy () {
    this.disconnect()
  },
  methods: {
    onEvent ({ type, e }) {
      const eventPosition = parseInt(e.lastEventId)
      if (this.position < eventPosition) {
        this.position = eventPosition
        this.$emit(type, JSON.parse(e.data))
      }
    },
    connect (self = this) {
      self.disconnect()

      const typesString = self.eventTypes.join('&eventTypes=')

      self.connection = new EventSource(`${Url}?eventTypes=${typesString}&position=${self.position}`)

      for (const type of self.eventTypes) {
        self.connection.addEventListener(type, e => self.onEvent({ type, e }))
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
