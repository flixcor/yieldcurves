<template>
  <div>
    <get-market-curves
      v-on:detailClicked="onDetailClicked($event)"
      v-on:createClicked="onCreateClicked()"
      class="comp"
    />
    <component
      v-bind:is="detailComponent"
      v-if="detailComponent"
      :id="id"
      class="comp"
      v-on:createClicked="onDetailCreateClicked()"
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
import externalComponent from "../utils/external-component";

export default {
  name: "MarketCurves",
  components: {
    GetMarketCurves: () =>
      externalComponent("https://localhost:5003/get-market-curves.umd.js")
  },
  data() {
    return {
      showCreate: false,
      detailComponent: null,
      addCurvePointComponent: null,
      id: null
    };
  },
  methods: {
    onDetailClicked(e) {
      this.id = e;
      this.detailComponent = () =>
        externalComponent("https://localhost:5003/get-market-curve.umd.js");
      this.addCurvePointComponent = null;
    },
    onCreateClicked() {
      this.detailComponent = () =>
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

