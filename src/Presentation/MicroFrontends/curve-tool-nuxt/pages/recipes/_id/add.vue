<template>
  <frame-live-feed
    :endpoint="`https://localhost:5007/features/add-transformation?recipeId=${id}`"
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
