<template>
  <div class="md-layout-item">
    <md-field>
      <label :for="id">{{ label }}</label>
      <md-select
        :id="id"
        ref="dropRef"
        v-model="computedValue"
        :name="id"
        @md-selected="$emit('input', internalValue)"
      >
        <md-option
          v-for="opt in computedOptions"
          :key="opt.key"
          :value="opt.value"
        >
          {{ opt.value }}
        </md-option>
      </md-select>
    </md-field>
  </div>
</template>

<script>
export default {
  name: 'MtSelect',
  props: ['label', 'id', 'options', 'value'],
  data () {
    return {
      internalValue: this.value,
    };
  },
  computed: {
    computedOptions () {
      if (this.options.length) {
        if (this.options[0].key) {
          return this.options;
        }
        return this.options.map(x => {
          return { key: x, value: x }
        });
      }
      return [];
    },
    computedValue: {
      get () {
        const option = this.computedOptions.find(x => x.key === this.internalValue);
        if (option) {
          return option.value;
        }
        return null;
      },
      set (newVal) {
        const option = this.computedOptions.find(x => x.value === newVal);
        if (option) {
          this.internalValue = option.key;
        }
      }
    }
  },
};
</script>
