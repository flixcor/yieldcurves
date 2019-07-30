<template>
  <div>
    <h5>{{value.command.vendor}} instrument</h5>

    <text-box
      v-model="value.command.name"
      id="nameField"
      label="Name"
      v-on:input="emitChange()"
    />
  </div>
</template>


<script>
import TextBox from './Material/TextBox.vue';

export default {
  data() {
    return {
      datasource: this.value,
    };
  },
  components: {
    TextBox,
  },
  props: ['value'],
  methods: {
    emitChange() {
      this.$emit('input', this.datasource);
    },
  },
};
</script>
