<template>
  <v-btn
    :fab="fab"
    v-on="$listeners"
  >
    <slot />
  </v-btn>
</template>

<script>
export default {
  props: {
    fab: {
      type: Boolean,
      default: false
    }
  }
}
</script>
