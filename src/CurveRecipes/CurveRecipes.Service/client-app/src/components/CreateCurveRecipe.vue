<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Create new curve recipe</div>
    </md-card-header>
    <md-card-content v-if="commandViewModel">
      <text-box label="ShortName" id="shortNameBox" v-model="commandViewModel.command.shortName" />
      <text-box
        label="Description"
        id="descriptionBox"
        v-model="commandViewModel.command.description"
      />
      <mt-select
        id="marketCurveDropdown"
        v-model="commandViewModel.command.marketCurveId"
        label="Market curve"
        :options="commandViewModel.marketCurves.map(x=> x.id)"
      />
      <mt-select
        v-if="this.tenors"
        id="lastLiquidTenorDropdown"
        v-model="commandViewModel.command.lastLiquidTenor"
        label="Last liquid tenor"
        :options="tenors"
      />
      <mt-select
        id="dayCountConventionDropdown"
        v-model="commandViewModel.command.dayCountConvention"
        label="Day count convention"
        :options="commandViewModel.dayCountConventions"
      />
      <mt-select
        id="interpolationDropdown"
        v-model="commandViewModel.command.interpolation"
        label="Interpolation"
        :options="commandViewModel.interpolations"
      />
      <mt-select
        id="extrapolationShortDropdown"
        v-model="commandViewModel.command.extrapolationShort"
        label="Extrapolation short end"
        :options="commandViewModel.extrapolationShorts"
      />
      <mt-select
        id="extrapolationLongDropdown"
        v-model="commandViewModel.command.extrapolationLong"
        label="Extrapolation long end"
        :options="commandViewModel.extrapolationLongs"
      />
      <mt-select
        id="outputSeriesDropdown"
        v-model="commandViewModel.command.outputFrequency.outputSeries"
        label="Output series"
        :options="commandViewModel.outputSeries"
      />
      <text-box
        type="number"
        max="100"
        min="0"
        step="0.1"
        v-model="commandViewModel.command.outputFrequency.maximumMaturity"
        label="Maximum maturity"
        id="maximumMaturityBox"
      ></text-box>
      <mt-select
        id="outputTypeDropdown"
        v-model="commandViewModel.command.outputType"
        label="Output type"
        :options="commandViewModel.outputTypes"
      />
      <md-button v-on:click="this.submit" class="md-raised md-primary">Submit</md-button>
    </md-card-content>
    <md-progress-bar v-else md-mode="indeterminate"></md-progress-bar>
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
  computed: {
    tenors() {
      if (this.commandViewModel) {
        const match = this.commandViewModel.marketCurves
          .find(x => x.id === this.commandViewModel.command.marketCurveId);
        if (match) {
          return match.tenors;
        }
      }
      return false;
    },
  },
  data() {
    return {
      commandViewModel: null,
      errors: [],
    };
  },
  methods: {
    submit() {
      if (this.commandViewModel.command.floatingLeg === 'N/A') {
        this.commandViewModel.command.floatingLeg = null;
      }

      axios.post(`${endpoint}/api`, this.commandViewModel.command).catch((e) => {
        this.errors.push(e);
      });
    },
    initialize() {
      axios
        .get(`${endpoint}/api`)
        .then((response) => {
          // JSON responses are automatically parsed.
          this.commandViewModel = response.data;
        })
        .catch((e) => {
          this.errors.push(e);
        });
    },
  },
  created() {
    this.initialize();
  },
};
</script>
