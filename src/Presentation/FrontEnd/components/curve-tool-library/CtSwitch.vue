<template>
  <v-switch
    :value="value"
    :label="label"
    :v-on="$listeners"
  />
</template>

<script>
export default {
  props: {
    value: {
      type: Boolean,
      required: true
    },
    label: {
      type: String,
      required: true
    }
  }
}
</script>
