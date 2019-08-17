<template>
  <div>
    <FrameLiveFeed
      endpoint="https://localhost:5009/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-on:create="showCreate = !showCreate"
    />
    <FrameLiveFeed
      endpoint="https://localhost:5011/api"
      class="comp md-layout-item md-size-50 md-small-size-100"
      v-if="showCreate"
    />
  </div>
</template>

<script>
import FrameLiveFeed from './distributed/FrameLiveFeed.vue';

export default {
  name: "Instruments",
  components: {
    FrameLiveFeed,
  },
  data() {
    return {
      showCreate: false
    };
  }
};
</script>