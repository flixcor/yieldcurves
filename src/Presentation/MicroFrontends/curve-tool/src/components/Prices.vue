<template>
  <div>
<!--     <FrameLiveFeed
      endpoint="https://localhost:5009/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-on:create="showCreate = !showCreate"
    /> -->
    <FrameLiveFeed
      endpoint="https://localhost:5013/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-if="showCreate"
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
  data() {
    return {
      showCreate: true
    };
  }
};
</script>