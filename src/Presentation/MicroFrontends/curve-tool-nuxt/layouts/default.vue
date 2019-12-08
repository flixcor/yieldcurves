<template>
  <v-app>
    <v-navigation-drawer
      v-model="drawer"
      :mini-variant="miniVariant"
      :clipped="clipped"
      fixed
      app
    >
      <v-list>
        <v-list-item
          v-for="(item, i) in items"
          :key="i"
          :to="item.to"
          router
          exact
        >
          <v-list-item-action>
            <v-icon>{{ item.icon }}</v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title v-text="item.title" />
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-navigation-drawer>
    <v-app-bar
      :clipped-left="clipped"
      fixed
      app
    >
      <v-app-bar-nav-icon @click.stop="drawer = !drawer" />
      <v-btn
        icon
        @click.stop="miniVariant = !miniVariant"
      >
        <v-icon>mdi-{{ `chevron-${miniVariant ? 'right' : 'left'}` }}</v-icon>
      </v-btn>
      <v-toolbar-title v-text="title" />
      <v-spacer />
      <v-switch
        v-model="dark"
        label="Dark Theme"
      />
    </v-app-bar>
    <v-content>
      <v-container>
        <nuxt />
      </v-container>
    </v-content>
    <v-footer
      :fixed="fixed"
      app
    >
      <span>&copy; {{ year }}</span>
    </v-footer>
  </v-app>
</template>

<script>
export default {
  data () {
    return {
      year: new Date().getFullYear(),
      clipped: false,
      drawer: true,
      fixed: false,
      dark: true,
      items: [
        {
          icon: 'mdi-trumpet',
          title: 'Instruments',
          to: '/instruments'
        },
        {
          icon: 'mdi-shopping',
          title: 'Market Curves',
          to: '/marketcurves'
        },
        {
          icon: 'mdi-chef-hat',
          title: 'Recipes',
          to: '/recipes'
        }
      ],
      miniVariant: false,
      right: true,
      title: 'Curve tool'
    }
  },
  watch: {
    dark (val) {
      this.$vuetify.theme.dark = !!val
    }
  }
}

</script>
