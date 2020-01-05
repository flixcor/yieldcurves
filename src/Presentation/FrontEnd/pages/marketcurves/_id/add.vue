<template>
  <frame-live-feed
    :endpoint="`http://localhost:5000/features/add-curve-point?marketCurveId=${id}`"
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
