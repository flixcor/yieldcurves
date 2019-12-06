<template>
  <v-container>
    <v-row>
      <v-col>
        <FrameLiveFeed
          endpoint="https://localhost:5009/features/get-instruments-overview"
          class="comp md-layout-item md-size-50 md-small-size-100"
          @create="showCreate = !showCreate"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <FrameLiveFeed
          v-if="showCreate"
          endpoint="https://localhost:5011/features/get-commands"
          class="comp md-layout-item md-size-50 md-small-size-100"
          @success="showCreate = false"
        />
      </v-col>
    </v-row>
  </v-container>
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
