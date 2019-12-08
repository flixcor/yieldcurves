<template>
  <FrameLiveFeed
    endpoint="https://localhost:5007/features/create-curve-recipe"
    @success="$router.push({path: `/recipes/${$event}`})"
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
