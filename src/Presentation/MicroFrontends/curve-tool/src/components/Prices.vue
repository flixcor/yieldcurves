<template>
  <div>
    <md-card class="comp md-layout-item md-size-50 md-small-size-100">
      <md-card-header>
        <div class="md-title">
          Prices
        </div>
        <FrameLiveFeed
          endpoint="https://localhost:44390/features/get-price-dates"
          @change="selectedDate = $event"
        />
        <md-button
          class="md-primary md-fab md-fab-top-right md-mini"
          @click="showCreate = !showCreate"
        >
          <md-icon>add</md-icon>
        </md-button>
      </md-card-header>
      <md-card-content>
        <FrameLiveFeed
          :endpoint="`https://localhost:44390/features/get-prices-overview?asOfDate=${jsonAsAtDate}`"
          @create="showCreate = !showCreate"
        />
      </md-card-content>
    </md-card>
    <FrameLiveFeed
      v-if="showCreate"
      endpoint="https://localhost:5013/features/publish-price"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @success="showCreate = false"
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
  data () {
    return {
      selectedDate: this.tMinus1(),
      showCreate: false
    };
  },
  computed: {
    jsonAsAtDate () {
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