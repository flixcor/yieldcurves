<template>
  <md-card>
    <md-card-header>
      <div class="md-title">
        Add Transformation
      </div>
    </md-card-header>
    <md-progress-bar
      v-if="loading"
      md-mode="indeterminate"
    />
    <md-card-content v-else>
      <mt-select
        id="transformationDropdown"
        v-model="currentTransformation"
        label="Transformation"
        :options="Object
          .getOwnPropertyNames(commands)
          .map(x=> x.toString())
          .filter(x=> x !== '__ob__')"
      />
      <add-shock
        v-if="currentTransformation === 'ParallelShock'"
        v-model="commands.ParallelShock"
        :shock-targets="shockTargets"
      />
      <add-shock
        v-if="currentTransformation === 'KeyRateShock'"
        v-model="commands.KeyRateShock"
        :shock-targets="shockTargets"
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
        v-if="commands[currentTransformation] && currentTransformation !== '__ob__'"
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
import AddShock from './AddShock/Component.vue';

const endpoint = 'https://localhost:5007/api';

export default {
  components: {
    AddShock,
    MtSelect,
  },
  props: ['commands', 'shockTargets'],
  data () {
    return {
      loading: false,
      currentTransformation: 'ParallelShock',
      errors: [],
    };
  },
  methods: {
    submit () {
      const transformationName = this.currentTransformation;
      this.loading = true;
      axios.post(`${endpoint}/add${transformationName}`, this.commands[transformationName])
        .then(() => this.$emit('success'))
        .catch((e) => {
          if (e.response.data && Array.isArray(e.response.data)) this.errors = e.response.data;
        });
      this.loading = false;
    },
  },
};
</script>
