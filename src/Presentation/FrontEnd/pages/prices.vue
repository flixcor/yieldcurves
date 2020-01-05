<template>
  <v-container>
    <h2>Prices</h2>
    <v-row>
      <v-col>
        <nuxt-child />
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <ct-card>
          <template v-slot:title>
            <frame-live-feed
              :endpoint="`${baseUrl}/get-price-dates`"
              @input="onDateChange($event)"
            />
            <ct-spacer />
            <ct-btn
              class="primary"
              fab
              @click="$router.push({path: '/prices/publish'})"
            >
              <v-icon>mdi-plus</v-icon>
            </ct-btn>
          </template>

          <template v-slot:content>
            <frame-live-feed
              v-if="selectedDate"
              :endpoint="`${baseUrl}/get-prices-overview?asOfDate=${selectedDate}`"
            />
          </template>
        </ct-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  name: 'Prices',
  data () {
    return {
      baseUrl: 'http://localhost:5090/features',
      selectedDate: null,
      showCreate: false
    }
  },
  methods: {
    onDateChange (e) {
      this.selectedDate = e
    }
  }
}
</script>
