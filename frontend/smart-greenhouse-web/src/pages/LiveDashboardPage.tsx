import { LiveReadingsPanel } from '../components/LiveReadingsPanel'

export default function LiveDashboardPage() {
  return (
    <div className="p-6 space-y-6">
      <div className="flex items-center justify-between">
        <h1 className="text-2xl font-bold">Live Dashboard</h1>
        <div className="text-sm text-gray-600">ESP32 → MQTT → backend → WebSocket → UI</div>
      </div>

      <LiveReadingsPanel />
    </div>
  )
}
