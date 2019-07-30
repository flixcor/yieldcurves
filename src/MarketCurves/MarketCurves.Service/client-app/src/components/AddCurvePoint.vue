<template>
  <md-card class="md-layout-item md-size-50 md-small-size-100">
    <md-card-header>
      <div class="md-title">Add new curve point</div>
    </md-card-header>
    <md-card-content v-if="commandViewModel">
      <mt-select
        id="tenorDropdown"
        v-model="commandViewModel.command.tenor"
        label="Tenor"
        :options="commandViewModel.tenors"
      />
      <mt-select
        id="instrumentDropdown"
        v-model="commandViewModel.command.instrumentId"
        label="Instrument"
        :options="commandViewModel.instruments.map(x=> x.id)"
      />
      <text-box
        type="number"
        max="0"
        v-model="this.commandViewModel.command.dateLag"
        label="DateLag"
        id="dateLagBox"
      ></text-box>

      <md-checkbox
        v-model="commandViewModel.command.isMandatory"
        >Mandatory
      </md-checkbox>

      <br/>

      <mt-select
        id="priceTypeDropdown"
        v-model="commandViewModel.command.priceType"
        label="Price Type"
        :options="commandViewModel.priceTypes"
        v-if="hasPriceType"
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

const endpoint = 'https://localhost:44348/api/curvepoint';

export default {
  components: {
    MtSelect,
    TextBox,
  },
  props: ['id'],
  data() {
    return {
      commandViewModel: null,
      errors: [],
    };
  },
  computed: {
    hasPriceType() {
      if (!this.commandViewModel) {
        return false;
      }
      const match = this.commandViewModel.instruments
        .find(x => x.id === this.commandViewModel.command.instrumentId);

      const hasPriceType = match && match.hasPriceType;

      return hasPriceType;
    },
  },
  methods: {
    submit() {
      if (!this.hasPriceType) {
        this.commandViewModel.command.priceType = null;
      }

      axios.post(endpoint, this.commandViewModel.command).catch((e) => {
        this.errors.push(e);
      });
    },
    initialize() {
      axios
        .get(`${endpoint}/${this.id}`)
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
