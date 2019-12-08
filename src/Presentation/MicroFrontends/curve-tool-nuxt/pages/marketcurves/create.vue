<template>
  <FrameLiveFeed
    endpoint="https://localhost:5001/features/create-market-curve"
    @success="$router.push({path: '/marketcurves'})"
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
