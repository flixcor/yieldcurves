<template>
  <div v-if="id">
    <v-row>
      <v-col>
        <nuxt-child
          :id="id"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <frame-live-feed
          :endpoint="`http://localhost:5002/features/get-market-curve-detail?id=${id}`"
          @createClicked="$router.push({path: `/marketcurves/${id}/add`})"
        />
      </v-col>
    </v-row>
  </div>
</template>

<script>
export default {
  asyncData ({ params }) {
    return {
      id: params.id
    }
  }
}
</script>
