<template>
  <md-card>
    <md-card-header>
      <div class="md-title">Create new curve recipe</div>
    </md-card-header>
    <md-progress-bar v-if="loading" md-mode="indeterminate"></md-progress-bar>
    <md-card-content v-else>
      <text-box label="ShortName" id="shortNameBox" v-model="command.shortName" />
      <text-box
        label="Description"
        id="descriptionBox"
        v-model="command.description"
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
        type="number"
        max="100"
        min="0"
        step="0.1"
        v-model="command.outputFrequency.maximumMaturity"
        label="Maximum maturity"
        id="maximumMaturityBox"
      ></text-box>
      <mt-select
        id="outputTypeDropdown"
        v-model="command.outputType"
        label="Output type"
        :options="outputTypes"
      />
      <ul v-if="errors.length">
        <li v-for="error in errors" :key="error">
          {{error}}
        </li>
      </ul>
      <md-button v-on:click="this.submit" class="md-raised md-primary">Submit</md-button>
    </md-card-content>
  </md-card>
</template>

<script>
import axios from 'axios';

import MtSelect from './Material/MtSelect.vue';
import TextBox from './Material/TextBox.vue';

const endpoint = 'https://localhost:5007';

export default {
  components: {
    MtSelect,
    TextBox,
  },
  props: ['command', 'marketCurves', 'tenors', 'dayCountConventions', 'interpolations', 'extrapolationShorts', 'extrapolationLongs', 'outputSeries', 'outputTypes'],
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
  data() {
    return {
      loading: false,
      errors: [],
    };
  },
  methods: {
    submit() {
      if (this.command.floatingLeg === 'N/A') {
        this.command.floatingLeg = null;
      }
      this.loading = true;
      axios.post(`${endpoint}/api`, this.command).catch((e) => {
        if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
      });
      this.loading = false;
    },
  },
};
</script>
