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
          :event-types="['InstrumentCreated']"
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

export default {
  name: 'Instruments',
  components: {
    Projection,
    InstrumentsOverview
  },
  data: () => ({
    overviewState: {
      instruments: []
    }
  })
}
</script>
