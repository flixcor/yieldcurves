<template>
  <frame-live-feed
    endpoint="http://localhost:5000/features/create-market-curve"
    @success="$router.push({path: '/marketcurves'})"
  />
</template>
