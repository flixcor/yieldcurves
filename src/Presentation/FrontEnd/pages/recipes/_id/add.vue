<template>
  <frame-live-feed
    :endpoint="`http://localhost:5006/features/add-transformation?recipeId=${id}`"
    @success="$router.push({path: `/recipes/${id}`})"
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
