<template>
  <div>
    <ct-multiple-choice
      v-model="value.shockTarget"
      label="Shock target"
      :options="shockTargets"
    />
    <ct-input
      v-model="value.shift"
      type="number"
      label="Shift"
    />
    <template v-if="value.maturities">
      <ct-input
        v-for="(maturity, index) of value.maturities"
        :key="index"
        v-model="value.maturities[index]"
        type="number"
        :label="`Maturity ${index + 1}`"
      />
      <ct-input
        v-model="newMaturity"
        type="number"
        :label="`Maturity ${value.maturities.length + 1}`"
        @change="onNewMaturityBlur()"
      />
    </template>
  </div>
</template>

<script>
export default {
  props: {
    value: {
      type: Object,
      required: true
    },
    shockTargets: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      newMaturity: null
    };
  },
  methods: {
    onNewMaturityBlur() {
      if (!Array.isArray(this.value.maturities)) {
        this.value.maturities = [];
      }
      if (this.newMaturity) {
        this.value.maturities.push(this.newMaturity);
        this.newMaturity = null;
      }
    }
  }
};
</script>
