<template>
  <div>
    <md-card class="comp md-layout-item md-size-50 md-small-size-100">
      <md-card-header>
        <div class="md-title">Calculated curves</div>
        <md-datepicker id="asOfDatePicker" v-model="selectedDate" md-immediately />
        <md-button class="md-primary md-fab md-fab-top-right md-mini" v-on:click="showCreate = !showCreate">
          <md-icon>add</md-icon>
        </md-button>
      </md-card-header>
      <md-card-content>
        <FrameLiveFeed
         :endpoint="`https://localhost:44393/api?curveRecipeId=5C4FEF4E-D475-4CAD-8A84-206D9F6528D1&asOfDate=${jsonAsAtDate}`"
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
  computed: {
    jsonAsAtDate() {
      return this.selectedDate;
    }
  },
  data() {
    return {
      selectedDate: this.tMinus1(),
      showCreate: false
    };
  },
  methods: {
    tMinus1() {
      var someDate = new Date();
      var numberOfDaysToAdd = 1;
      someDate.setDate(someDate.getDate() - numberOfDaysToAdd); 
      return someDate.toISOString().split('T')[0];
    },
  }
};
</script>