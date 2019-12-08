<template>
  <v-container>
    <v-row>
      <v-col>
        <FrameLiveFeed
          endpoint="https://localhost:5003/features/get-market-curves-overview"
          @detailClicked="$router.push({path: `/marketcurves/${$event}`})"
          @createClicked="$router.push({path: '/marketcurves/create'})"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <nuxt-child />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import FrameLiveFeed from '../components/distributed/FrameLiveFeed.vue'

export default {
  name: 'MarketCurves',
  components: {
    FrameLiveFeed
  }
}
</script>
