<template>
  <frame-live-feed
    endpoint="http://localhost:5012/features/publish-price"
    @success="$router.push({path: '/prices'})"
  />
</template>
