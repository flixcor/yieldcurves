<template>
  <ct-card>
    <template v-slot:title>
      <span>Instruments</span>
      <ct-spacer />
      <ct-btn
        class="primary"
        fab
        @click="onCreateClick"
      >
        <v-icon>mdi-plus</v-icon>
      </ct-btn>
    </template>

    <template v-slot:content>
      <ct-table>
        <thead>
          <tr>
            <th>Vendor</th>
            <th>Description</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="instrument of entities"
            :key="instrument.id"
          >
            <td>{{ instrument.vendor }}</td>
            <td>{{ instrument.description }}</td>
          </tr>
        </tbody>
      </ct-table>
    </template>
  </ct-card>
</template>
<script>
export default {
  props: {
    entities: {
      type: Array,
      required: true
    }
  },
  methods: {
    onCreateClick() {
      this.$emit("create");
    }
  }
};
</script>