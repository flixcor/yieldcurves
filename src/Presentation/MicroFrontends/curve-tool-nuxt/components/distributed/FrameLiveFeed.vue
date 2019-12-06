<template>
  <dynamic-component
    :component="comp"
    :props="props"
    v-on="$listeners"
  />
</template>

<script>
import getConnection from '../../utils/websockets.js'
import DynamicComponent from './DynamicComponent.vue'

export default {
  name: 'FrameLiveFeed',
  components: { DynamicComponent },
  props: {
    endpoint: {
      required: true,
      type: String
    }
  },
  data () {
    return {
      props: {},
      comp: '',
      hubUrl: null,
      hubConnection: null
    }
  },
  watch: {
    endpoint () {
      this.fetch()
    }
  },
  created () {
    // Fetch initial data.
    this.fetch()
  },
  mounted () {
    this.init()
  },
  destroyed () {
    if (this.hubConnection) { this.hubConnection.stop() }
  },
  methods: {
    async fetch () {
      const data = await this.$axios.$get(this.endpoint)
      let props = data.data

      if (Array.isArray(props)) {
        props = { entities: props }
      }

      this.props = props
      this.comp = data.url
      this.hubUrl = data.hub
    },
    async init () {
      await this.fetch()
      if (this.hubUrl) {
        this.hubConnection = getConnection(this.hubUrl)
        this.hubConnection.on('insert', obj => this.insert(obj))
        this.hubConnection.on('update', obj => this.update(obj))
        await this.hubConnection.start()
      }
    },
    update (obj) {
      const { entities, id } = this.props
      if (Array.isArray(entities)) {
        this.props.entities = entities.map((x) => {
          if (x.id === obj.id) { return obj }
          return x
        })
      } else if (id === obj.id) {
        this.props = obj
      }
    },
    insert (obj) {
      const { entities } = this.props
      if (Array.isArray(entities) && obj.id) {
        if (entities.find(x => x.id === obj.id)) { return }
        this.props.entities = [obj, ...entities]
      }
    }
  }
}
</script>
