<template>
  <div>
    <FrameLiveFeed
      endpoint="https://localhost:5005/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-on:createClicked="onCreateClicked()"
      v-on:detailClicked="onDetailClicked($event)"
    />
    <FrameLiveFeed
      endpoint="https://localhost:5007/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-if="createComponent"
      v-on:success="createComponent = false"
    />
    <FrameLiveFeed
      :endpoint="`https://localhost:5005/api/${id}`"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-if="detailComponent && id"
      v-on:success="detailComponent = false"
      v-on:createClicked="addTransformationComponent = true"
    />
    <FrameLiveFeed
      :endpoint="`https://localhost:5007/api/${id}/addtransformation`"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-if="addTransformationComponent && id"
      v-on:success="addTransformationComponent = false"
    />
  </div>
</template>

<script>
import FrameLiveFeed from './distributed/FrameLiveFeed.vue';

export default {
  name: "Recipes",
  components: {
    FrameLiveFeed,
  },
  data() {
    return {
      createComponent: false,
      detailComponent: false,
      addTransformationComponent: false,
      id: false,
    };
  },
  methods: {
    onCreateClicked() {
      this.id = false;
      this.detailComponent = false;
      this.addTransformationComponent = false;
      this.createComponent = true;
    },
    onDetailClicked(id) {
      this.id = id;
      this.createComponent = false;
      this.addTransformationComponent = false;
      this.detailComponent = true;
    },
  },
};
</script>