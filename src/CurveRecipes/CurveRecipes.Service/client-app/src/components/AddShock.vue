<template>
  <div>
      <text-box
        type="number"
        label="Order"
        id="orderBox"
        v-model="datasource.order"
      />
      <mt-select
        id="shockTargetDropdown"
        v-model="datasource.shockTarget"
        label="Shock target"
        :options="shockTargets"
      />
      <text-box
        type="number"
        label="Shift"
        id="shiftBox"
        v-model="datasource.shift"
      />
      <text-box
        label="Maturities"
        id="maturitiesBox"
        v-model="datasource.maturities"
        v-if="datasource.maturities"
      />
  </div>
</template>

<script>
import MtSelect from './Material/MtSelect.vue';
import TextBox from './Material/TextBox.vue';

export default {
  components: {
    MtSelect,
    TextBox,
  },
  props: ['value', 'shockTargets'],
  data() {
    return {
      datasource: this.value,
    };
  },
  methods: {
    emitChange() {
      this.$emit('input', this.datasource);
    },
  },
};
</script>
