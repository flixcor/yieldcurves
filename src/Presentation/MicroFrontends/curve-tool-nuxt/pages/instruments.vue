<template>
  <v-container>
    <v-row>
      <v-col>
        <FrameLiveFeed
          endpoint="https://localhost:5009/features/get-instruments-overview"
          @create="$router.push({path: '/instruments/create'})"
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
  name: 'Instruments',
  components: {
    FrameLiveFeed
  }
}
</script>
