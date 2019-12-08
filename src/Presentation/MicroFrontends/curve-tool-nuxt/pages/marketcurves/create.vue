<template>
  <frame-live-feed
    endpoint="https://localhost:5001/features/create-market-curve"
    @success="$router.push({path: '/marketcurves'})"
  />
</template>
