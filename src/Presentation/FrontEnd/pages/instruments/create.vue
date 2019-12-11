<template>
  <frame-live-feed
    endpoint="https://localhost:5011/features/get-commands"
    @success="$router.push({path: '/instruments'})"
  />
</template>
