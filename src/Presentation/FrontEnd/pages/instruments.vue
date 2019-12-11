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
        <frame-live-feed
          endpoint="https://localhost:5009/features/get-instruments-overview"
          @create="$router.push({path: '/instruments/create'})"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: 'Instruments'
}
</script>
