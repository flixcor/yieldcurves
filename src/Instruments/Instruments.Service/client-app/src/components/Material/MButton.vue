<template>
  <md-button v-on:click="this.onclick" class="md-raised md-primary">{{label}}</md-button>
</template>

<script>

export default {
  name: 'm-button',
  props: ['label', 'onclick'],
};
</script>
