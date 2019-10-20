<template>
  <div>
    <FrameLiveFeed
      endpoint="https://localhost:5009/features/get-instruments-overview"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @create="showCreate = !showCreate"
    />
    <FrameLiveFeed
      v-if="showCreate"
      endpoint="https://localhost:5011/features/get-commands"
      class="comp md-layout-item md-size-50 md-small-size-100"
      @success="showCreate = false"
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