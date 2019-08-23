<template>
  <div>
      <text-box
        type="number"
        label="Order"
        id="orderBox"
        v-model="value.order"
      />
      <mt-select
        id="shockTargetDropdown"
        v-model="value.shockTarget"
        label="Shock target"
        :options="shockTargets"
      />
      <text-box
        type="number"
        label="Shift"
        id="shiftBox"
        v-model="value.shift"
      />
      <template
       v-if="value.maturities"
      >
        <text-box
         v-for="(maturity, index) of value.maturities"
         type="number"
         :key="index"
         :label="`Maturity ${index + 1}`"
         :id="`maturitiesBox[${index}]`"
         v-model="value.maturities[index]"
        />
        <text-box
         type="number"
         :label="`Maturity ${value.maturities.length + 1}`"
         id="newMaturityBox"
         v-on:change="onNewMaturityBlur()"
         v-model="newMaturity"
        />
      </template>
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
      newMaturity: false,
    };
  },
  methods: {
    emitChange() {
      this.$emit('input', this.value);
    },
    onNewMaturityBlur() {
      if (!Array.isArray(this.value.maturities)) {
        this.value.maturities = [];
      }
      if (this.newMaturity) {
        this.value.maturities.push(this.newMaturity);
        this.newMaturity = false;
      }
    },
  },
};
</script>
