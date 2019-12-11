<template>
  <frame-live-feed
    :endpoint="`https://localhost:5001/features/add-curve-point?marketCurveId=${id}`"
    @success="$router.push({path: `/marketcurves/${id}`})"
  />
</template>

<script>
export default {
  props: {
    id: {
      type: String,
      required: true
    }
  }
}
</script>
