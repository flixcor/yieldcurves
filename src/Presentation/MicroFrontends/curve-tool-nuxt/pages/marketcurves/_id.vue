<template>
  <div>
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
          :endpoint="`https://localhost:5003/features/get-market-curve-detail?id=${id}`"
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
