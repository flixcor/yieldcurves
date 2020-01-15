<template>
  <v-container>
    <h2>Market Curves</h2>
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
          @MarketCurveCreated="overviewState.marketCurves.push($event)"
        >
          <market-curves-overview
            :state="overviewState"
            @detailClicked="$router.push({path: `/marketcurves/${$event}`})"
            @createClicked="$router.push({path: '/marketcurves/create'})"
          />
        </projection>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import Projection from '../components/distributed/Projection.vue'
import MarketCurvesOverview from '../components/market-curves/market-curves-overview.vue'

const EventTypes = ['MarketCurveCreated']

export default {
  name: 'MarketCurves',
  components: {
    Projection,
    MarketCurvesOverview
  },
  async asyncData ({ app }) {
    const initialEvents = await app.preloadEvents(EventTypes)
    return { initialEvents }
  },
  data () {
    return {
      overviewState: {
        marketCurves: []
      },
      eventTypes: EventTypes,
      initialEvents: []
    }
  }
}
</script>
