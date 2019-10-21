<template>
  <div>
    <md-card class="comp md-layout-item md-size-50 md-small-size-100">
      <md-card-header>
        <div class="md-title">
          Calculated curves
        </div>
        <FrameLiveFeed
          endpoint="https://localhost:44393/features/get-calculation-dates"
          @change="selectedDate = $event"
        />
        <FrameLiveFeed
          v-if="selectedDate"
          :endpoint="`https://localhost:44393/features/get-calculations-overview-for-date?asOfDate=${jsonAsOfDate}`"
          @change="selectedRecipe = $event"
        />
      </md-card-header>
      <md-card-content>
        <FrameLiveFeed
          :v-if="selectedDate && selectedRecipe"
          :endpoint="`https://localhost:44393/features/get-calculated-curves-overview?curveRecipeId=${selectedRecipe}&asOfDate=${jsonAsOfDate}`"
        />
      </md-card-content>
    </md-card>
  </div>
</template>

<script>
import FrameLiveFeed from './distributed/FrameLiveFeed.vue';

export default {
  name: "CalculatedCurves",
  components: {
    FrameLiveFeed,
  },
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