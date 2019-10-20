<template>
  <md-button
    class="md-raised md-primary"
    @click="onclick"
  >
    {{ label }}
  </md-button>
</template>

<script>

export default {
  name: 'MButton',
  props: ['label', 'onclick'],
};
</script>
