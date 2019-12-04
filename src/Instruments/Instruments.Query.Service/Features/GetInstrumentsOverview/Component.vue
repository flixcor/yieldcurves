<template>
  <v-card>
    <v-card-title>
      <span>
        Instruments
      </span>
    </v-card-title>
    <v-card-text>
      <v-simple-table>
        <template v-slot:default>
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
        </template>
      </v-simple-table>
    </v-card-text>
    <v-card-actions>
      <v-spacer />
      <v-btn
        fab
        @click="onCreateClick"
      >
        <v-icon>mdi-plus</v-icon>
      </v-btn>
    </v-card-actions>
  </v-card>
</template>
<script>
export default {
  props: ["entities"],
  methods: {
    onCreateClick () {
      this.$emit("create");
    },
  }
};
</script>