<template>
  <v-container>
    <h2>Instruments</h2>
    <v-row>
      <v-col>
        <nuxt-child />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <projection
          :event-types="eventTypes"
          :initial-events="initialEvents"
          @InstrumentCreated="overviewState.instruments.push($event)"
        >
          <instruments-overview :state="overviewState" />
        </projection>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import InstrumentsOverview from '../components/InstrumentsOverview.vue'
import Projection from '../components/distributed/Projection.vue'

const EventTypes = ['InstrumentCreated']

export default {
  name: 'Instruments',
  components: {
    Projection,
    InstrumentsOverview
  },
  async asyncData ({ $axios }) {
    const typesString = EventTypes
      ? '?eventTypes=' + EventTypes.join('&eventTypes=')
      : ''

    const url = 'http://localhost:65072' + typesString

    const { data } = await $axios.get(url)
    return { initialEvents: data, typesString }
  },
  data: () => ({
    overviewState: {
      instruments: []
    },
    eventTypes: EventTypes,
    initialEvents: []
  })
}
</script>
