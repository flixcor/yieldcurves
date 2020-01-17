<template>
  <ct-card>
    <template v-slot:title>
      <span>
        Curve Recipes
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
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="curve of entities"
            :key="curve.id"
            @click="emitDetailClicked(curve.id)"
          >
            <td>{{ curve.name }}</td>
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
    entities: {
      required: true,
      type: Array,
    },
  },
  methods: {
    emitDetailClicked(id) {
      this.$emit('detailClicked', id);
    },
    emitCreateClicked() {
      this.$emit('createClicked');
    },
  },
};
</script>
