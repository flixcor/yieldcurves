<template>
  <div>
    <FrameLiveFeed
      endpoint="https://localhost:5003/features/get-market-curves-overview"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @detailClicked="onDetailClicked($event)"
      @createClicked="onCreateClicked()"
    />
    <FrameLiveFeed
      v-if="id && showDetail"
      :endpoint="`https://localhost:5003/features/get-market-curve-detail?id=${id}`"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @createClicked="onDetailCreateClicked()"
      @success="showDetail = false"
    />
    <FrameLiveFeed
      v-if="showCreate"
      endpoint="https://localhost:5001/features/create-market-curve"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @success="showCreate = false"
    />
    <FrameLiveFeed
      v-if="id && showAdd"
      :endpoint="`https://localhost:5001/features/add-curve-point?marketCurveId=${id}`"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @success="showAdd = false"
    />
  </div>
</template>

<script>
import FrameLiveFeed from './distributed/FrameLiveFeed.vue';

export default {
  name: "MarketCurves",
  components: {
    FrameLiveFeed,
  },
  data() {
    return {
      showCreate: false,
      showDetail: false,
      showAdd: false,
      id: null
    };
  },
  methods: {
    onDetailClicked(e) {
      this.showCreate = false;
      this.showAdd = false;
      this.id = e;
      this.showDetail = true;
    },
    onCreateClicked() {
      this.showAdd = false;
      this.showDetail = false;
      this.showCreate = true;
    },
    onDetailCreateClicked() {
      this.showCreate = false;
      this.showAdd = true;
    }
  }
};
</script>

