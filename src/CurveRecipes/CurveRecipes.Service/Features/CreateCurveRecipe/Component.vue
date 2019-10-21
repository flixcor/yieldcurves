<template>
  <md-card>
    <md-card-header>
      <div class="md-title">
        Create new curve recipe
      </div>
    </md-card-header>
    <md-progress-bar
      v-if="loading"
      md-mode="indeterminate"
    />
    <md-card-content v-else>
      <text-box
        id="shortNameBox"
        v-model="command.shortName"
        label="ShortName"
      />
      <text-box
        id="descriptionBox"
        v-model="command.description"
        label="Description"
      />
      <mt-select
        id="marketCurveDropdown"
        v-model="command.marketCurveId"
        label="Market curve"
        :options="marketCurves.map(x=> x.id)"
      />
      <mt-select
        v-if="matchingTenors"
        id="lastLiquidTenorDropdown"
        v-model="command.lastLiquidTenor"
        label="Last liquid tenor"
        :options="matchingTenors"
      />
      <mt-select
        id="dayCountConventionDropdown"
        v-model="command.dayCountConvention"
        label="Day count convention"
        :options="dayCountConventions"
      />
      <mt-select
        id="interpolationDropdown"
        v-model="command.interpolation"
        label="Interpolation"
        :options="interpolations"
      />
      <mt-select
        id="extrapolationShortDropdown"
        v-model="command.extrapolationShort"
        label="Extrapolation short end"
        :options="extrapolationShorts"
      />
      <mt-select
        id="extrapolationLongDropdown"
        v-model="command.extrapolationLong"
        label="Extrapolation long end"
        :options="extrapolationLongs"
      />
      <mt-select
        id="outputSeriesDropdown"
        v-model="command.outputFrequency.outputSeries"
        label="Output series"
        :options="outputSeries"
      />
      <text-box
        id="maximumMaturityBox"
        v-model="command.outputFrequency.maximumMaturity"
        type="number"
        max="100"
        min="0"
        step="0.1"
        label="Maximum maturity"
      />
      <mt-select
        id="outputTypeDropdown"
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
      <md-button
        class="md-raised md-primary"
        @click="submit"
      >
        Submit
      </md-button>
    </md-card-content>
  </md-card>
</template>

<script>
import axios from 'axios';

import MtSelect from '../Common/Material/MtSelect.vue';
import TextBox  from '../Common/Material/TextBox.vue';

const endpoint = 'https://localhost:5007/features/create-curve-recipe';

export default {
  components: {
    MtSelect,
    TextBox,
  },
  props: ['command', 'marketCurves', 'tenors', 'dayCountConventions', 'interpolations', 'extrapolationShorts', 'extrapolationLongs', 'outputSeries', 'outputTypes'],
  data() {
    return {
      loading: false,
      errors: [],
    };
  },
  computed: {
    matchingTenors() {
      const match = this.marketCurves
        .find(x => x.id === this.command.marketCurveId);
      if (match) {
        return match.tenors;
      }
      return false;
    },
  },
  methods: {
    submit() {
      if (this.command.floatingLeg === 'N/A') {
        this.command.floatingLeg = null;
      }
      this.loading = true;
      axios.post(endpoint, this.command)
        .then(() => this.$emit('success'))
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
      this.loading = false;
    },
  },
};
</script>
