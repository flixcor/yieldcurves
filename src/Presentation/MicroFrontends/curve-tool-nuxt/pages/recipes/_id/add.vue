<template>
  <FrameLiveFeed
    :endpoint="`https://localhost:5007/features/add-transformation?recipeId=${id}`"
    @success="$router.push({path: `/recipes/${id}`})"
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
