<template>
  <div>
    <FrameLiveFeed
      endpoint="https://localhost:5003/api"
      class="comp"
      v-on:detailClicked="onDetailClicked($event)"
      v-on:createClicked="onCreateClicked()"
    />
    <FrameLiveFeed
      v-if="id && showDetail"
      :endpoint="`https://localhost:5003/api/${id}`"
      class="comp"
      v-on:createClicked="onDetailCreateClicked()"
    />
    <component
      v-bind:is="createComponent"
      v-if="createComponent"
      :id="id"
      class="comp"
    />
    <component
      v-bind:is="addCurvePointComponent"
      v-if="addCurvePointComponent"
      :id="id"
      class="comp"
    />
  </div>
</template>

<script>
import FrameLiveFeed from './distributed/FrameLiveFeed.vue';
import externalComponent from "../utils/external-component";

export default {
  name: "MarketCurves",
  components: {
    FrameLiveFeed,
    GetMarketCurves: () =>
      externalComponent("https://localhost:5003/get-market-curves.umd.js")
  },
  data() {
    return {
      showCreate: false,
      showDetail: false,
      createComponent: null,
      addCurvePointComponent: null,
      id: null
    };
  },
  methods: {
    onDetailClicked(e) {
      this.id = e;
      this.showDetail = true;
      this.addCurvePointComponent = null;
    },
    onCreateClicked() {
      this.createComponent = () =>
        externalComponent("https://localhost:5001/create-market-curve.umd.js");
      this.id = null;
      this.addCurvePointComponent = null;
    },
    onDetailCreateClicked() {
      this.addCurvePointComponent = () =>
        externalComponent("https://localhost:5001/add-curve-point.umd.js");
    }
  }
};
</script>

