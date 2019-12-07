<template>
  <v-card class="md-layout-item md-size-50 md-small-size-100">
    <v-card-title>
      <slot name="title" />
    </v-card-title>
    <v-card-text>
      <slot name="content" />
    </v-card-text>
    <v-card-actions>
      <slot name="actions" />
    </v-card-actions>
  </v-card>
</template>

<script>
export default {

}
</script>

<style>

</style>
