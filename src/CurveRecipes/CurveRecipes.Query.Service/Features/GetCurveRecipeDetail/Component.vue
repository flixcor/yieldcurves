<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Transformations for {{ name }}
      </span>
      <ct-spacer />
      <ct-btn
        class="primary"
        fab
        @click="emitCreateClicked()"
      >
        <v-icon>mdi-plus</v-icon>
      </ct-btn>
    </template>

    <template v-slot:content>
      <ct-table>
        <thead>
          <tr>
            <th>Name</th>
          </tr>
        </thead>
        <tr>
          <th>Transformation</th>
          <th>Parameter</th>
          <th>Value</th>
        </tr>
        <template v-for="(transformation, i) of transformations">
          <tr
            v-for="(parameter, j) of transformation.parameters"
            :key="`${i}_${j}`"
          >
            <td>{{ transformation.name }}</td>
            <td>{{ parameter.name }}</td>
            <td>{{ parameter.value }}</td>
          </tr>
        </template>
      </ct-table>
    </template>
  </ct-card>
</template>

<script>
export default {
  name: 'GetCurveRecipes',
  props: {
    name: {
      type: String,
      required: true
    },
    transformations: {
      type: Array,
      required: true
    }
  },
  methods: {
    emitCreateClicked() {
      this.$emit('createClicked');
    },
  },
};
</script>
