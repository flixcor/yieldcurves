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
        <frame-live-feed
          endpoint="https://localhost:5003/features/get-market-curves-overview"
          @detailClicked="$router.push({path: `/marketcurves/${$event}`})"
          @createClicked="$router.push({path: '/marketcurves/create'})"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: 'MarketCurves'
}
</script>
