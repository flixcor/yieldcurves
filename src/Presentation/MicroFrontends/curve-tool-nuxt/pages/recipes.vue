<template>
  <v-container>
    <v-row>
      <v-col>
        <FrameLiveFeed
          endpoint="https://localhost:5005/features/get-curve-recipes-overview"
          @detailClicked="$router.push({path: `/recipes/${$event}`})"
          @createClicked="$router.push({path: '/recipes/create'})"
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
  name: 'Recipes',
  components: {
    FrameLiveFeed
  }
}
</script>
