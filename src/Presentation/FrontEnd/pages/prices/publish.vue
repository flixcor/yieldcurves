<template>
  <frame-live-feed
    endpoint="https://localhost:5013/features/publish-price"
    @success="$router.push({path: '/prices'})"
  />
</template>
