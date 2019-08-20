<template>
  <div>
    <FrameLiveFeed
      endpoint="https://localhost:5003/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-on:detailClicked="onDetailClicked($event)"
      v-on:createClicked="onCreateClicked()"
    />
    <FrameLiveFeed
      v-if="id && showDetail"
      :endpoint="`https://localhost:5003/api/${id}`"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-on:createClicked="onDetailCreateClicked()"
      v-on:success="showDetail = false"
    />
    <FrameLiveFeed
      v-if="showCreate"
      endpoint="https://localhost:5001/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-on:success="showCreate = false"
    />
    <FrameLiveFeed
      v-if="id && showAdd"
      :endpoint="`https://localhost:5001/api/curvepoint/${id}`"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-on:success="showAdd = false"
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

