<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Create new curve recipe
      </span>
    </template>

    <template v-slot:content>
      <ct-input
        v-model="command.shortName"
        label="ShortName"
      />
      <ct-input
        v-model="command.description"
        label="Description"
      />
      <ct-multiple-choice
        v-model="command.marketCurveId"
        label="Market curve"
        :options="marketCurveOptions"
      />
      <ct-multiple-choice
        v-if="matchingTenors"
        v-model="command.lastLiquidTenor"
        label="Last liquid tenor"
        :options="matchingTenors"
      />
      <ct-multiple-choice
        v-model="command.dayCountConvention"
        label="Day count convention"
        :options="dayCountConventions"
      />
      <ct-multiple-choice
        v-model="command.interpolation"
        label="Interpolation"
        :options="interpolations"
      />
      <ct-multiple-choice
        v-model="command.extrapolationShort"
        label="Extrapolation short end"
        :options="extrapolationShorts"
      />
      <ct-multiple-choice
        v-model="command.extrapolationLong"
        label="Extrapolation long end"
        :options="extrapolationLongs"
      />
      <ct-multiple-choice
        v-model="command.outputFrequency.outputSeries"
        label="Output series"
        :options="outputSeries"
      />
      <ct-input
        v-model="command.outputFrequency.maximumMaturity"
        type="number"
        max="100"
        min="0"
        step="0.1"
        label="Maximum maturity"
      />
      <ct-multiple-choice
        v-model="command.outputType"
        label="Output type"
        :options="outputTypes"
      />
      <ul v-if="errors.length">
        <li
          v-for="error in errors"
          :key="error"
        >
          {{ error }}
        </li>
      </ul>
    </template>

    <template v-slot:actions>
      <ct-spacer />
      <ct-btn
        class="primary"
        fab
        @click="submit"
      >
        <v-icon>mdi-send</v-icon>
      </ct-btn>
    </template>
  </ct-card>
</template>

<script>
const endpoint = "https://localhost:5007/features/create-curve-recipe";

export default {
  props: {
    command: {
      type: Object,
      required: true
    },
    marketCurves: {
      type: Array,
      required: true
    },
    tenors: {
      type: Array,
      required: true
    },
    dayCountConventions: {
      type: Array,
      required: true
    },
    interpolations: {
      type: Array,
      required: true
    },
    extrapolationShorts: {
      type: Array,
      required: true
    },
    extrapolationLongs: {
      type: Array,
      required: true
    },
    outputSeries: {
      type: Array,
      required: true
    },
    outputTypes: {
      type: Array,
      required: true
    }
  },
  data() {
    return {
      errors: []
    };
  },
  computed: {
    matchingTenors() {
      const match = this.marketCurves.find(
        x => x.id === this.command.marketCurveId
      );
      if (match) {
        return match.tenors;
      }
      return false;
    },
    marketCurveOptions() {
      return this.marketCurves.map(x => {
        return { key: x.id, value: x.name };
      });
    }
  },
  methods: {
    submit() {
      if (this.command.floatingLeg === "N/A") {
        this.command.floatingLeg = null;
      }
      this.loading = true;
      this.$axios
        .$post(endpoint, this.command)
        .then(() => this.$emit("success", this.command.id))
        .catch(e => {
          if (e.response.data && Array.isArray(e.response.data))
            this.errors = e.response.data;
        });
    }
  }
};
</script>
