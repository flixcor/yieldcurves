<template>
  <div>
    <FrameLiveFeed
      endpoint="https://localhost:5005/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-on:createClicked="onCreateClicked()"
    />
    <FrameLiveFeed
      endpoint="https://localhost:5007/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-if="detailComponent"
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
      detailComponent: false
    };
  },
  methods: {
    onCreateClicked() {
      this.detailComponent = true;
    }
  }
};
</script>