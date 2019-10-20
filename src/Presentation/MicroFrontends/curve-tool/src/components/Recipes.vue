<template>
  <div>
    <FrameLiveFeed
      endpoint="https://localhost:5005/features/get-curve-recipes-overview"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @createClicked="onCreateClicked()"
      @detailClicked="onDetailClicked($event)"
    />
    <FrameLiveFeed
      v-if="createComponent"
      endpoint="https://localhost:5007/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @success="createComponent = false"
    />
    <FrameLiveFeed
      v-if="detailComponent && id"
      :endpoint="`https://localhost:5005/features/get-curve-recipe-detail?id=${id}`"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @success="detailComponent = false"
      @createClicked="addTransformationComponent = true"
    />
    <FrameLiveFeed
      v-if="addTransformationComponent && id"
      :endpoint="`https://localhost:5007/api/${id}/addtransformation`"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @success="addTransformationComponent = false"
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