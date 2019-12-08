<template>
  <v-container>
    <h2>Recipes</h2>
    <v-row>
      <v-col>
        <nuxt-child />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <frame-live-feed
          endpoint="https://localhost:5005/features/get-curve-recipes-overview"
          @detailClicked="$router.push({path: `/recipes/${$event}`})"
          @createClicked="$router.push({path: '/recipes/create'})"
        />
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: 'Recipes'
}
</script>
