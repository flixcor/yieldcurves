<template>
  <v-container>
    <h2>Calculated curves</h2>
    <v-row>
      <v-col>
        <ct-card>
          <template #title>
            <frame-live-feed
              :endpoint="`${baseUrl}/get-calculation-dates`"
              @input="selectedDate = $event"
            />
            <frame-live-feed
              v-if="selectedDate"
              :endpoint="`${baseUrl}/get-calculations-overview-for-date?asOfDate=${selectedDate}`"
              @change="selectedRecipe = $event"
            />
          </template>
          <template #content>
            <frame-live-feed
              :v-if="selectedDate && selectedRecipe"
              :endpoint="`${baseUrl}/get-calculated-curve-detail?curveRecipeId=${selectedRecipe}&asOfDate=${selectedDate}`"
            />
          </template>
        </ct-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: 'CalculatedCurves',
  data () {
    return {
      baseUrl: 'https://localhost:44393/features',
      selectedDate: this.tMinus1(),
      selectedRecipe: null,
      showCreate: false
    }
  },
  methods: {
    tMinus1 () {
      const someDate = new Date()
      const numberOfDaysToAdd = 1
      someDate.setDate(someDate.getDate() - numberOfDaysToAdd)
      return someDate.toISOString().split('T')[0]
    }
  }
}
</script>
