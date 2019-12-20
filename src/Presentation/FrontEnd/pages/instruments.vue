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
          script-url="https://localhost:5009/get-instruments-overview.7589c4f43f7012e78982.umd.js"
          :subscribe="true"
          @create="$router.push({path: '/instruments/create'})"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: 'Instruments',
  components: {
    projection: () => import('../components/distributed/Projection.vue')
  }
}
</script>
