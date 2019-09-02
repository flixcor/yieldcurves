<template>
  <div>
    <md-card class="comp md-layout-item md-size-50 md-small-size-100">
      <md-card-header>
        <div class="md-title">Prices</div>
        <md-datepicker id="asOfDatePicker" v-model="selectedDate" md-immediately />
        <md-button class="md-primary md-fab md-fab-top-right md-mini" v-on:click="showCreate = !showCreate">
          <md-icon>add</md-icon>
        </md-button>
      </md-card-header>
      <md-card-content>
        <FrameLiveFeed
         :endpoint="`https://localhost:44390/api?asOfDate=${jsonAsAtDate}`"
         v-on:create="showCreate = !showCreate"
        />
      </md-card-content>
    </md-card>
    <FrameLiveFeed
      endpoint="https://localhost:5013/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-if="showCreate"
      v-on:success="showCreate = false"
    />
  </div>
</template>

<script>
import FrameLiveFeed from './distributed/FrameLiveFeed.vue';

export default {
  name: "Prices",
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