<template>
  <ct-card>
    <template v-slot:title>
      <span>Add Transformation</span>
    </template>

    <template v-slot:content>
      <ct-multiple-choice
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
    </template>

    <template v-slot:actions>
      <ct-spacer />
      <ct-btn
        v-if="currentTransformation !== '__ob__' && commands[currentTransformation]"
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
import AddShock from "./AddShock.vue";

const endpoint = "https://localhost:5007/features";

export default {
  components: {
    AddShock
  },
  props: {
    id: {
      type: String,
      required: true
    },
    commands: {
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
      loading: false,
      currentTransformation: "ParallelShock",
      errors: []
    };
  },
  methods: {
    submit() {
      const transformationName = this.currentTransformation;

      const payload = {
        id: this.id,
        transformationName,
        transformation: this.commands[transformationName]
      };

      this.loading = true;
      this.$axios
        .$post(`${endpoint}/add-transformation`, payload)
        .then(() => this.$emit("success"))
        .catch(e => {
          if (e.response.data && Array.isArray(e.response.data))
            this.errors = e.response.data;
        });
      this.loading = false;
    }
  }
};
</script>
