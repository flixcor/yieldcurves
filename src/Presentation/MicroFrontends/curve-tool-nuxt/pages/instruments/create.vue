<template>
  <FrameLiveFeed
    endpoint="https://localhost:5011/features/get-commands"
    @success="$router.push({path: '/instruments'})"
  />
</template>

<script>
import FrameLiveFeed from '../../components/distributed/FrameLiveFeed.vue'

export default {
  components: {
    FrameLiveFeed
  }
}
</script>
