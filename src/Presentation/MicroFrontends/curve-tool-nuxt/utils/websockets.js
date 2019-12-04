import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr'
export default function getConnection (url) {
  const connection = new HubConnectionBuilder()
    .withUrl(url)
    .withAutomaticReconnect()
    .configureLogging(LogLevel.Information)
    .build()

  return connection
}
