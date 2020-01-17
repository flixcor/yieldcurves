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
            <th scope="col">
              Name
            </th>
            <th scope="col">
              Parameters
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="(transformation, i) of transformations"
            :key="i"
          >
            <td>{{ transformation.name }}</td>
            <td>
              <ct-table>
                <tbody>
                  <tr
                    v-for="(parameter, j) of transformation.parameters"
                    :key="`${i}_${j}`"
                  >
                    <td>{{ parameter.name }}</td>
                    <td>{{ parameter.value }}</td>
                  </tr>
                </tbody>
              </ct-table>
            </td>
          </tr>
        </tbody>
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
