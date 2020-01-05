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
          :endpoint="`http://localhost:5004/features/get-curve-recipe-detail?id=${id}`"
          @createClicked="$router.push({path: `/recipes/${id}/add`})"
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
