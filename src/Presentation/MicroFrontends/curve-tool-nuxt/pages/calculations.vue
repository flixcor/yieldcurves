<template>
  <v-container>
    <h2>Calculated curves</h2>
    <v-row>
      <v-col>
        <ct-card>
          <template #title>
            <FrameLiveFeed
              endpoint="https://localhost:44393/features/get-calculation-dates"
              @change="selectedDate = $event"
            />
            <FrameLiveFeed
              v-if="selectedDate"
              :endpoint="`https://localhost:44393/features/get-calculations-overview-for-date?asOfDate=${jsonAsOfDate}`"
              @change="selectedRecipe = $event"
            />
          </template>
          <template #content>
            <FrameLiveFeed
              :v-if="selectedDate && selectedRecipe"
              :endpoint="`https://localhost:44393/features/get-calculated-curve-detail?curveRecipeId=${selectedRecipe}&asOfDate=${jsonAsOfDate}`"
            />
          </template>
        </ct-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: "CalculatedCurves",
  data () {
    return {
      selectedDate: this.tMinus1(),
      selectedRecipe: null,
      showCreate: false
    };
  },
  computed: {
    jsonAsOfDate () {
      return this.selectedDate;
    }
  },
  methods: {
    tMinus1 () {
      var someDate = new Date();
      var numberOfDaysToAdd = 1;
      someDate.setDate(someDate.getDate() - numberOfDaysToAdd);
      return someDate.toISOString().split('T')[0];
    },
  }
};
</script>
