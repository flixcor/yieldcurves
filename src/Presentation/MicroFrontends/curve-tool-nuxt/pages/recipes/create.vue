<template>
  <frame-live-feed
    endpoint="https://localhost:5007/features/create-curve-recipe"
    @success="$router.push({path: `/recipes/${$event}`})"
  />
</template>
