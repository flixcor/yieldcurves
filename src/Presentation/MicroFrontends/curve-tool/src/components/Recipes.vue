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
      id: false,
    };
  },
  methods: {
    onCreateClicked() {
      this.id = false;
      this.createComponent = true;
      this.detailComponent = false;
    },
    onDetailClicked(id) {
      this.id = id;
      this.createComponent = false;
      this.detailComponent = true;
    },
  },
};
</script>