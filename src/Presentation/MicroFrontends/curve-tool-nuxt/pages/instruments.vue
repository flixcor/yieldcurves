<template>
  <v-layout
    column
    justify-center
    align-center
  >
    <v-flex
      xs12
      sm8
      md6
    >
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
    </v-flex>
  </v-layout>
</template>

<script>
import FrameLiveFeed from '../components/distributed/FrameLiveFeed.vue'

export default {
  name: 'Instruments',
  components: {
    FrameLiveFeed
  },
  data () {
    return {
      showCreate: false
    }
  }
}
</script>
