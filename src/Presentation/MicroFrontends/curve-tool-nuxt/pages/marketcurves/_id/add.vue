<template>
  <FrameLiveFeed
    :endpoint="`https://localhost:5001/features/add-curve-point?marketCurveId=${id}`"
    @success="$router.push({path: `/marketcurves/${id}`})"
  />
</template>

<script>
import FrameLiveFeed from '~/components/distributed/FrameLiveFeed.vue'

export default {
  components: {
    FrameLiveFeed
  },
  props: {
    id: {
      type: String,
      required: true
    }
  }
}
</script>
