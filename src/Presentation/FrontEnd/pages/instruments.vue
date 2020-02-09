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
          <instruments-overview :state="overviewState" @create="$router.push({path: '/instruments/create'})" />
        </projection>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import InstrumentsOverview from '../components/instruments/instruments-overview.vue'
import Projection from '../components/distributed/Projection.vue'

const EventTypes = ['InstrumentCreated']

export default {
  name: 'Instruments',
  components: {
    Projection,
    InstrumentsOverview
  },
  async asyncData ({ app }) {
    const initialEvents = await app.preloadEvents(EventTypes)
    return { initialEvents }
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
