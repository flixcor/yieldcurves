<template>
  <frame-live-feed
    endpoint="http://localhost:5006/features/create-curve-recipe"
    @success="$router.push({path: `/recipes/${$event}`})"
  />
</template>
